﻿using System.Collections;
using FlowReports.Model.DataSources;
using FlowReports.Model.ReportItems;

namespace FlowReports.Model
{
  public class Report
  {
    public ReportBandCollection Bands { get; } = new ReportBandCollection();

    public DataSource DataSource { get; internal set; }

    public DateTime LastChanged { get; internal set; }
    public IEnumerable Data { get; internal set; }
    public Type TypeOfData { get; internal set; }
    public string FilePath { get; set; }

    public void Analyze<T>(IEnumerable<T> items) where T : class
    {
      ReportEngine.Analyze(this, items);
    }
  }
}
