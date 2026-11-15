using FlowReports.Model.ReportItems;

namespace FlowReports.ViewModel.Editor
{
  public class OrderDescriptorViewModel : ViewModelBase
  {
    private string _property;
    private SortDirection _direction;
    private bool _isSelected;

    public OrderDescriptorViewModel()
    {
    }

    public OrderDescriptorViewModel(SortDescriptor model)
    {
      _property = model?.Property ?? string.Empty;
      _direction = model?.Direction ?? SortDirection.Ascending;
    }

    public string Property
    {
      get => _property;
      set
      {
        if (_property != value)
        {
          _property = value;
          OnPropertyChanged();
        }
      }
    }

    public SortDirection Direction
    {
      get => _direction;
      set
      {
        if (_direction != value)
        {
          _direction = value;
          OnPropertyChanged();
        }
      }
    }

    public bool IsSelected
    {
      get => _isSelected;
      set
      {
        if (_isSelected != value)
        {
          _isSelected = value;
          OnPropertyChanged();
        }
      }
    }

    public SortDescriptor ToModel()
    {
      return new SortDescriptor { Property = Property, Direction = Direction };
    }
  }
}
