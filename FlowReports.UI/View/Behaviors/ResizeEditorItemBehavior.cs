using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using FlowReports.UI.View.Adorners;
using FlowReports.ViewModel.EditorItems;
using Microsoft.Xaml.Behaviors;

namespace FlowReports.UI.View.Behaviors
{
  /// <summary>
  /// Behavior that connects the size and location properties of a <see cref="IEditorItemViewModel"/> with the respective properties
  /// of a control.
  /// </summary>
  internal class ResizeEditorItemBehavior : Behavior<FrameworkElement>
  {
    private IEditorItemViewModel _vm;
    private AdornerLayer _adornerLayer;
    private ReportItemAdorner _itemAdorner;

    protected override void OnAttached()
    {
      base.OnAttached();
      if (AssociatedObject.DataContext is not IEditorItemViewModel vm)
      {
        throw new ArgumentException($"Data Context of UI element is not an {nameof(IEditorItemViewModel)}.");
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

      _vm = null;
      base.OnDetaching();
    }

    private void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(IEditorItemViewModel.IsSelected))
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
      if (_itemAdorner != null)
      {
        _adornerLayer?.Remove(_itemAdorner);
        _itemAdorner.ChangeLocation -= ItemAdorner_ChangeLocation;
        _itemAdorner = null;
      }
      _adornerLayer = null;
    }

    private void ShowAdorner()
    {
      _adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
      _itemAdorner = new ReportItemAdorner(AssociatedObject);
      _adornerLayer.Add(_itemAdorner);
      _itemAdorner.ChangeLocation += ItemAdorner_ChangeLocation;
    }

    private void ItemAdorner_ChangeLocation(Rect newLocation)
    {
      _vm.Top += newLocation.Top;
      _vm.Left += newLocation.Left;
      _vm.Width = newLocation.Width;
      _vm.Height = newLocation.Height;
    }
  }
}

