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
  public class ReportViewModel : BandContainerViewModel
  {
    #region Fields

    private ReportBandViewModel _selectedBand;
    private IEditorItemViewModel _selectedItem;
    private bool _isDirty;
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

    public ReportViewModel(Report report)
      : base(report.Bands)
    {
      Report = report;
      SelectionChanged += ReportVM_SelectionChanged;
      DataSourceVM = new DataSourceViewModel[] { new DataSourceViewModel(report.DataSource) };
      IsDirty = false;
    }

    #endregion

    #region Properties

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
          _addSubBandCommand.RaiseCanExecuteChanged();
          _removeBandCommand.RaiseCanExecuteChanged();
          _addTextItemCommand.RaiseCanExecuteChanged();
          _pasteCommand.RaiseCanExecuteChanged();
        }
      }
    }

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

    public string FilePath
    {
      get => Report.FilePath;
      set => Report.FilePath = value;
    }

    public IEnumerable<DataSourceViewModel> DataSourceVM { get; }

    internal Report Report { get; private set; }

    #endregion

    #region Public Methods

    internal static ReportViewModel NewReport()
    {
      var report = new Report();
      var vm = new ReportViewModel(report)
      {
        IsDirty = true
      };

      return vm;
    }

    public static ReportViewModel LoadReport(string filePath)
    {
      var report = ReportReader.Read(filePath);
      var vm = new ReportViewModel(report)
      {
        FilePath = filePath
      };
      return vm;
    }

    public void SaveReport()
    {
      if (FilePath == null)
      {
        throw new ApplicationException("Don't know where to save to.");
      }

      ReportWriter.Write(Report, FilePath);
      IsDirty = false;
    }

    public void SaveReport(string filePath)
    {
      FilePath = filePath;
      SaveReport();
    }

    public void Attach<T>(IEnumerable<T> items, string dataSourceName) where T : class
    {
      Report.Analyze(items, dataSourceName);
    }

    #endregion

    #region Commands

    #region Add Band

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

    protected override void SubBands_SubBandAdded(object sender, BandsEventArgs e)
    {
      base.SubBands_SubBandAdded(sender, e);
      IsDirty = true;
    }

    protected override void SubBands_SubBandRemoved(object sender, BandsEventArgs e)
    {
      base.SubBands_SubBandRemoved(sender, e);
      IsDirty = true;
    }

    #endregion

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
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
      }

      base.Dispose(disposing);
    }

    #endregion
  }
}
