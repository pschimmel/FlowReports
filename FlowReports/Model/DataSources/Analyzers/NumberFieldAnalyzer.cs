using System;
using FlowReports.Model.DataSources.DataSourceItems;

namespace FlowReports.Model.DataSources.Analyzers
{
  internal class NumberFieldAnalyzer : PropertyAnalyzer
  {
    public override bool IsSupported(Type type)
    {
      return typeof(int).IsAssignableFrom(type) 
        || typeof(double).IsAssignableFrom(type) 
        || typeof(decimal).IsAssignableFrom(type)
        || typeof(long).IsAssignableFrom(type)
        || typeof(float).IsAssignableFrom(type)
        || typeof(short).IsAssignableFrom(type)
        || typeof(int?).IsAssignableFrom(type) 
        || typeof(double?).IsAssignableFrom(type)
        || typeof(decimal?).IsAssignableFrom(type)
        || typeof(long?).IsAssignableFrom(type)
        || typeof(float?).IsAssignableFrom(type)
        || typeof(short?).IsAssignableFrom(type);
    }

    protected override IDataSourceItem GetItemInternal(Type type, string name)
    {
      return new NumberField { Name = name, Type = type };
    }
  }
}
