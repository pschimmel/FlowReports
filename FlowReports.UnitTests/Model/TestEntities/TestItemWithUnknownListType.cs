using System.Collections;
using System.Collections.Generic;

namespace FlowReports.UnitTests.Model.TestEntities
{
  internal class TestItemWithUnknownListType
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable Elements { get; } = new List<Element>();
  }
}