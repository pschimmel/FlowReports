using System.Collections.ObjectModel;
using System.Printing;
using FlowReports.ViewModel;

namespace NAS.ViewModel.Printing
{
  public class PageSettingsViewModel : ViewModelBase
  {
    #region Fields

    private PrintQueue _selectedPrinter;
    private PageMediaSize _selectedPageSize;
    private PageOrientation _orientation;

    #endregion

    #region Constructor

    public PageSettingsViewModel(PrintQueue printer, PageMediaSize pageSize, PageOrientation orientation, double zoom)
    {
      var server = new PrintServer();
      var queues = server.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });
      Printers = queues?.OrderBy(x => x.FullName).ToList() ?? new List<PrintQueue>();
      if (printer != null)
      {
        SelectedPrinter = Printers.FirstOrDefault(x => x.FullName == printer.FullName);
      }

      UpdatePaperSizes();
      if (pageSize != null)
      {
        _selectedPageSize = PageSizes.FirstOrDefault(x => x.PageMediaSizeName == pageSize.PageMediaSizeName);
      }

      if (orientation == PageOrientation.ReverseLandscape)
      {
        orientation = PageOrientation.Landscape;
      }

      if (orientation == PageOrientation.ReversePortrait)
      {
        orientation = PageOrientation.ReversePortrait;
      }

      _orientation = orientation;

      Zoom = Convert.ToInt32(zoom * 100);
    }

    #endregion

    #region Properties

    public List<PrintQueue> Printers { get; }

    public PrintQueue SelectedPrinter
    {
      get => _selectedPrinter;
      set
      {
        if (_selectedPrinter != value)
        {
          _selectedPrinter = value;
          OnPropertyChanged(nameof(SelectedPrinter));
          UpdatePaperSizes();
        }
      }
    }

    public ObservableCollection<PageMediaSize> PageSizes { get; } = new ObservableCollection<PageMediaSize>();

    public PageMediaSize SelectedPageSize
    {
      get => _selectedPageSize;
      set
      {
        if (_selectedPageSize != value)
        {
          _selectedPageSize = value;
          OnPropertyChanged(nameof(_selectedPageSize));
          UpdateDimensions();
        }
      }
    }

    public PageOrientation Orientation
    {
      get => _orientation;
      set
      {
        if (_orientation != value)
        {
          _orientation = value;
          OnPropertyChanged(nameof(Orientation));
          UpdateDimensions();
        }
      }
    }

    public double PageWidth { get; set; }

    public double PageHeight { get; set; }

    public int Zoom { get; set; }

    #endregion

    #region Private Methods

    private void UpdatePaperSizes()
    {
      string oldSelectedPageSize = SelectedPageSize?.PageMediaSizeName?.ToString();
      PageSizes.Clear();
      if (SelectedPrinter != null)
      {
        var capabilities = SelectedPrinter.GetPrintCapabilities();
        capabilities?.PageMediaSizeCapability.OrderBy(x => x.PageMediaSizeName).ToList().ForEach(x => PageSizes.Add(x));

        if (!string.IsNullOrWhiteSpace(oldSelectedPageSize))
        {
          SelectedPageSize = PageSizes.FirstOrDefault(x => x.PageMediaSizeName.ToString() == oldSelectedPageSize);
        }

        SelectedPageSize ??= PageSizes.FirstOrDefault(x => x.PageMediaSizeName == SelectedPrinter.DefaultPrintTicket.PageMediaSize.PageMediaSizeName);
      }
    }

    private void UpdateDimensions()
    {
      if (SelectedPageSize != null)
      {
        if (Orientation == PageOrientation.Landscape || Orientation == PageOrientation.ReverseLandscape)
        {
          PageWidth = Math.Round(SelectedPageSize.Height.Value / 96 * 2.54, 1);
          PageHeight = Math.Round(SelectedPageSize.Width.Value / 96 * 2.54, 1);
        }
        else
        {
          PageWidth = Math.Round(SelectedPageSize.Width.Value / 96 * 2.54, 1);
          PageWidth = Math.Round(SelectedPageSize.Height.Value / 96 * 2.54, 1);
        }
      }
    }

    #endregion
  }
}
