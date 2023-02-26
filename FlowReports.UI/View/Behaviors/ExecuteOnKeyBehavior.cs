using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace FlowReports.UI.View.Behaviors
{
  /// <summary>
  /// Executes any command when the control is clicked.
  /// This excutes the command on PreviewMouseLeftButtonDown.
  /// </summary>
  internal class ExecuteOnKeyBehavior : Behavior<FrameworkElement>, ICommandSource
  {
    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
    }

    protected override void OnDetaching()
    {
      AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
      base.OnDetaching();
    }

    public static DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ExecuteOnKeyBehavior));

    public ICommand Command
    {
      get => (ICommand)GetValue(CommandProperty);
      set => SetValue(CommandProperty, value);
    }

    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(ExecuteOnKeyBehavior), new UIPropertyMetadata(null));

    public object CommandParameter
    {
      get => GetValue(CommandParameterProperty);
      set => SetValue(CommandParameterProperty, value);
    }

    public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(ExecuteOnKeyBehavior), new UIPropertyMetadata(null));

    public IInputElement CommandTarget
    {
      get => (IInputElement)GetValue(CommandTargetProperty);
      set => SetValue(CommandTargetProperty, value);
    }

    public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(Key), typeof(ExecuteOnKeyBehavior), new UIPropertyMetadata(default(Key)));

    public Key Key
    {
      get => (Key)GetValue(KeyProperty);
      set => SetValue(KeyProperty, value);
    }

    private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key)
      {
        var command = Command;
        var parameter = CommandParameter;
        var target = CommandTarget;

        if (command is RoutedCommand routedCmd && routedCmd.CanExecute(parameter, target))
        {
          routedCmd.Execute(parameter, target);
        }
        else if (command != null && command.CanExecute(parameter))
        {
          command.Execute(parameter);
        }
      }
    }
  }
}

