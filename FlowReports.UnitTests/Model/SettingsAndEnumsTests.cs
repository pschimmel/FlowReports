using FlowReports.Model;

namespace FlowReports.UnitTests.Model
{
  public class SettingsTests
  {
    private Settings _settings;

    [SetUp]
    public void Setup()
    {
      _settings = new Settings();
    }

    [Test]
    public void RecursionDepth_DefaultValue_IsRecursionDefault()
    {
      Assert.That(_settings.RecursionDepth, Is.EqualTo(Settings.RECURSION_DEFAULT));
    }

    [Test]
    public void RecursionDepth_CanBeSet_WithinValidRange()
    {
      _settings.RecursionDepth = 5;
      Assert.That(_settings.RecursionDepth, Is.EqualTo(5));
    }

    [Test]
    public void RecursionDepth_CanBeSet_ToMinValidValue()
    {
      _settings.RecursionDepth = 1;
      Assert.That(_settings.RecursionDepth, Is.EqualTo(1));
    }

    [Test]
    public void RecursionDepth_CanBeSet_ToMaxValidValue()
    {
      _settings.RecursionDepth = Settings.RECURSION_MAX_DEPTH - 1;
      Assert.That(_settings.RecursionDepth, Is.EqualTo(Settings.RECURSION_MAX_DEPTH - 1));
    }

    [Test]
    public void RecursionDepth_IgnoresInvalidValue_Zero()
    {
      var originalValue = _settings.RecursionDepth;
      _settings.RecursionDepth = 0;
      Assert.That(_settings.RecursionDepth, Is.EqualTo(originalValue));
    }

    [Test]
    public void RecursionDepth_IgnoresInvalidValue_Negative()
    {
      var originalValue = _settings.RecursionDepth;
      _settings.RecursionDepth = -5;
      Assert.That(_settings.RecursionDepth, Is.EqualTo(originalValue));
    }

    [Test]
    public void RecursionDepth_IgnoresInvalidValue_TooLarge()
    {
      var originalValue = _settings.RecursionDepth;
      _settings.RecursionDepth = Settings.RECURSION_MAX_DEPTH + 1;
      Assert.That(_settings.RecursionDepth, Is.EqualTo(originalValue));
    }

    [Test]
    public void RecursionDepth_IgnoresInvalidValue_EqualsMax()
    {
      var originalValue = _settings.RecursionDepth;
      _settings.RecursionDepth = Settings.RECURSION_MAX_DEPTH;
      Assert.That(_settings.RecursionDepth, Is.EqualTo(originalValue));
    }

    [Test]
    public void RecursionDepth_AcceptsValidValue_AfterInvalidAttempt()
    {
      _settings.RecursionDepth = 999;  // Invalid
      _settings.RecursionDepth = 15;   // Valid
      Assert.That(_settings.RecursionDepth, Is.EqualTo(15));
    }

    [Test]
    public void Default_ReturnsNewSettingsInstance()
    {
      var defaultSettings = Settings.Default;
      Assert.That(defaultSettings, Is.Not.Null);
      Assert.That(defaultSettings, Is.InstanceOf<Settings>());
    }

    [Test]
    public void Default_HasDefaultRecursionDepth()
    {
      var defaultSettings = Settings.Default;
      Assert.That(defaultSettings.RecursionDepth, Is.EqualTo(Settings.RECURSION_DEFAULT));
    }

    [Test]
    public void Constants_AreCorrect()
    {
      Assert.That(Settings.RECURSION_MAX_DEPTH, Is.EqualTo(100));
      Assert.That(Settings.RECURSION_DEFAULT, Is.EqualTo(10));
      Assert.That(Settings.DATASOURCE_OPENING_BRACKET, Is.EqualTo('['));
      Assert.That(Settings.DATASOURCE_CLOSING_BRACKET, Is.EqualTo(']'));
    }

    [Test]
    public void RecursionDepth_MultipleInstances_AreIndependent()
    {
      var settings1 = new Settings();
      var settings2 = new Settings();

      settings1.RecursionDepth = 5;
      settings2.RecursionDepth = 20;

      Assert.That(settings1.RecursionDepth, Is.EqualTo(5));
      Assert.That(settings2.RecursionDepth, Is.EqualTo(20));
    }

    [Test]
    public void RecursionDepth_BoundaryTest_ValidRange()
    {
      for (int i = 1; i < Settings.RECURSION_MAX_DEPTH; i++)
      {
        _settings.RecursionDepth = i;
        Assert.That(_settings.RecursionDepth, Is.EqualTo(i));
      }
    }
  }

  public class EnumTests
  {
    [Test]
    public void InsertLocation_CanBeUsedInSwitch()
    {
      var location = InsertLocation.After;
      var result = location switch
      {
        InsertLocation.Before => "before",
        InsertLocation.After => "after",
        _ => "unknown"
      };

      Assert.That(result, Is.EqualTo("after"));
    }

    [Test]
    public void InsertLocation_ToString_ReturnsMemberName()
    {
      Assert.That(InsertLocation.Before.ToString(), Is.EqualTo("Before"));
      Assert.That(InsertLocation.After.ToString(), Is.EqualTo("After"));
    }

    [Test]
    public void InsertLocation_HasTwoValues()
    {
      var values = Enum.GetValues<InsertLocation>();
      Assert.That(values.Length, Is.EqualTo(2));
    }
  }
}
