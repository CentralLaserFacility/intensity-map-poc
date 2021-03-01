//
// PixelToSceneCoordinatesMapper.cs
//

namespace SkiaUtilities
{

  public class PixelToSceneCoordinatesMapper
  {

    public System.Drawing.Size PixelDimensions { get ; }

    public SkiaSharp.SKSize SceneDimensions { get ; }

    public PixelToSceneCoordinatesMapper (
      System.Drawing.Size pixelDimensions,
      SkiaSharp.SKSize    sceneDimensions
    ) {
      PixelDimensions = pixelDimensions ;
      SceneDimensions = sceneDimensions ;
    }

    public bool CanGetPointInSceneCoordinates ( 
      System.Drawing.Point?  pointInPixelCoordinates,
      /*[NotNullWhen(true)]*/ out  SkiaSharp.SKPoint? pointInSceneCoordinates
    ) => (
      pointInSceneCoordinates = (
        pointInPixelCoordinates.HasValue
        ? new SkiaSharp.SKPoint(
            (float) Scale(
              pointInPixelCoordinates.Value.X,
              PixelDimensions.Width,
              SceneDimensions.Width
            ),
            (float) Scale(
              pointInPixelCoordinates.Value.Y,
              PixelDimensions.Height,
              SceneDimensions.Height
            )
          )
        : null
      )
    ) != null ;

    public bool CanGetPointInPixelCoordinates ( 
      SkiaSharp.SKPoint?                                pointInSceneCoordinates,
      /*[NotNullWhen(true)]*/ out System.Drawing.Point? pointInPixelCoordinates
          
    ) => (
      pointInPixelCoordinates = (
        pointInSceneCoordinates.HasValue
        ? new System.Drawing.Point(
            (int) Scale(
              pointInSceneCoordinates.Value.X,
              SceneDimensions.Width,
              PixelDimensions.Width
            ),
            (int) Scale(
              pointInSceneCoordinates.Value.Y,
              SceneDimensions.Height,
              PixelDimensions.Height
            )
          )
        : null
      )
    ) != null ;

    private static float Scale ( float value, float nImagePixels, float nDisplayPixels )
    => (
      value * nDisplayPixels / nImagePixels
    ) ;

  }


}
