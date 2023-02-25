using System.ComponentModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using FlowReports.Model;
using FlowReports.Model.Events;
using FlowReports.Model.ImportExport;

namespace FlowReports.UI.ViewModel
{
  public class ReportViewModel : BandContainerViewModel
  {
    #region Fields

    private readonly Report _report;
    private ReportBandViewModel _selectedBand;
    private IItemViewModel _selectedItem;
    private bool _isDirty;
    private readonly ActionCommand _addNewBandCommand;
    private readonly ActionCommand _addSubBandCommand;
    private readonly ActionCommand _editBandDetailsCommand;
    private readonly ActionCommand _removeBandCommand;
    private readonly ActionCommand _addTextItemCommand;
    private readonly ActionCommand _removeItemCommand;
    private readonly Lazy<IEnumerable<DataSourceViewModel>> _lazyDataSource;

    #endregion

    #region Constructor

    public ReportViewModel(Report report)
      : base(report.Bands)
    {
      _report = report;
      SelectionChanged += ReportVM_SelectionChanged;

      _addNewBandCommand = new ActionCommand(AddNewBand, CanAddNewBand);
      _addSubBandCommand = new ActionCommand(AddSubBand, CanAddSubBand);
      _editBandDetailsCommand = new ActionCommand(EditBandDetails, CanEditBandDetails);
      _removeBandCommand = new ActionCommand(RemoveBand, CanRemoveBand);
      _addTextItemCommand = new ActionCommand(AddTextItem, CanAddTextItem);
      _removeItemCommand = new ActionCommand(RemoveItem, CanRemoveItem);
      _lazyDataSource = new Lazy<IEnumerable<DataSourceViewModel>>(() => new DataSourceViewModel[] { new DataSourceViewModel(report.DataSource) });
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
          OnPropertyChanged();
          _addSubBandCommand.RaiseCanExecuteChanged();
          _removeBandCommand.RaiseCanExecuteChanged();
          _addTextItemCommand.RaiseCanExecuteChanged();

          if (_selectedBand != null)
          {
            _selectedBand.ItemSelected += SelectedBand_ItemSelected;
          }
        }
      }
    }

    public IItemViewModel SelectedItem
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

    public string FilePath { get; set; }

    public IEnumerable<DataSourceViewModel> DataSourceVM => _lazyDataSource.Value;

    internal Report Report => _report;

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

      ReportWriter.Write(_report, FilePath);
      IsDirty = false;
    }

    public void SaveReport(string filePath)
    {
      FilePath = filePath;
      SaveReport();
    }

    public void Attach<T>(IEnumerable<T> items) where T : class
    {
      _report.Analyze(items);
    }

    #endregion

    #region Add Band

    public ICommand AddBandCommand => _addNewBandCommand;

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

    public ICommand AddSubBandCommand => _addSubBandCommand;

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

    public ICommand EditBandDetailsCommand => _editBandDetailsCommand;

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

    public ICommand RemoveBandCommand => _removeBandCommand;

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

    #region Add Text Item

    public ICommand AddTextItemCommand => _addTextItemCommand;

    private void AddTextItem()
    {
      var newItem = SelectedBand?.AddTextItem();
      SelectedItem = newItem;
    }

    private bool CanAddTextItem()
    {
      return SelectedBand != null;
    }

    #endregion

    #region Remove Text Item

    public ICommand RemoveItemCommand => _removeItemCommand;

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

    #region Event Handlers

    private void SelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      IsDirty = true;
    }

    private void SelectedBand_ItemSelected(object sender, EventArgs e)
    {
      if (sender is IItemViewModel itemViewModel)
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
