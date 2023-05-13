namespace FlowReports.ViewModel.ReportItems
{
  public interface IItemViewModel
  {
    double Left { get; }
    double Top { get; }
    double Width { get; }
    double Height { get; }
  }
}