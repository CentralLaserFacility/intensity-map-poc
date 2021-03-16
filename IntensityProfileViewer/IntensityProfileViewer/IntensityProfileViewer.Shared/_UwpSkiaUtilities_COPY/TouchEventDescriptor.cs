//
// TouchEventDescriptor.cs
//

namespace UwpSkiaUtilities
{

  public class TouchEventDescriptor
  {
    public TouchTracking.TouchActionType EventType ;
    public SkiaSharp.SKPoint             TouchPositionInSceneCoordinates ;
    public bool                          InContact ;
  }

}
