using ES.Tools.Core.MVVM;
using FlowReports.Model.ReportItems;

namespace FlowReports.ViewModel.Editor
{
  public class FooterBandViewModel : ReportbandBaseViewModel<FooterBand>
  {
    #region Fields

    private ActionCommand _editBandDetails;

    #endregion

    public FooterBandViewModel(FooterBand band)
      : base(band)
    {
    }

    #region Commands

    public ActionCommand EditBandDetailsCommand => _editBandDetails ??= new ActionCommand(EditBandDetails, CanEditBandDetails);

    private void EditBandDetails()
    {
      bool oldHeightAuto = HeightAuto;
      double oldHeight = Height;

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
