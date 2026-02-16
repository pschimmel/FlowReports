using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using FlowReports.Model;
using FlowReports.Model.Events;
using FlowReports.Model.ImportExport;
using FlowReports.Model.ReportItems;
using FlowReports.ViewModel.EditorItems;

namespace FlowReports.ViewModel
{
  /// <summary>
  /// Provides view model functionality for managing a report and its bands and items.
  /// </summary>
  public class ReportViewModel : ViewModelBase, IBandParentViewModel
  {
    #region Events

    internal event EventHandler SelectionChanged;

    #endregion

    #region Fields

    private ReportBandViewModel _selectedBand;
    private IEditorItemViewModel _selectedItem;
    private bool _isDirty;
    private readonly ReportBandCollection _subBands;
    private HeaderBand _oldHeaderBand;
    private FooterBand _oldFooterBand;
    private ActionCommand _addNewBandCommand;
    private ActionCommand _addSubBandCommand;
    private ActionCommand _editBandDetailsCommand;
    private ActionCommand _removeBandCommand;
    private ActionCommand _moveBandUp;
    private ActionCommand _moveBandDown;
    private ActionCommand _addTextItemCommand;
    private ActionCommand _addBooleanItemCommand;
    private ActionCommand _addImageItemCommand;
    private ActionCommand _removeItemCommand;
    private ActionCommand _cutCommand;
    private ActionCommand _copyCommand;
    private ActionCommand _pasteCommand;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportViewModel"/> class.
    /// </summary>
    /// <param name="report">The report model to manage.</param>
    public ReportViewModel(Report report)
    {
      _subBands = report.Bands;
      _subBands.SubBandAdded += SubBands_SubBandAdded;
      _subBands.SubBandRemoved += SubBands_SubBandRemoved;

      foreach (var subBand in _subBands)
      {
        var newReportBandVM = new ReportBandViewModel(subBand) { Parent = this };
        newReportBandVM.SelectionChanged += ReportBandVM_SelectionChanged;
        Bands.Add(newReportBandVM);
      }

      Report = report;
      SelectionChanged += ReportVM_SelectionChanged;
      DataSourceVM = new DataSourceViewModel[] { new DataSourceViewModel(report.DataSource) };
      IsDirty = false;
    }

    #endregion

    #region Properties

    public ObservableCollection<ReportBandViewModel> Bands { get; } = new ObservableCollection<ReportBandViewModel>();


    /// <summary>
    /// Gets or sets the selected band in the report.
    /// </summary>
    public ReportBandViewModel SelectedBand
    {
      get => _selectedBand;
      set
      {
        if (_selectedBand != value)
        {
          if (_selectedBand != null)
          {
            _selectedBand.ItemSelected -= SelectedBand_ItemSelected;
          }

          _selectedBand = value;

          if (_selectedBand != null)
          {
            _selectedBand.ItemSelected += SelectedBand_ItemSelected;
          }

          OnPropertyChanged();
          OnPropertyChanged(nameof(IsBandSelected));
          _addSubBandCommand.RaiseCanExecuteChanged();
          _removeBandCommand.RaiseCanExecuteChanged();
          _addTextItemCommand.RaiseCanExecuteChanged();
          _pasteCommand.RaiseCanExecuteChanged();
        }
      }
    }

    /// <summary>
    /// Gets or sets the selected item in the report.
    /// </summary>
    public IEditorItemViewModel SelectedItem
    {
      get => _selectedItem;
      set
      {
        if (_selectedItem != value)
        {
          if (_selectedItem != null)
          {
            _selectedItem.PropertyChanged -= SelectedItem_PropertyChanged;
          }

          _selectedItem = value;

          if (_selectedItem != null)
          {
            _selectedItem.PropertyChanged += SelectedItem_PropertyChanged;
          }

          OnPropertyChanged();
          _removeItemCommand.RaiseCanExecuteChanged();
          _cutCommand.RaiseCanExecuteChanged();
          _copyCommand.RaiseCanExecuteChanged();
        }
      }
    }

    private IBandParentViewModel SelectedBandParent => SelectedBand?.Parent ?? this;

