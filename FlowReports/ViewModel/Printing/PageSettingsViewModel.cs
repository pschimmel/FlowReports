﻿using System.Collections.ObjectModel;
using System.Printing;

namespace FlowReports.ViewModel.Printing
{
  public class PageSettingsViewModel : ViewModelBase
  {
    #region Fields

    private PrintQueue _selectedPrinter;
    private PageMediaSize _selectedPageSize;
    private PageOrientation _orientation;

    #endregion

    #region Constructor

    public PageSettingsViewModel(PageInformation info)
    {
      if (info is null || info.Printer is null)
      {
        return;
      }

      var server = new PrintServer();
      var queues = server.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });
      Printers = queues?.OrderBy(x => x.FullName).ToList() ?? new List<PrintQueue>();
      if (info.Printer != null)
      {
        SelectedPrinter = Printers.FirstOrDefault(x => x.FullName == info.Printer.FullName);
      }

      UpdatePaperSizes();
      if (info.PageSize != null)
      {
        _selectedPageSize = PageSizes.FirstOrDefault(x => x.PageMediaSizeName == info.PageSize.PageMediaSizeName);
      }

      _orientation = info.Orientation == PageOrientation.ReverseLandscape
        ? PageOrientation.Landscape
        : info.Orientation == PageOrientation.ReversePortrait
          ? PageOrientation.ReversePortrait
          : info.Orientation;
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
