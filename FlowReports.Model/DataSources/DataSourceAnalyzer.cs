using System.Collections;
using FlowReports.Model.DataSources.Analyzers;
using FlowReports.Model.DataSources.DataSourceItems;

namespace FlowReports.Model.DataSources
{
  internal static class DataSourceAnalyzer
  {
    private static readonly Lazy<IEnumerable<IPropertyAnalyzer>> _analyzers = new(() =>
    {
      return new IPropertyAnalyzer[]
      {
        new TextFieldAnalyzer(),
        new NumberFieldAnalyzer(),
        new DateFieldAnalyzer(),
        new BooleanFieldAnalyzer(),
      };
    });

    public static DataSource Analyze<T>(IEnumerable<T> source) where T : class
    {
      if (source == null)
      {
        throw new ArgumentNullException(nameof(source));
      }

      var dataSource = new DataSource() { Name = GenerateTypeName(typeof(T)) };
      AnalyzeList(source, dataSource);
      return dataSource;
    }

    private static void AnalyzeItemByInstance(object source, IDataSourceItemContainer container)
    {
      var type = source.GetType();
      AnalyzeItemByType(type, container);
    }

    private static bool AnalyzeItemByType(Type type, IDataSourceItemContainer container)
    {
      var propertyInfos = type.GetProperties();
      bool result = true;

      foreach (var propertyInfo in propertyInfos)
      {
        if (propertyInfo.CanRead)
        {
          // Check for string before checking for IEnumerable, because string is an IEnumerable
          if (AnalyzeProperty(propertyInfo.PropertyType, container, propertyInfo.Name))
          {
            continue;
          }
          else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) && AnalyzeList(propertyInfo.PropertyType, container, propertyInfo.Name))
          {
            continue;
          }
          else
          {
            // For complex types add sub items
            Type childType = propertyInfo.PropertyType;
            var objectField = new ObjectField { Type = childType, Name = propertyInfo.Name };
            container.Add(objectField);
            if (AnalyzeItemByType(childType, objectField))
            {
              continue;
            }
          }

          result = false;
        }
      }

      return result;
    }

    private static bool AnalyzeList<T>(IEnumerable<T> list, IDataSourceItemContainer container, string name = null)
    {
      if (list.Any())
      {
        T item = list.First();
        AnalyzeItemByInstance(item, container);
      }
      else
      {
        var type = list.GetType();// typeof(T);
        return AnalyzeList(type, container, name);
      }

      return false;
    }

    private static bool AnalyzeList(Type type, IDataSourceItemContainer container, string name = null)
    {
      if (!typeof(IEnumerable).IsAssignableFrom(type))
      {
        throw new ArgumentException("Type must be IEnumerable");
      }

      foreach (Type interfaceType in type.GetInterfaces())
      {
        if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
          Type itemType = type.GetGenericArguments()[0];

          var newList = new DataSourceItemList
          {
            Name = name ?? GenerateTypeName(itemType)
          };

          container.Add(newList);

          return AnalyzeItemByType(itemType, newList);
        }
      }

      return false;
    }

    private static bool AnalyzeProperty(Type type, IDataSourceItemContainer container, string name = null)
    {
      // If it is a basic type, we add it to the container.
      foreach (var analyzer in _analyzers.Value)
      {
        if (analyzer.IsSupported(type))
        {
          container.Add(analyzer.GetItem(type, name));
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Generates a name from a type. Set pluralize to true to get name in plural.
    /// </summary>
    internal static string GenerateTypeName(Type type)
    {
      Type underlyingType = Nullable.GetUnderlyingType(type);
      return underlyingType?.Name ?? type.Name;
    }
  }
}
