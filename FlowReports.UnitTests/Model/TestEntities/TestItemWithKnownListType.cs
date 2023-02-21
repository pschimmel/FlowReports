using System.Collections.Generic;

namespace FlowReports.UnitTests.Model.TestEntities
{
  internal class TestItemWithKnownListType
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Element> Items { get; } = new List<Element>();
  }
}