using FlowReports.Model.Filtering;

namespace FlowReports.UnitTests.Model.Filtering
{
  public class FilterExpressionTests
  {
    private class TestItem
    {
      public string Name { get; set; }
      public int Age { get; set; }
      public string Status { get; set; }
      public bool IsActive { get; set; }
      public double Salary { get; set; }
    }

    [Test]
    public void Evaluate_EmptyExpression_ReturnsTrue()
    {
      var filter = new FilterExpression();
      var item = new TestItem { Name = "John" };

      Assert.That(filter.Evaluate(item), Is.True);
    }

    [Test]
    public void Evaluate_NullExpression_ReturnsTrue()
    {
      var filter = new FilterExpression(null);
      var item = new TestItem { Name = "John" };

      Assert.That(filter.Evaluate(item), Is.True);
    }

    [Test]
    public void Evaluate_NullItem_ReturnsTrue()
    {
      var filter = new FilterExpression("Name == 'John'");

      Assert.That(filter.Evaluate(null), Is.True);
    }

    [Test]
    public void Evaluate_StringEquality_Success()
    {
      var filter = new FilterExpression("Name == 'John'");
      var itemMatch = new TestItem { Name = "John" };
      var itemNoMatch = new TestItem { Name = "Jane" };

      Assert.That(filter.Evaluate(itemMatch), Is.True);
      Assert.That(filter.Evaluate(itemNoMatch), Is.False);
    }

    [Test]
    public void Evaluate_StringEqualityWithDoubleQuotes_Success()
    {
      var filter = new FilterExpression("Name == \"John\"");
      var itemMatch = new TestItem { Name = "John" };
      var itemNoMatch = new TestItem { Name = "Jane" };

      Assert.That(filter.Evaluate(itemMatch), Is.True);
      Assert.That(filter.Evaluate(itemNoMatch), Is.False);
    }

    [Test]
    public void Evaluate_StringInequality_Success()
    {
      var filter = new FilterExpression("Name != 'John'");
      var itemMatch = new TestItem { Name = "Jane" };
      var itemNoMatch = new TestItem { Name = "John" };

      Assert.That(filter.Evaluate(itemMatch), Is.True);
      Assert.That(filter.Evaluate(itemNoMatch), Is.False);
    }

    [Test]
    public void Evaluate_NumericComparison_GreaterThan()
    {
      var filter = new FilterExpression("Age > 30");
      var itemMatch = new TestItem { Age = 35 };
      var itemNoMatch = new TestItem { Age = 25 };

      Assert.That(filter.Evaluate(itemMatch), Is.True);
      Assert.That(filter.Evaluate(itemNoMatch), Is.False);
    }

    [Test]
    public void Evaluate_NumericComparison_LessThan()
    {
      var filter = new FilterExpression("Age < 30");
      var itemMatch = new TestItem { Age = 25 };
      var itemNoMatch = new TestItem { Age = 35 };

      Assert.That(filter.Evaluate(itemMatch), Is.True);
      Assert.That(filter.Evaluate(itemNoMatch), Is.False);
    }

    [Test]
    public void Evaluate_NumericComparison_GreaterThanOrEqual()
    {
      var filter = new FilterExpression("Age >= 30");
      var itemMatch1 = new TestItem { Age = 35 };
      var itemMatch2 = new TestItem { Age = 30 };
      var itemNoMatch = new TestItem { Age = 25 };

      Assert.That(filter.Evaluate(itemMatch1), Is.True);
      Assert.That(filter.Evaluate(itemMatch2), Is.True);
      Assert.That(filter.Evaluate(itemNoMatch), Is.False);
    }

    [Test]
    public void Evaluate_NumericComparison_LessThanOrEqual()
    {
      var filter = new FilterExpression("Age <= 30");
      var itemMatch1 = new TestItem { Age = 25 };
      var itemMatch2 = new TestItem { Age = 30 };
      var itemNoMatch = new TestItem { Age = 35 };

      Assert.That(filter.Evaluate(itemMatch1), Is.True);
      Assert.That(filter.Evaluate(itemMatch2), Is.True);
      Assert.That(filter.Evaluate(itemNoMatch), Is.False);
    }

    [Test]
    public void Evaluate_BooleanProperty_True()
    {
      var filter = new FilterExpression("IsActive == true");
      var itemMatch = new TestItem { IsActive = true };
      var itemNoMatch = new TestItem { IsActive = false };

      Assert.That(filter.Evaluate(itemMatch), Is.True);
      Assert.That(filter.Evaluate(itemNoMatch), Is.False);
    }

