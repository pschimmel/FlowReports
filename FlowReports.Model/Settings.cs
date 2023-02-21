namespace FlowReports.Model
{
  public class Settings
  {
    public const int RECURSION_MAX_DEPTH = 10;
    public const int RECURSION_DEFAULT = 5;
    private int _recursionDepth = RECURSION_DEFAULT;

    public int RecursionDepth
    {
      get { return _recursionDepth; }
      set
      {
        if (value > 0 && value < RECURSION_MAX_DEPTH)
        {
          _recursionDepth = value;
        }
      }
    }

    public static Settings Default => new();
  }
}
