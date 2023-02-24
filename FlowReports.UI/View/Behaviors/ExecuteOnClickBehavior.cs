using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace FlowReports.UI.View.Behaviors
{
  internal class ExecuteOnClickBehavior : Behavior<FrameworkElement>, ICommandSource
  {
    public static DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ExecuteOnClickBehavior));

    public ICommand Command
    {
      get => (ICommand)GetValue(CommandProperty);
      set => SetValue(CommandProperty, value);
    }

    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
    }

    protected override void OnDetaching()
    {
      AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
      base.OnDetaching();
    }

    private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

    // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(ExecuteOnClickBehavior), new UIPropertyMetadata(null));

    public object CommandParameter
    {
      get => GetValue(CommandParameterProperty);
      set => SetValue(CommandParameterProperty, value);
    }

    // Using a DependencyProperty as the backing store for CommandTarget.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(ExecuteOnClickBehavior), new UIPropertyMetadata(null));

    public IInputElement CommandTarget
    {
      get => (IInputElement)GetValue(CommandTargetProperty);
      set => SetValue(CommandTargetProperty, value);
    }
  }
}

