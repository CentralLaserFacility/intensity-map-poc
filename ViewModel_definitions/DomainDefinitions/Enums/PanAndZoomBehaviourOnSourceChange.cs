//
// PanAndZoomBehaviourOnSourceChange.cs
//

namespace IntensityMapViewer
{

  //
  // When the 'source' changes, ie to a different camera,
  // should we (A) reset the Pan/Zoom settings to their nominal values,
  // or (B) restore the previous settings that were being applied.
  //

  public enum PanAndZoomBehaviourOnSourceChange {
    ResetToNominal,
    RestoreSavedSettings
  }

}
