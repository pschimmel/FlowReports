using System;
using FlowReports.Model.DataSources.DataSourceItems;

namespace FlowReports.Model.DataSources.Analyzers
{
  internal abstract class PropertyAnalyzer : IPropertyAnalyzer
  {
    public IDataSourceItem GetItem(Type type, string name = null)
    {
      return GetItemInternal(type, name ?? DataSourceAnalyzer.GenerateTypeName(type));
    }

    protected abstract IDataSourceItem GetItemInternal(Type type, string name);

    public abstract bool IsSupported(Type type);
  }
}
