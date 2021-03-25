//
// MouseEventHandler.cs
//

namespace Experiments_01_UWP
{

  public enum MouseDeltaType {
    EnteredActiveRegion,
    ExitedActiveRegion,
    PositionChanged,
    LeftButtonPressed,
    LeftButtonReleased,
    RightButtonPressed,
    RightButtonReleased,
    WheelNudgedForwards,
    WheelNudgedBackwards,
    // These low level events are of no interest
    // to client code at higher levels ...
    Canceled,
    CatureGained,
    CaptureLost,
  }

  public record MouseDeltaDescriptor ( ) ;

  public record EnteredActiveRegion : MouseDeltaDescriptor ;

  public record ExitedActiveRegion : MouseDeltaDescriptor ;

  public record LeftButtonPressed : MouseDeltaDescriptor ;

  public record LeftButtonReleased : MouseDeltaDescriptor ;

  public record RightButtonPressed : MouseDeltaDescriptor ;

  public record RightButtonReleased : MouseDeltaDescriptor ;

  public record WheelNudgedForwards : MouseDeltaDescriptor ;

  public record WheelNudgedBackwards : MouseDeltaDescriptor ;

  public record PositionChanged ( FractionalXY PositionChange ) : MouseDeltaDescriptor ;

}
