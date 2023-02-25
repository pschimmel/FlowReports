using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using ES.Tools.UI;
using FlowReports.UI.ViewModel;
using Microsoft.Xaml.Behaviors;

namespace FlowReports.UI.View.Behaviors
{
  /// <summary>
  /// Behavior that connects the size and location properties of a <see cref="IItemViewModel"/> with the respective properties
  /// of a control.
  /// </summary>
  internal class ResizeEditorItemBehavior : Behavior<FrameworkElement>
  {
    private IItemViewModel _vm;
    private AdornerLayer _adornerLayer;
    private ReportItemAdorner _itemAdorner;

    protected override void OnAttached()
    {
      base.OnAttached();
      if (AssociatedObject.DataContext is not IItemViewModel vm)
      {
        throw new ArgumentException($"Data Context of UI element is not an {nameof(IItemViewModel)}.");
      }

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

      _vm = vm;
      _vm.PropertyChanged += VM_PropertyChanged;

      if (_vm.IsSelected)
      {
        ShowAdorner();
      }
    }

    protected override void OnDetaching()
    {
      if (_vm != null)
      {
        _vm.PropertyChanged -= VM_PropertyChanged;
      }

      var parentContentPresenter = AssociatedObject.GetParent<ContentPresenter>();
      if (parentContentPresenter != null)
      {
        BindingOperations.ClearBinding(parentContentPresenter, Canvas.TopProperty);
        BindingOperations.ClearBinding(parentContentPresenter, Canvas.LeftProperty);
      }

      _vm = null;
      base.OnDetaching();
    }

    private void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(IItemViewModel.IsSelected))
      {
        if (_vm.IsSelected)
        {
          ShowAdorner();
        }
        else
        {
          HideAdorner();
        }
      }
    }

    private void HideAdorner()
    {
      _adornerLayer?.Remove(_itemAdorner);
      _itemAdorner.MoveLeftBorder -= ItemAdorner_MoveLeftBorder;
      _itemAdorner.MoveRightBorder -= ItemAdorner_MoveRightBorder;
      _itemAdorner.MoveTopBorder -= ItemAdorner_MoveTopBorder;
      _itemAdorner.MoveBottomBorder -= ItemAdorner_MoveBottomBorder;
      _itemAdorner = null;
      _adornerLayer = null;
    }

    private void ShowAdorner()
    {
      _adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
      _itemAdorner = new ReportItemAdorner(AssociatedObject);
      _adornerLayer.Add(_itemAdorner);
      _itemAdorner.MoveLeftBorder += ItemAdorner_MoveLeftBorder;
      _itemAdorner.MoveRightBorder += ItemAdorner_MoveRightBorder;
      _itemAdorner.MoveTopBorder += ItemAdorner_MoveTopBorder;
      _itemAdorner.MoveBottomBorder += ItemAdorner_MoveBottomBorder;
    }

    private void ItemAdorner_MoveTopBorder(double delta)
    {
      _vm.Height -= delta;
      _vm.Top += delta;
    }

    private void ItemAdorner_MoveLeftBorder(double delta)
    {
      _vm.Width -= delta;
      _vm.Left += delta;
    }

    private void ItemAdorner_MoveRightBorder(double delta)
    {
      _vm.Width += delta;
    }

    private void ItemAdorner_MoveBottomBorder(double delta)
    {
      _vm.Height += delta;
    }
  }
}

