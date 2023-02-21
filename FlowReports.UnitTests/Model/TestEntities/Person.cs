using System;
using System.Collections.Generic;

namespace FlowReports.UnitTests.Model.TestEntities
{
  internal class Person
  {
    public Person(string firstName, string lastName)
    {
      FirstName = firstName;
      LastName = lastName;
    }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Name => LastName + ", " + FirstName;

    public DateTime? DOB { get; set; }

    public List<Person> Children { get; } = new List<Person>();

    public Person Father { get; set; }

    public Person Mother { get; set; }
  }
}
