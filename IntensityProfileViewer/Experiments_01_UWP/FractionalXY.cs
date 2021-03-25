//
// FractionalXY.cs
//

namespace Experiments_01_UWP
{

  public record FractionalXY ( double X, double Y )
  {
    public override string ToString ( ) => $"[{X:F3},{Y:F3}]" ;
  }

}
