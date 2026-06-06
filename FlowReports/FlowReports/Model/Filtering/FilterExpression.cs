using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FlowReports.Model.Filtering
{
  /// <summary>
  /// Represents a filter expression that can be applied to data items to determine if they should be rendered.
  /// </summary>
  public partial class FilterExpression
  {
    private Func<object, bool> _compiledExpression;
    private string _expression;

    /// <summary>
    /// Gets or sets the filter expression string.
    /// The expression should be in a format like: "PropertyName == 'value'" or "Age > 18 && Status == 'Active'"
    /// Supported operators: ==, !=, >, <, >=, <=, &&, ||, !, Contains (for strings)
    /// </summary>
    public string Expression
    {
      get => _expression;
      set
      {
        if (_expression != value)
        {
          _expression = value;
          _compiledExpression = null;
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of the FilterExpression class.
    /// </summary>
    public FilterExpression()
    {
    }

    /// <summary>
    /// Initializes a new instance of the FilterExpression class with the specified expression.
    /// </summary>
    /// <param name="expression">The filter expression string.</param>
    public FilterExpression(string expression)
    {
      Expression = expression;
    }

    /// <summary>
    /// Evaluates the filter expression against the specified item.
    /// </summary>
    /// <param name="item">The item to evaluate.</param>
    /// <returns>True if the item matches the filter expression; otherwise, false.</returns>
    public bool Evaluate(object item)
    {
      if (string.IsNullOrWhiteSpace(_expression))
      {
        return true;
      }

      if (item == null)
      {
        return true;
      }

      if (_compiledExpression == null)
      {
        CompileExpression(item.GetType());
      }

      try
      {
        return _compiledExpression(item);
      }
      catch
      {
        // If evaluation fails, return true (don't filter out the item)
        return true;
      }
    }

    private void CompileExpression(Type itemType)
    {
      try
      {
        var parameter = System.Linq.Expressions.Expression.Parameter(itemType, "x");

        // Create a wrapper that casts the object to the correct type
        var objectParam = System.Linq.Expressions.Expression.Parameter(typeof(object), "obj");
        var castParam = System.Linq.Expressions.Expression.Variable(itemType, "casted");
        var expr = System.Linq.Expressions.Expression.Lambda<Func<object, bool>>(
          System.Linq.Expressions.Expression.Block(
            new[] { castParam },
            System.Linq.Expressions.Expression.Assign(
              castParam,
              System.Linq.Expressions.Expression.Convert(objectParam, itemType)
            ),
            System.Linq.Expressions.Expression.Convert(
              ParseLogicalExpression(_expression, itemType, castParam),
              typeof(bool)
            )
          ),
          objectParam
        );

        _compiledExpression = expr.Compile();
      }
      catch
      {
        // If compilation fails, create a function that always returns true
        _compiledExpression = _ => true;
      }
    }

    private static Expression ParseLogicalExpression(string expression, Type itemType, ParameterExpression parameter)
    {
      expression = expression?.Trim() ?? "";

      if (string.IsNullOrWhiteSpace(expression))
      {
        return System.Linq.Expressions.Expression.Constant(true);
      }

      // Handle logical OR (lowest precedence)
      var orIndex = FindTopLevelOperatorIndex(expression, "||");
      if (orIndex >= 0)
      {
        var left = ParseLogicalExpression(expression.Substring(0, orIndex), itemType, parameter);
        var right = ParseLogicalExpression(expression.Substring(orIndex + 2), itemType, parameter);
        return System.Linq.Expressions.Expression.OrElse(left, right);
      }

      // Handle logical AND (higher precedence than OR)
      var andIndex = FindTopLevelOperatorIndex(expression, "&&");
      if (andIndex >= 0)
      {
        var left = ParseLogicalExpression(expression.Substring(0, andIndex), itemType, parameter);
        var right = ParseLogicalExpression(expression.Substring(andIndex + 2), itemType, parameter);
        return System.Linq.Expressions.Expression.AndAlso(left, right);
      }

      // Handle NOT operator
      expression = expression.Trim();
      if (expression.StartsWith("!"))
      {
        var expr = ParseLogicalExpression(expression.Substring(1), itemType, parameter);
        return System.Linq.Expressions.Expression.Not(expr);
      }

      // Handle parentheses
      if (expression.StartsWith("(") && expression.EndsWith(")"))
      {
        return ParseLogicalExpression(expression.Substring(1, expression.Length - 2), itemType, parameter);
      }

      return ParseComparisonExpression(expression, parameter);
    }

    private static Expression ParseComparisonExpression(string expression, ParameterExpression parameter)
    {
      expression = expression.Trim();

      // Find comparison operator
      var comparisonOps = new[] { "==", "!=", ">=", "<=", ">", "<" };

      foreach (var op in comparisonOps)
      {
        var opIndex = FindTopLevelOperatorIndex(expression, op);
        if (opIndex > 0)
        {
          var leftExpr = expression.Substring(0, opIndex).Trim();
          var rightExpr = expression.Substring(opIndex + op.Length).Trim();

          var left = ParseValueExpression(leftExpr, parameter);
          var right = ParseValueExpression(rightExpr, parameter);

          // Ensure both sides have compatible types
          if (left.Type != right.Type)
          {
            // If both are numeric types, convert both to double for comparison
            if (IsNumericType(left.Type) && IsNumericType(right.Type))
            {
              left = System.Linq.Expressions.Expression.Convert(left, typeof(double));
              right = System.Linq.Expressions.Expression.Convert(right, typeof(double));
            }
            else if (right.Type == typeof(object))
            {
              right = System.Linq.Expressions.Expression.Convert(right, left.Type);
            }
            else if (left.Type == typeof(object))
            {
              left = System.Linq.Expressions.Expression.Convert(left, right.Type);
            }
          }

          return op switch
          {
            "==" => System.Linq.Expressions.Expression.Equal(left, right),
            "!=" => System.Linq.Expressions.Expression.NotEqual(left, right),
            ">" => System.Linq.Expressions.Expression.GreaterThan(left, right),
            "<" => System.Linq.Expressions.Expression.LessThan(left, right),
            ">=" => System.Linq.Expressions.Expression.GreaterThanOrEqual(left, right),
            "<=" => System.Linq.Expressions.Expression.LessThanOrEqual(left, right),
            _ => System.Linq.Expressions.Expression.Constant(true)
          };
        }
      }

      return System.Linq.Expressions.Expression.Constant(true);
    }

    private static Expression ParseValueExpression(string value, ParameterExpression parameter)
    {
      value = value.Trim();

      // String literal with single quotes
      if (value.StartsWith("'") && value.EndsWith("'"))
      {
        return System.Linq.Expressions.Expression.Constant(value.Substring(1, value.Length - 2));
      }

      // String literal with double quotes
      if (value.StartsWith("\"") && value.EndsWith("\""))
      {
        return System.Linq.Expressions.Expression.Constant(value.Substring(1, value.Length - 2));
      }

      // Numeric literal: try int first then double
      if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
      {
        return System.Linq.Expressions.Expression.Constant(intValue);
      }

      if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var doubleValue))
      {
        return System.Linq.Expressions.Expression.Constant(doubleValue);
      }

      // Boolean literal
      if (bool.TryParse(value, out var boolValue))
      {
        return System.Linq.Expressions.Expression.Constant(boolValue);
      }

      // Property access
      return ParsePropertyAccess(value, parameter);
    }

    private static Expression ParsePropertyAccess(string propertyPath, ParameterExpression parameter)
    {
      if (!PropertyRegex().IsMatch(propertyPath))
      {
        return System.Linq.Expressions.Expression.Constant(null);
      }

      var parts = propertyPath.Split('.');
      Expression expression = parameter;

      foreach (var part in parts)
      {
        var property = expression.Type.GetProperty(part, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (property == null)
        {
          return System.Linq.Expressions.Expression.Constant(null);
        }

        expression = System.Linq.Expressions.Expression.Property(expression, property);
      }

      return expression;
    }

    private static int FindTopLevelOperatorIndex(string expression, string op)
    {
      var index = 0;
      var inQuotes = false;
      var quoteChar = ' ';
      var parenDepth = 0;

      while (index < expression.Length - op.Length + 1)
      {
        var currentChar = expression[index];

        // Handle quotes
        if ((currentChar == '\'' || currentChar == '"') && (index == 0 || expression[index - 1] != '\\'))
        {
          if (!inQuotes)
          {
            inQuotes = true;
            quoteChar = currentChar;
          }
          else if (currentChar == quoteChar)
          {
            inQuotes = false;
          }
        }

        // Handle parentheses
        if (!inQuotes)
        {
          if (currentChar == '(')
          {
            parenDepth++;
          }
          else if (currentChar == ')')
          {
            parenDepth--;
          }
        }

        // Check for operator at top level
        if (!inQuotes && parenDepth == 0 && expression.Substring(index, op.Length) == op)
        {
          return index;
        }

        index++;
      }

      return -1;
    }

    private static bool IsNumericType(Type type)
    {
      if (type == null)
      {
        return false;
      }

      var t = Nullable.GetUnderlyingType(type) ?? type;
      return t == typeof(byte) || t == typeof(sbyte) || t == typeof(short) || t == typeof(ushort)
             || t == typeof(int) || t == typeof(uint) || t == typeof(long) || t == typeof(ulong)
             || t == typeof(float) || t == typeof(double) || t == typeof(decimal);
    }

    public override bool Equals(object obj)
    {
      return obj is FilterExpression other && Equals(_expression, other._expression);
    }

    public override int GetHashCode()
    {
      return _expression?.GetHashCode() ?? 0;
    }

    public override string ToString()
    {
      return _expression ?? "";
    }

    [GeneratedRegex(@"^[a-zA-Z_][a-zA-Z0-9_]*(\.[a-zA-Z_][a-zA-Z0-9_]*)*$")]
    private static partial Regex PropertyRegex();
  }
}
