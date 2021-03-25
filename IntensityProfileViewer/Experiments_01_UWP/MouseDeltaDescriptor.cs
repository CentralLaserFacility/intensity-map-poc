//
// MouseEventHandler.cs
//

namespace Experiments_01_UWP
{

  public enum MouseDeltaType {
    EnteredActiveRegion,
    ExitedActiveRegion,
    PositionChanged,
    PositionChangedSignificantly,
    LeftButtonPressed,
    LeftButtonReleased,
    RightButtonPressed,
    RightButtonReleased,
    WheelNudgedForwards,
    WheelNudgedBackwards,
    Canceled,
    CatureGained,
    CaptureLost,
  }

  public record MouseDeltaDescriptor ( 
    FractionalXY PositionChange,
    double       TimeDeltaInMillisecs,
    int          WheelDelta
  ) {
    public bool   PositionChanged => PositionChange.IsZero ;
    public double DistanceMoved   => PositionChange.Length ;
  }

}
