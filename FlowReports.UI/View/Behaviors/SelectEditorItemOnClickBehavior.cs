using System.Windows;
using System.Windows.Input;
using FlowReports.UI.ViewModel;
using Microsoft.Xaml.Behaviors;

namespace FlowReports.UI.View.Behaviors
{
  internal class SelectEditorItemOnClickBehavior : Behavior<FrameworkElement>
  {
    private IItemViewModel _vm;

    protected override void OnAttached()
    {
      base.OnAttached();

      if (AssociatedObject.DataContext is not IItemViewModel vm)
      {
        throw new ArgumentException($"Data Context of UI element is not an {nameof(IItemViewModel)}.");
      }

      _vm = vm;
      AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
    }

    protected override void OnDetaching()
    {
      if (_vm != null)
      {
        AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
      }

      _vm = null;
      base.OnDetaching();
    }

    private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      _vm.IsSelected = true;
    }
  }
}

