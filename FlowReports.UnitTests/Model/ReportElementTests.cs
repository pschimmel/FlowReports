using FlowReports.Model.ReportItems;

namespace FlowReports.UnitTests.Model
{
  public class ReportElementTests
  {
    [Test]
    public void Constructor_DefaultValue_AssignsNewGuid()
    {
      var item = new TextItem();
      Assert.That(item.ID, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Constructor_WithGuid_UsesProvidedGuid()
    {
      var expectedId = Guid.NewGuid();
      var item = new TextItem(expectedId);
      Assert.That(item.ID, Is.EqualTo(expectedId));
    }

    [Test]
    public void ID_IsUnique_ForDifferentInstances()
    {
      var item1 = new TextItem();
      var item2 = new TextItem();
      Assert.That(item1.ID, Is.Not.EqualTo(item2.ID));
    }

    [Test]
    public void ID_CanBeRetrieved()
    {
      var item = new TextItem();
      var id = item.ID;
      Assert.That(id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void MultipleElements_HaveDifferentIds()
    {
      var textItem = new TextItem();
      var boolItem = new BooleanItem();
      var imageItem = new ImageItem();

      var ids = new[] { textItem.ID, boolItem.ID, imageItem.ID };
      Assert.That(ids.Distinct().Count(), Is.EqualTo(3));
    }

    [Test]
    public void ReportBand_HasUniqueId()
    {
      var band1 = new ReportBand();
      var band2 = new ReportBand();
      Assert.That(band1.ID, Is.Not.EqualTo(band2.ID));
    }

    [Test]
    public void HeaderBand_HasUniqueId()
    {
      var header1 = new HeaderBand();
      var header2 = new HeaderBand();
      Assert.That(header1.ID, Is.Not.EqualTo(header2.ID));
    }

    [Test]
    public void FooterBand_HasUniqueId()
    {
      var footer1 = new FooterBand();
      var footer2 = new FooterBand();
      Assert.That(footer1.ID, Is.Not.EqualTo(footer2.ID));
    }
  }
}
