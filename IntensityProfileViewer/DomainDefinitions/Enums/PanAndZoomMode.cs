//
// PanAndZoomMode.cs
//

namespace IntensityProfileViewer
{

  //
  // In 'constrained' mode we limit the factor by which you can zoom in and out,
  // and also ensure that the image isn't permitted to be panned 'off the screen'.
  //

  public enum PanAndZoomMode {
    Constrained,
    Unconstrained
  }

}
