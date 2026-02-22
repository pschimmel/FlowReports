using ES.Tools.Core.MVVM;
using FlowReports.Model.ReportItems;

namespace FlowReports.ViewModel.Editor
{
  public class HeaderBandViewModel : ReportbandBaseViewModel<HeaderBand>
  {
    #region Fields

    private ActionCommand _editBandDetails;

    #endregion

    #region Constructor

    public HeaderBandViewModel(HeaderBand band)
      : base(band)
    { }

    #endregion

    #region Properties

    public bool RepeatOnEachPage
    {
      get => Band.RepeatOnEachPage;
      set
      {
        if (Band.RepeatOnEachPage != value)
        {
          Band.RepeatOnEachPage = value;
          OnPropertyChanged();
        }
      }
    }

    #endregion

    #region Commands

    public ActionCommand EditBandDetailsCommand => _editBandDetails ??= new ActionCommand(EditBandDetails, CanEditBandDetails);

    private void EditBandDetails()
    {
      bool oldHeightAuto = HeightAuto;
      double oldHeight = Height;
      bool oldRepeatOnEachPage = RepeatOnEachPage;

      var view = ViewFactory.Instance.CreateView(this);
      if (view.ShowDialog() != true)
      {
        if (oldHeightAuto)
        {
          HeightAuto = true;
        }
        else
        {
          Height = oldHeight;
        }

        RepeatOnEachPage = oldRepeatOnEachPage;
      }
      else if (ReportVM != null)
      {
        ReportVM.IsDirty = true;
      }
    }

    private bool CanEditBandDetails()
    {
      return IsSelected;
    }

    #endregion
  }
}