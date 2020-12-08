//
// ISystemConstants.cs
//

namespace IntensityMapViewer
{

  public interface ISystemConstants
  {

    float MaxZoomInFactor ( PanAndZoomMode zoomMode ) ;

    float MaxZoomOutFactor ( PanAndZoomMode zoomMode ) ;

    public byte MaxAmplitudeOfNoiseToAddToSynthesiseImageMaps { get ; }

    // Hmm, these should be defined in the Theme ??

    float ProfileGraphLineThickness { get ; }

    System.Drawing.Color ProfileGraphLineColour { get ; }

  }

}
