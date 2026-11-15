using System.Collections.ObjectModel;
using FlowReports.Model;
using FlowReports.ViewModel.Editor;

namespace FlowReports.ViewModel
{
  /// <summary>
  /// Defines the interface for view models that contain report bands.
  /// </summary>
  public interface IBandParentViewModel
  {
    /// <summary>
    /// Gets the collection of report band view models.
    /// </summary>
    ObservableCollection<ReportBandViewModel> Bands { get; }

    /// <summary>
    /// Adds a new band to the collection.
    /// </summary>
    void AddBand();

    /// <summary>
    /// Adds a band relative to another band at a specified location.
    /// </summary>
    /// <param name="otherBand">The reference band.</param>
    /// <param name="location">The location where the band should be inserted relative to the reference band.</param>
    void AddBand(ReportBandViewModel otherBand, InsertLocation location);

    /// <summary>
    /// Removes a band from the collection.
    /// </summary>
    /// <param name="subBand">The band to remove.</param>
    void RemoveBand(ReportBandViewModel subBand);

    /// <summary>
    /// Moves a band up in the collection.
    /// </summary>
    /// <param name="band">The band to move.</param>
    void MoveBandUp(ReportBandViewModel band);

    /// <summary>
    /// Moves a band down in the collection.
    /// </summary>
    /// <param name="band">The band to move.</param>
    void MoveBandDown(ReportBandViewModel band);

    /// <summary>
    /// Determines whether a band can be moved up in the collection.
    /// </summary>
    /// <param name="band">The band to check.</param>
    /// <returns>true if the band can be moved up; otherwise, false.</returns>
    bool CanMoveBandUp(ReportBandViewModel band);

    /// <summary>
    /// Determines whether a band can be moved down in the collection.
    /// </summary>
    /// <param name="band">The band to check.</param>
    /// <returns>true if the band can be moved down; otherwise, false.</returns>
    bool CanMoveBandDown(ReportBandViewModel band);
  }
}