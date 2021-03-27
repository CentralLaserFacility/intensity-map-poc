//
// PixelToSceneCoordinatesMapper.cs
//

using Common.ExtensionMethods ;

namespace SkiaUtilities
{

  public class PixelToSceneCoordinatesMapper
  {

    public System.Drawing.Size PixelDimensions { get ; }

    public System.Drawing.Rectangle PixelRectangle { get ; }

    public SkiaSharp.SKSize SceneDimensions { get ; }

    public SkiaSharp.SKRect SceneRectangle { get ; }

    public PixelToSceneCoordinatesMapper (
      System.Drawing.Size pixelDimensions,
      SkiaSharp.SKSize    sceneDimensions
    ) {
      PixelDimensions = pixelDimensions ;
      SceneDimensions = sceneDimensions ;
      PixelRectangle = new System.Drawing.Rectangle(
        new System.Drawing.Point(0,0),
        PixelDimensions
      ) ;
      SceneRectangle = SkiaSharp.SKRect.Create(
        new SkiaSharp.SKPoint(0.0f,0.0f),
        SceneDimensions
      ) ;
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
          ).ConstrainedToBeInside(SceneRectangle)
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
          ).ConstrainedToBeInside(PixelRectangle)
        : null
      )
    ) != null ;

    private static float Scale ( float value, float nImagePixels, float nDisplayPixels )
    => (
      value * nDisplayPixels / nImagePixels
    ) ;

  }


}
