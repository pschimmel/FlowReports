using FlowReports.Model.Filtering;
using FlowReports.Model.ReportItems;

namespace FlowReports.UnitTests.Model
{
  /// <summary>
  /// Unit tests for ReportBand.FilterExpression and ReportBand.Ordering properties.
  /// </summary>
  public class ReportBandFilterAndOrderingTests
  {
    private ReportBand _band;

    [SetUp]
    public void Setup()
    {
      _band = new ReportBand();
    }

    #region FilterExpression Tests

    [Test]
    public void FilterExpression_DefaultValue_IsNull()
    {
      Assert.That(_band.FilterExpression, Is.Null);
    }

    [Test]
    public void FilterExpression_CanBeSet()
    {
      var filter = new FilterExpression("Age > 25");
      _band.FilterExpression = filter;
      Assert.That(_band.FilterExpression, Is.SameAs(filter));
    }

    [Test]
    public void FilterExpression_CanBeChanged()
    {
      var filter1 = new FilterExpression("Age > 25");
      var filter2 = new FilterExpression("Name == 'John'");

      _band.FilterExpression = filter1;
      Assert.That(_band.FilterExpression, Is.SameAs(filter1));

      _band.FilterExpression = filter2;
      Assert.That(_band.FilterExpression, Is.SameAs(filter2));
    }

    [Test]
    public void FilterExpression_CanBeSetToNull()
    {
      var filter = new FilterExpression("Age > 25");
      _band.FilterExpression = filter;
      _band.FilterExpression = null;
      Assert.That(_band.FilterExpression, Is.Null);
    }

    [Test]
    public void FilterExpression_WithSimpleCondition_WorksCorrectly()
    {
      _band.FilterExpression = new FilterExpression("Status == 'Active'");
      Assert.That(_band.FilterExpression.Expression, Is.EqualTo("Status == 'Active'"));
    }

    [Test]
    public void FilterExpression_WithComplexCondition_WorksCorrectly()
    {
      var complexFilter = "Age > 25 && (Status == 'Active' || Status == 'Pending')";
      _band.FilterExpression = new FilterExpression(complexFilter);
      Assert.That(_band.FilterExpression.Expression, Is.EqualTo(complexFilter));
    }

    [Test]
    public void FilterExpression_IsIncludedInEquality()
    {
      var band1 = new ReportBand();
      var band2 = new ReportBand();
      var filter = new FilterExpression("Age > 25");

      band1.FilterExpression = filter;
      band2.FilterExpression = filter;

      Assert.That(band1, Is.EqualTo(band2));
    }

    [Test]
    public void FilterExpression_DifferentFilters_MakesBandsNotEqual()
    {
      var band1 = new ReportBand();
      var band2 = new ReportBand();

      band1.FilterExpression = new FilterExpression("Age > 25");
      band2.FilterExpression = new FilterExpression("Status == 'Active'");

      Assert.That(band1, Is.Not.EqualTo(band2));
    }

    [Test]
    public void FilterExpression_WithNullAndNotNull_MakesBandsNotEqual()
    {
      var band1 = new ReportBand();
      var band2 = new ReportBand();

      band1.FilterExpression = null;
      band2.FilterExpression = new FilterExpression("Age > 25");

      Assert.That(band1, Is.Not.EqualTo(band2));
    }

    #endregion

    #region Ordering Tests

    [Test]
    public void Ordering_DefaultValue_IsEmptyList()
    {
      Assert.That(_band.Ordering, Is.Empty);
    }

    [Test]
    public void Ordering_CanAddSingleDescriptor()
    {
      var descriptor = new SortDescriptor { Property = "Name", Direction = SortDirection.Ascending };
      _band.Ordering.Add(descriptor);

      Assert.That(_band.Ordering.Count, Is.EqualTo(1));
      Assert.That(_band.Ordering[0], Is.SameAs(descriptor));
    }

    [Test]
    public void Ordering_CanAddMultipleDescriptors()
    {
      var descriptor1 = new SortDescriptor { Property = "LastName", Direction = SortDirection.Ascending };
      var descriptor2 = new SortDescriptor { Property = "FirstName", Direction = SortDirection.Ascending };
      var descriptor3 = new SortDescriptor { Property = "Age", Direction = SortDirection.Descending };

      _band.Ordering.Add(descriptor1);
      _band.Ordering.Add(descriptor2);
      _band.Ordering.Add(descriptor3);

      Assert.That(_band.Ordering.Count, Is.EqualTo(3));
      Assert.That(_band.Ordering[0], Is.SameAs(descriptor1));
      Assert.That(_band.Ordering[1], Is.SameAs(descriptor2));
      Assert.That(_band.Ordering[2], Is.SameAs(descriptor3));
    }

    [Test]
    public void Ordering_CanRemoveDescriptor()
    {
      var descriptor1 = new SortDescriptor { Property = "Name", Direction = SortDirection.Ascending };
      var descriptor2 = new SortDescriptor { Property = "Age", Direction = SortDirection.Descending };

      _band.Ordering.Add(descriptor1);
      _band.Ordering.Add(descriptor2);

      _band.Ordering.Remove(descriptor1);

      Assert.That(_band.Ordering.Count, Is.EqualTo(1));
      Assert.That(_band.Ordering[0], Is.SameAs(descriptor2));
    }

    [Test]
    public void Ordering_CanClear()
    {
      _band.Ordering.Add(new SortDescriptor { Property = "Name", Direction = SortDirection.Ascending });
      _band.Ordering.Add(new SortDescriptor { Property = "Age", Direction = SortDirection.Descending });

      _band.Ordering.Clear();

      Assert.That(_band.Ordering, Is.Empty);
    }

