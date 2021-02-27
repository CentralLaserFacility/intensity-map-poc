//
// PanAndZoomParameters.cs
//

namespace IntensityMapViewer
{

  public class PanAndZoomParameters
  {

    //
    // These parameters define an Affine Transform
    // that gets us from nominal 'model' coordinates
    // to zoomed-and-panned 'View' coordinates.
    //

    public float ScaleXY { get ; set ; }

    public float TranslateX { get ; set ; }

    public float TranslateY { get ; set ; }

  }

}
