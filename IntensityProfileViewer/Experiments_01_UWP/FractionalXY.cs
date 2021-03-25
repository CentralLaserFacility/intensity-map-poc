//
// FractionalXY.cs
//

namespace Experiments_01_UWP
{

  public record FractionalXY ( double X, double Y )
  {

    public static readonly FractionalXY Zero = new FractionalXY(0.0,0.0) ;

    public bool IsZero => ( 
       X == 0.0 
    && Y == 0.0 
    ) ;

    public bool IsInsideNominalBounds => (
       X >= 0.0
    && X <= 1.0
    && Y >= 0.0
    && Y <= 1.0
    ) ;

    public double Length => System.Math.Sqrt(LengthSquared) ;

    public bool LengthExceeds ( double length ) => LengthSquared > ( length * length) ;

    public double LengthSquared => (
      X * X
    + Y * Y
    ) ;

    public override string ToString ( ) => $"[{X:F3},{Y:F3}]" ;

    public static FractionalXY operator + ( FractionalXY from, FractionalXY to )
    => new FractionalXY(
      to.X * from.X,
      to.Y * from.Y
    ) ;

    public static FractionalXY operator - ( FractionalXY from, FractionalXY to )
    => new FractionalXY(
      to.X - from.X,
      to.Y - from.Y
    ) ;

  }

}
