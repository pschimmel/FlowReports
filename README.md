# FlowReports

Create printable reports based on any data.

## Code Examples

### Show Report

How to show the report print preview:

```csharp
// Create data
var hans = new Person("Hans", "M�ller");
var gerda = new Person("Gerda", "M�ller");
var josef = new Person("Josef", "M�ller");
var helmut = new Person("Helmut", "Schmidt");
hans.Father = josef;
hans.Mother = gerda;
josef.Children.Add(hans);
gerda.Children.Add(hans);

var list = new Person[] { hans, helmut };

// Load report file
var report =  FlowReport.Load("Personlist.flow");
// Show report
FlowReport.Show(report, list);
```

### Edit Report

Open the **FlowReports** report editor and edit your report defintion.

```csharp
// Create data
var hans = new Person("Hans", "M�ller");
var gerda = new Person("Gerda", "M�ller");
var josef = new Person("Josef", "M�ller");
var helmut = new Person("Helmut", "Schmidt");
hans.Father = josef;
hans.Mother = gerda;
josef.Children.Add(hans);
gerda.Children.Add(hans);

var list = new Person[] { hans, helmut };

// Load report file
var report =  FlowReport.Load("Personlist.flow");
// Show report
FlowReport.Edit(report, list);
```