using System;
using FlowReports.Model.DataSources.DataSourceItems;

namespace FlowReports.Model.DataSources.Analyzers
{
  internal class DateFieldAnalyzer : PropertyAnalyzer
  {
    public override bool IsSupported(Type type)
    {
      return typeof(DateTime).IsAssignableFrom(type) || typeof(DateTime?).IsAssignableFrom(type);
    }

    protected override IDataSourceItem GetItemInternal(Type type, string name)
    {
      return new DateField { Name = name, Type = type };
    }
  }
}
