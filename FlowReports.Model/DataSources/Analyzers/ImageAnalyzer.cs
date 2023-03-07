using FlowReports.Model.DataSources.DataSourceItems;

namespace FlowReports.Model.DataSources.Analyzers
{
  internal class ImageAnalyzer : PropertyAnalyzer
  {
    public override bool IsSupported(Type type)
    {
      return typeof(byte[]).IsAssignableFrom(type);
    }

    protected override IDataSourceItem GetItemInternal(Type type, string name)
    {
      return new ImageField { Name = name, Type = type };
    }
  }
}
