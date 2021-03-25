//
// FractionalXY.cs
//

namespace Experiments_01_UWP
{

  public record FractionalXY ( double X, double Y )
  {

    public bool IsZero => ( 
       X == 0.0 
    && Y == 0.0 
    ) ;

    public double Length => System.Math.Sqrt(LengthSquared) ;

    public bool LengthExceeds ( double length ) => LengthSquared > ( length * length) ;

    public double LengthSquared => (
      X * X
    + Y * Y
    ) ;

    public override string ToString ( ) => $"[{X:F3},{Y:F3}]" ;

  }

}
