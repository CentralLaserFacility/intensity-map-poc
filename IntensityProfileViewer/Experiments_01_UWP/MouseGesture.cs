//
// MouseGesture.cs
//

namespace Experiments_01_UWP
{

  public record MouseGesture ( ) ;

  public record PanGesture ( ) : MouseGesture ;

  public record ZoomGesture ( ) : MouseGesture ;

  public record PanGesture_Starting ( ) : PanGesture ;

  public record PanGesture_Changing ( FractionalXY deltaFromStartPoint ) : PanGesture ;

  public record PanGesture_Finished ( ) : PanGesture ;

  public record ZoomInGesture ( FractionalXY AnchorPoint ) : ZoomGesture ;

  public record ZoomOutGesture ( FractionalXY AnchorPoint ) : ZoomGesture ;

  public record PositionChangeNotification ( FractionalXY? CurrentPosition ) : MouseGesture ;

}