    [Test]
    public void Evaluate_BooleanProperty_False()
    {
      var filter = new FilterExpression("IsActive == false");
      var itemMatch = new TestItem { IsActive = false };
      var itemNoMatch = new TestItem { IsActive = true };

      Assert.That(filter.Evaluate(itemMatch), Is.True);
      Assert.That(filter.Evaluate(itemNoMatch), Is.False);
    }

    [Test]
    public void Evaluate_AndExpression_BothTrue()
    {
      var filter = new FilterExpression("Age > 30 && Status == 'Active'");
      var itemMatch = new TestItem { Age = 35, Status = "Active" };
      var itemNoMatch1 = new TestItem { Age = 25, Status = "Active" };
      var itemNoMatch2 = new TestItem { Age = 35, Status = "Inactive" };

      Assert.That(filter.Evaluate(itemMatch), Is.True);
      Assert.That(filter.Evaluate(itemNoMatch1), Is.False);
      Assert.That(filter.Evaluate(itemNoMatch2), Is.False);
    }

    [Test]
    public void Evaluate_OrExpression_AtLeastOneTrue()
    {
      var filter = new FilterExpression("Age > 30 || Status == 'VIP'");
      var itemMatch1 = new TestItem { Age = 35, Status = "Regular" };
      var itemMatch2 = new TestItem { Age = 25, Status = "VIP" };
      var itemMatch3 = new TestItem { Age = 35, Status = "VIP" };
      var itemNoMatch = new TestItem { Age = 25, Status = "Regular" };

      Assert.That(filter.Evaluate(itemMatch1), Is.True);
      Assert.That(filter.Evaluate(itemMatch2), Is.True);
      Assert.That(filter.Evaluate(itemMatch3), Is.True);
      Assert.That(filter.Evaluate(itemNoMatch), Is.False);
    }

    [Test]
    public void Evaluate_NotExpression()
    {
      var filter = new FilterExpression("!(Age > 30)");
      var itemMatch = new TestItem { Age = 25 };
      var itemNoMatch = new TestItem { Age = 35 };

      Assert.That(filter.Evaluate(itemMatch), Is.True);
      Assert.That(filter.Evaluate(itemNoMatch), Is.False);
    }

    [Test]
    public void Evaluate_ComplexExpression()
    {
      var filter = new FilterExpression("(Age > 25 && Status == 'Active') || IsActive == true");
      var itemMatch1 = new TestItem { Age = 30, Status = "Active", IsActive = false };
      var itemMatch2 = new TestItem { Age = 20, Status = "Inactive", IsActive = true };
      var itemNoMatch = new TestItem { Age = 20, Status = "Inactive", IsActive = false };

      Assert.That(filter.Evaluate(itemMatch1), Is.True);
      Assert.That(filter.Evaluate(itemMatch2), Is.True);
      Assert.That(filter.Evaluate(itemNoMatch), Is.False);
    }

    [Test]
    public void Evaluate_DoubleComparison()
    {
      var filter = new FilterExpression("Salary >= 50000.0");
      var itemMatch = new TestItem { Salary = 60000.0 };
      var itemNoMatch = new TestItem { Salary = 40000.0 };

      Assert.That(filter.Evaluate(itemMatch), Is.True);
      Assert.That(filter.Evaluate(itemNoMatch), Is.False);
    }

    [Test]
    public void Evaluate_ExpressionCanBeUpdated()
    {
      var filter = new FilterExpression("Age > 30");
      var item = new TestItem { Age = 35 };
      Assert.That(filter.Evaluate(item), Is.True);

      filter.Expression = "Age < 30";
      Assert.That(filter.Evaluate(item), Is.False);
    }

    [Test]
    public void FilterExpression_Equality()
    {
      var filter1 = new FilterExpression("Age > 30");
      var filter2 = new FilterExpression("Age > 30");
      var filter3 = new FilterExpression("Age < 30");

      Assert.That(filter1, Is.EqualTo(filter2));
      Assert.That(filter1, Is.Not.EqualTo(filter3));
    }

    [Test]
    public void FilterExpression_ToString()
    {
      var filter = new FilterExpression("Age > 30");
      Assert.That(filter.ToString(), Is.EqualTo("Age > 30"));
    }
  }
}
