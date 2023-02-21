using FlowReports.Model.DataSources.DataSourceItems;

namespace FlowReports.Model.DataSources.Analyzers
{
  internal class ObjectFieldAnalyzer : PropertyAnalyzer
  {
    public override bool IsSupported(Type type)
    {
      return true;
    }

    protected override IDataSourceItem GetItemInternal(Type type, string name)
    {
      return new ObjectField { Name = name, Type = type };
    }
  }
}
