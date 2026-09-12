namespace FlowReports.ViewModel.Printing
{
  /// <summary>
  /// Event args for error events raised by the paginator.
  /// </summary>
  public sealed class ReportPaginatorErrorEventArgs : EventArgs
  {
    public ReportPaginatorErrorEventArgs(string message, Exception exception = null)
    {
      Message = message;
      Exception = exception;
    }

    public string Message { get; }

    public Exception Exception { get; }
  }
}