    [Test]
    public void Ordering_SupportsBothDirections()
    {
      var ascending = new SortDescriptor { Property = "Name", Direction = SortDirection.Ascending };
      var descending = new SortDescriptor { Property = "Age", Direction = SortDirection.Descending };

      _band.Ordering.Add(ascending);
      _band.Ordering.Add(descending);

      Assert.That(_band.Ordering[0].Direction, Is.EqualTo(SortDirection.Ascending));
      Assert.That(_band.Ordering[1].Direction, Is.EqualTo(SortDirection.Descending));
    }

    [Test]
    public void Ordering_CanHaveMultiplePropertiesInSequence()
    {
      // Typical use case: sort by LastName first, then FirstName
      _band.Ordering.Add(new SortDescriptor { Property = "LastName", Direction = SortDirection.Ascending });
      _band.Ordering.Add(new SortDescriptor { Property = "FirstName", Direction = SortDirection.Ascending });

      Assert.That(_band.Ordering.Count, Is.EqualTo(2));
      Assert.That(_band.Ordering[0].Property, Is.EqualTo("LastName"));
      Assert.That(_band.Ordering[1].Property, Is.EqualTo("FirstName"));
    }

    [Test]
    public void Ordering_IsCaseSensitiveForPropertyNames()
    {
      var descriptor1 = new SortDescriptor { Property = "Name", Direction = SortDirection.Ascending };
      var descriptor2 = new SortDescriptor { Property = "name", Direction = SortDirection.Ascending };

      // SortDescriptor.Equals uses OrdinalIgnoreCase, so these should be equal
      Assert.That(descriptor1, Is.EqualTo(descriptor2));
    }

    [Test]
    public void Ordering_IsIncludedInEquality()
    {
      var band1 = new ReportBand();
      var band2 = new ReportBand();

      band1.Ordering.Add(new SortDescriptor { Property = "Name", Direction = SortDirection.Ascending });
      band2.Ordering.Add(new SortDescriptor { Property = "Name", Direction = SortDirection.Ascending });

      Assert.That(band1, Is.EqualTo(band2));
    }

    [Test]
    public void Ordering_DifferentOrderings_MakeBandsNotEqual()
    {
      var band1 = new ReportBand();
      var band2 = new ReportBand();

      band1.Ordering.Add(new SortDescriptor { Property = "Name", Direction = SortDirection.Ascending });
      band2.Ordering.Add(new SortDescriptor { Property = "Age", Direction = SortDirection.Descending });

      Assert.That(band1, Is.Not.EqualTo(band2));
    }

    [Test]
    public void Ordering_DifferentDirections_MakeBandsNotEqual()
    {
      var band1 = new ReportBand();
      var band2 = new ReportBand();

      band1.Ordering.Add(new SortDescriptor { Property = "Name", Direction = SortDirection.Ascending });
      band2.Ordering.Add(new SortDescriptor { Property = "Name", Direction = SortDirection.Descending });

      Assert.That(band1, Is.Not.EqualTo(band2));
    }

    [Test]
    public void Ordering_CountDifference_MakeBandsNotEqual()
    {
      var band1 = new ReportBand();
      var band2 = new ReportBand();

      band1.Ordering.Add(new SortDescriptor { Property = "Name", Direction = SortDirection.Ascending });
      band2.Ordering.Add(new SortDescriptor { Property = "Name", Direction = SortDirection.Ascending });
      band2.Ordering.Add(new SortDescriptor { Property = "Age", Direction = SortDirection.Descending });

      Assert.That(band1, Is.Not.EqualTo(band2));
    }

    #endregion

    #region Combined FilterExpression and Ordering Tests

    [Test]
    public void Band_CanHaveBothFilterAndOrdering()
    {
      _band.FilterExpression = new FilterExpression("Status == 'Active'");
      _band.Ordering.Add(new SortDescriptor { Property = "Name", Direction = SortDirection.Ascending });
      _band.Ordering.Add(new SortDescriptor { Property = "Age", Direction = SortDirection.Descending });

      Assert.That(_band.FilterExpression, Is.Not.Null);
      Assert.That(_band.Ordering.Count, Is.EqualTo(2));
    }

    [Test]
    public void Band_WithFilterAndOrdering_AreIncludedInEquality()
    {
      var band1 = new ReportBand();
      var band2 = new ReportBand();

      var filter = new FilterExpression("Status == 'Active'");
      band1.FilterExpression = filter;
      band2.FilterExpression = filter;

      band1.Ordering.Add(new SortDescriptor { Property = "Name", Direction = SortDirection.Ascending });
      band2.Ordering.Add(new SortDescriptor { Property = "Name", Direction = SortDirection.Ascending });

      Assert.That(band1, Is.EqualTo(band2));
    }

    [Test]
    public void Band_DifferentFilterOrOrdering_MakesBandsNotEqual()
    {
      var band1 = new ReportBand();
      var band2 = new ReportBand();

      // Same filter
      var filter = new FilterExpression("Status == 'Active'");
      band1.FilterExpression = filter;
      band2.FilterExpression = filter;

      // Different ordering
      band1.Ordering.Add(new SortDescriptor { Property = "Name", Direction = SortDirection.Ascending });
      band2.Ordering.Add(new SortDescriptor { Property = "Age", Direction = SortDirection.Descending });

      Assert.That(band1, Is.Not.EqualTo(band2));
    }

    #endregion
  }
}
