using System;
using FlowReports.Model.DataSources.DataSourceItems;

namespace FlowReports.Model.DataSources.Analyzers
{
  internal interface IPropertyAnalyzer
  {
    bool IsSupported(Type type);

    public IDataSourceItem GetItem(Type type, string name = null);
  }
}
