using System.Collections.Generic;
using FlowReports.Model.DataSources;
using FlowReports.UnitTests.Model.TestEntities;

namespace FlowReports.UnitTests.Model
{
  public class DataSourceAnalyzerTests
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void DataSourceAnalyzerTest_ItemIsObject_KnownListType()
    {
      var item = new TestItemWithKnownListType();
      DataSourceAnalyzer.Analyze(new List<TestItemWithKnownListType> { item });
    }

    [Test]
    public void DataSourceAnalyzerTest_ItemIsObject_UnknownListType()
    {
      var item = new TestItemWithUnknownListType();
      DataSourceAnalyzer.Analyze(new List<TestItemWithUnknownListType> { item });
    }

    [Test]
    public void DataSourceAnalyzerTest_ItemIsIEnumerable()
    {
      var list = new TestItemWithKnownListType[] { new TestItemWithKnownListType(), new TestItemWithKnownListType() };
      DataSourceAnalyzer.Analyze(list);
    }

    [Test, Ignore("Infinite loop")]
    public void DataSourceAnalyzerTest_Recursion()
    {
      var hans = new Person("Hans", "Müller");
      var gerda = new Person("Gerda", "Müller");
      var josef = new Person("Josef", "Müller");
      var helmut = new Person("Helmut", "Schmidt");

      hans.Father = josef;
      hans.Mother = gerda;
      josef.Children.Add(hans);
      gerda.Children.Add(hans);

      var list = new Person[] { hans, helmut };
      DataSource dataSource = DataSourceAnalyzer.Analyze(list);
      Assert.That(dataSource, Is.Not.Null);
    }
  }
}