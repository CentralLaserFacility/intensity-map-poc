//
// DrawingHelpers.cs
//

using Windows.Foundation ;

namespace UwpUtilities
{

  public static class DrawingHelpers
  {

    public static Point MovedBy (
      this Point point,
      double     deltaX,
      double     deltaY
    ) => new Point(
      point.X + deltaX,
      point.Y + deltaY
    ) ;

  }

}