    /// <summary>
    /// Gets a value indicating whether the report has unsaved changes.
    /// </summary>
    public bool IsDirty
    {
      get => _isDirty;
      internal set
      {
        if (_isDirty != value)
        {
          _isDirty = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Gets or sets the file path of the report.
    /// </summary>
    public string FilePath
    {
      get => Report.FilePath;
      set => Report.FilePath = value;
    }

    /// <summary>
    /// Gets the data source view models associated with the report.
    /// </summary>
    public IEnumerable<DataSourceViewModel> DataSourceVM { get; }

    internal Report Report { get; private set; }

    public bool IsBandSelected => SelectedBand != null;

    public bool IsHeaderBandActive
    {
      get => SelectedBand?.HeaderBand != null;
      set
      {
        if (SelectedBand is ReportBandViewModel vm)
        {
          if (value)
          {
            vm.Band.HeaderBand = _oldHeaderBand ?? new HeaderBand();
          }
          else
          {
            _oldHeaderBand = vm.Band.HeaderBand;
            vm.Band.HeaderBand = null;
          }
          OnPropertyChanged();
        }
      }
    }

    public bool IsFooterBandActive
    {
      get => SelectedBand?.FooterBand != null;
      set
      {
        if (SelectedBand is ReportBandViewModel vm)
        {
          if (value)
          {
            vm.Band.FooterBand = _oldFooterBand ?? new FooterBand();
          }
          else
          {
            _oldFooterBand = vm.Band.FooterBand;
            vm.Band.FooterBand = null;
          }
          OnPropertyChanged();
        }
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates a new empty report view model.
    /// </summary>
    /// <returns>A new ReportViewModel for an empty report.</returns>
    internal static ReportViewModel NewReport()
    {
      var report = new Report();
      var vm = new ReportViewModel(report)
      {
        IsDirty = true
      };

      return vm;
    }

    /// <summary>
    /// Loads a report from the specified file path.
    /// </summary>
    /// <param name="filePath">The path to the report file to load.</param>
    /// <returns>A ReportViewModel for the loaded report.</returns>
    public static ReportViewModel LoadReport(string filePath)
    {
      var report = ReportReader.Read(filePath);
      var vm = new ReportViewModel(report)
      {
        FilePath = filePath
      };
      return vm;
    }

    /// <summary>
    /// Saves the report to its current file path.
    /// </summary>
    public void SaveReport()
    {
      if (FilePath == null)
      {
        throw new ApplicationException("Don't know where to save to.");
      }

      ReportWriter.Write(Report, FilePath);
      IsDirty = false;
    }

    /// <summary>
    /// Saves the report to the specified file path.
    /// </summary>
    /// <param name="filePath">The file path where the report should be saved.</param>
    public void SaveReport(string filePath)
    {
      FilePath = filePath;
      SaveReport();
    }

    /// <summary>
    /// Attaches data to the report and analyzes the data structure.
    /// </summary>
    /// <typeparam name="T">The type of items in the data collection.</typeparam>
    /// <param name="items">The data items to attach.</param>
    /// <param name="dataSourceName">The name of the data source.</param>
    public void Attach<T>(IEnumerable<T> items, string dataSourceName) where T : class
    {
      Report.Analyze(items, dataSourceName);
    }

    public void AddBand()
    {
      _subBands.AddBand();
    }

    public void AddBand(ReportBandViewModel otherBand, InsertLocation location)
    {
      _subBands.AddBand(otherBand.Band, location);
    }

    public void RemoveBand(ReportBandViewModel subBand)
    {
      _subBands.RemoveBand(subBand.Band);
    }

    protected void OnSelectionChanged()
    {
      SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void MoveBandUp(ReportBandViewModel band)
    {
      _subBands.MoveBandUp(band.Band);
    }

    public void MoveBandDown(ReportBandViewModel band)
    {
      _subBands.MoveBandDown(band.Band);
    }

    public bool CanMoveBandUp(ReportBandViewModel band)
    {
      return _subBands.CanMoveBandUp(band.Band);
    }

    public bool CanMoveBandDown(ReportBandViewModel band)
    {
      return _subBands.CanMoveBandDown(band.Band);
    }

    #endregion

    #region Commands

    #region Add Band

    /// <summary>
    /// Gets the command to add a new band to the report.
    /// </summary>
    public ICommand AddBandCommand => _addNewBandCommand ??= new ActionCommand(AddNewBand, CanAddNewBand);

    private void AddNewBand()
    {
      if (SelectedBand == null)
      {
        // Nothing selected -> Add to report
        AddBand();
      }
      else
      {
        // Selected band is on the report -> Add to report after selected band
        SelectedBandParent.AddBand(SelectedBand, InsertLocation.After);
      }
    }

    private bool CanAddNewBand()
    {
      return true;
    }

    #endregion

    #region Add Sub Band

    /// <summary>
    /// Gets the command to add a sub-band to the selected band.
    /// </summary>
    public ICommand AddSubBandCommand => _addSubBandCommand ??= new ActionCommand(AddSubBand, CanAddSubBand);

    private void AddSubBand()
    {
      SelectedBand?.AddBand();
    }

    private bool CanAddSubBand()
    {
      return SelectedBand != null;
    }

    #endregion

    #region Edit Band Details

    /// <summary>
    /// Gets the command to edit the details of the selected band.
    /// </summary>
    public ICommand EditBandDetailsCommand => _editBandDetailsCommand ??= new ActionCommand(EditBandDetails, CanEditBandDetails);

    private void EditBandDetails()
    {
      SelectedBand?.EditDetailsCommand.Execute(null);
    }

    private bool CanEditBandDetails()
    {
      return SelectedBand != null && SelectedBand.EditDetailsCommand.CanExecute(null);
    }

    #endregion

    #region Remove Band

    /// <summary>
    /// Gets the command to remove the selected band.
    /// </summary>
    public ICommand RemoveBandCommand => _removeBandCommand ??= new ActionCommand(RemoveBand, CanRemoveBand);

    private void RemoveBand()
    {
      SelectedBand?.Parent?.RemoveBand(SelectedBand);
      SelectedBand = null;
    }

    private bool CanRemoveBand()
    {
      return SelectedBand != null;
    }

    #endregion

    #region Move Band Up

    /// <summary>
    /// Gets the command to move the selected band up.
    /// </summary>
    public ICommand MoveBandUpCommand => _moveBandUp ??= new ActionCommand(MoveBandUp, CanMoveBandUp);

    private void MoveBandUp()
    {
      SelectedBandParent?.MoveBandUp(SelectedBand);
    }

    private bool CanMoveBandUp()
    {
      return SelectedBand != null && SelectedBandParent != null && SelectedBandParent.CanMoveBandUp(SelectedBand);
    }

    #endregion

    #region Move Band Down

    /// <summary>
    /// Gets the command to move the selected band down.
    /// </summary>
    public ICommand MoveBandDownCommand => _moveBandDown ??= new ActionCommand(MoveBandDown, CanMoveBandDown);

    private void MoveBandDown()
    {
      SelectedBandParent?.MoveBandDown(SelectedBand);
    }

    private bool CanMoveBandDown()
    {
      return SelectedBand != null && SelectedBandParent != null && SelectedBandParent.CanMoveBandDown(SelectedBand);
    }

    #endregion

    #region Add Text Item

    /// <summary>
    /// Gets the command to add a text item to the selected band.
    /// </summary>
    public ICommand AddTextItemCommand => _addTextItemCommand ??= new ActionCommand(AddTextItem, CanAddTextItem);

    private void AddTextItem()
    {
      var newItem = SelectedBand.AddTextItem();
      SelectedItem = newItem;
    }

    private bool CanAddTextItem()
    {
      return SelectedBand != null;
    }

    #endregion

    #region Add Boolean Item

    /// <summary>
    /// Gets the command to add a boolean item to the selected band.
    /// </summary>
    public ICommand AddBooleanItemCommand => _addBooleanItemCommand ??= new ActionCommand(AddBooleanItem, CanAddBooleanItem);

    private void AddBooleanItem()
    {
      var newItem = SelectedBand.AddBooleanItem();
      SelectedItem = newItem;
    }

    private bool CanAddBooleanItem()
    {
      return SelectedBand != null;
    }

    #endregion

    #region Add Image Item

    /// <summary>
    /// Gets the command to add an image item to the selected band.
    /// </summary>
    public ICommand AddImageItemCommand => _addImageItemCommand ??= new ActionCommand(AddImageItem, CanAddImageItem);

    private void AddImageItem()
    {
      var newItem = SelectedBand.AddImageItem();
      SelectedItem = newItem;
    }

    private bool CanAddImageItem()
    {
      return SelectedBand != null;
    }

    #endregion

    #region Remove Item

    /// <summary>
    /// Gets the command to remove the selected item.
    /// </summary>
    public ICommand RemoveItemCommand => _removeItemCommand ??= new ActionCommand(RemoveItem, CanRemoveItem);

    private void RemoveItem()
    {
      SelectedBand?.RemoveItem(SelectedItem);
      SelectedItem = null;
    }

    private bool CanRemoveItem()
    {
      return SelectedItem != null;
    }

    #endregion

    #region Cut

    /// <summary>
    /// Gets the command to cut the selected item.
    /// </summary>
    public ICommand CutCommand => _cutCommand ??= new ActionCommand(Cut, CanCut);

    private void Cut()
    {
      if (SelectedItem != null)
      {
        var xml = ReportWriter.GetXMLRepresentation(new List<ReportItem> { SelectedItem.Item });
        Clipboard.SetData(DataFormats.UnicodeText, xml);
        _pasteCommand.RaiseCanExecuteChanged();
        RemoveItem();
      }
    }

    private bool CanCut()
    {
      return SelectedItem != null && CanRemoveItem();
    }

    #endregion

    #region Copy

    /// <summary>
    /// Gets the command to copy the selected item.
    /// </summary>
    public ICommand CopyCommand => _copyCommand ??= new ActionCommand(Copy, CanCopy);

    private void Copy()
    {
      if (SelectedItem != null)
      {
        var xml = ReportWriter.GetXMLRepresentation(new List<ReportItem> { SelectedItem.Item });
        Clipboard.SetData(DataFormats.UnicodeText, xml);
        _pasteCommand.RaiseCanExecuteChanged();
      }
    }

    private bool CanCopy()
    {
      return SelectedItem != null;
    }

    #endregion

    #region Paste

    /// <summary>
    /// Gets the command to paste an item from the clipboard.
    /// </summary>
    public ICommand PasteCommand => _pasteCommand ??= new ActionCommand(Paste, CanPaste);

    private void Paste()
    {
      var text = Clipboard.GetText();
      var items = Enumerable.Empty<ReportItem>();

      try
      {
        items = ReportReader.GetItems(text);
      }
      catch (Exception ex)
      {
        Debug.Fail(ex.Message);
      }

      foreach (var item in items)
      {
        SelectedBand.Band.AddReportItem(item);
      }
    }

    private bool CanPaste()
    {
      return Clipboard.ContainsText() && SelectedBand != null;
    }

    #endregion

    #endregion

    #region Event Handlers

    private void ReportBandVM_SelectionChanged(object sender, EventArgs e)
    {
      SelectionChanged?.Invoke(sender, EventArgs.Empty);
    }

    private void SelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      IsDirty = true;
    }

    private void SelectedBand_ItemSelected(object sender, EventArgs e)
    {
      if (sender is IEditorItemViewModel itemViewModel)
      {
        SelectedItem = itemViewModel;
      }
    }

    private void ReportVM_SelectionChanged(object sender, EventArgs e)
    {
      if (sender is ReportBandViewModel reportBandViewModel)
      {
        SelectedBand = reportBandViewModel.IsSelected ? reportBandViewModel : null;
      }
    }

    private void SubBands_SubBandAdded(object sender, BandsEventArgs e)
    {
      var newReportBandVM = new ReportBandViewModel(e.Item.Band) { Parent = this };
      newReportBandVM.SelectionChanged += ReportBandVM_SelectionChanged;
      Bands.Insert(e.Item.Index, newReportBandVM);

      if (!ReportEditorViewModel.IsInitializing)
      {
        newReportBandVM.IsSelected = true;
      }
      IsDirty = true;
    }

    private void SubBands_SubBandRemoved(object sender, BandsEventArgs e)
    {
      var bandVM = Bands.FirstOrDefault(x => x.Band == e.Item.Band);
      if (bandVM != null)
      {
        bandVM.SelectionChanged -= ReportBandVM_SelectionChanged;
        Bands.Remove(bandVM);
        bandVM.Dispose();
      }
      IsDirty = true;
    }

    #endregion

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);

      if (disposing)
      {
        SelectionChanged -= ReportVM_SelectionChanged;

        if (_selectedItem != null)
        {
          _selectedItem.PropertyChanged -= SelectedItem_PropertyChanged;
        }

        if (_selectedBand != null)
        {
          _selectedBand.ItemSelected -= SelectedBand_ItemSelected;
        }

        foreach (var dataSourceVM in DataSourceVM)
        {
          dataSourceVM.Dispose();
        }

        foreach (var subBand in Bands)
        {
          subBand.SelectionChanged -= ReportBandVM_SelectionChanged;
          subBand.Dispose();
        }

        _subBands.SubBandAdded -= SubBands_SubBandAdded;
        _subBands.SubBandRemoved -= SubBands_SubBandRemoved;
      }
    }

#endregion

  }
}
