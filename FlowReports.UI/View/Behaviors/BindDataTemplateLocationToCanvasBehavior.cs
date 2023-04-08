using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ES.Tools.UI;
using FlowReports.UI.ViewModel;
using Microsoft.Xaml.Behaviors;

namespace FlowReports.UI.View.Behaviors
{
  /// <summary>
  /// Behavior that connects the size and location properties of a <see cref="IEditorItemViewModel"/> with the respective properties
  /// of a control.
  /// </summary>
  internal class BindDataTemplateLocationToCanvasBehavior : Behavior<FrameworkElement>
  {
    protected override void OnAttached()
    {
      base.OnAttached();

      // Connect the attached properties Canvas.Left and Canvas.Top with the parent content
      // presenter, as this is the control that is actually contained in the canvas.
      var parentContentPresenter = AssociatedObject.GetParent<ContentPresenter>();

      if (parentContentPresenter != null)
      {
        var bindingLeft = new Binding
        {
          Source = AssociatedObject,
          Path = new PropertyPath(Canvas.LeftProperty),
          Mode = BindingMode.OneWay
        };
        parentContentPresenter.SetBinding(Canvas.LeftProperty, bindingLeft);

        var bindingTop = new Binding
        {
          Source = AssociatedObject,
          Path = new PropertyPath(Canvas.TopProperty),
          Mode = BindingMode.OneWay
        };
        parentContentPresenter.SetBinding(Canvas.TopProperty, bindingTop);
      }
    }

    protected override void OnDetaching()
    {
      var parentContentPresenter = AssociatedObject.GetParent<ContentPresenter>();
      if (parentContentPresenter != null)
      {
        BindingOperations.ClearBinding(parentContentPresenter, Canvas.TopProperty);
        BindingOperations.ClearBinding(parentContentPresenter, Canvas.LeftProperty);
      }

      base.OnDetaching();
    }
  }
}

