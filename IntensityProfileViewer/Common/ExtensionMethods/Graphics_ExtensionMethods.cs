//
// Graphics_ExtensionMethods.cs
//

namespace Common.ExtensionMethods
{

  public static partial class Graphics_ExtensionMethods
  {

    public static System.Drawing.Point ConstrainedToBeInside (
      this System.Drawing.Point point,
      System.Drawing.Rectangle  rectangle
    ) => new System.Drawing.Point(
      point.X.ClampedToInclusiveRange(rectangle.Left,rectangle.Right-1),
      point.Y.ClampedToInclusiveRange(rectangle.Top,rectangle.Bottom-1)
    ) ;

    public static string ToPixelPositionString (
      this System.Drawing.Point point
    ) => $"[{point.X},{point.Y}]" ;

  }

}
