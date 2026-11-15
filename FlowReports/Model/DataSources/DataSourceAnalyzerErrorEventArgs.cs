namespace FlowReports.Model.DataSources
{
  /// <summary>
  /// Event args for errors raised by the DataSourceAnalyzer.
  /// </summary>
  public sealed class DataSourceAnalyzerErrorEventArgs : EventArgs
  {
    public string Message { get; }
    public Exception Exception { get; }

    public DataSourceAnalyzerErrorEventArgs(string message, Exception exception = null)
    {
      Message = message;
      Exception = exception;
    }
  }
}
