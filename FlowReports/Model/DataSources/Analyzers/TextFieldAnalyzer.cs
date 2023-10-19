using FlowReports.Model.DataSources.DataSourceItems;

namespace FlowReports.Model.DataSources.Analyzers
{
  internal class TextFieldAnalyzer : PropertyAnalyzer
  {
    public override bool IsSupported(Type type)
    {
      return typeof(string).IsAssignableFrom(type);
    }

    protected override IDataSourceItem GetItemInternal(Type type, string name)
    {
      return new TextField { Name = name, Type = type };
    }
  }
}
