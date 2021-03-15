//
// IntensityMap creation.cs
//

using Common.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;

namespace IntensityProfileViewer
{
  
  public partial class IntensityMap
  {

    public class CreatedFromSincFunction : IntensityMap
    {

      public CreatedFromSincFunction ( 
        int    width                                    = 320,
        int    height                                   = 240,
        double sincFactor                               = 10.0,
        double powerFactor                              = 1.0,
        double fractionalRadialOffsetFromCentre         = 0.0,
        double fractionalRotationOfOffsettedCentrePoint = 0.0,
        byte   maxIntensity                             = 255
      ) :
      base(
        new System.Drawing.Size(width,height)
      ) {
        IntensityValues = Common.GraphicsHelpers.ForEachPixel(
          nX             : width,
          nY             : height,
          pixelValueFunc : (x,y) => GetPixelValue(x,y)
        ).ToList() ;
        // Old version ...
        // var imageBytes = new byte[width*height] ;
        // int jFirstByteNotYetWritten = 0 ;
        // Common.GraphicsHelpers.ForEachPixel(
        //   nX : width,
        //   nY : height,
        //   (x,y) => {
        //     imageBytes[
        //       jFirstByteNotYetWritten++
        //     ] = GetPixelValue(x,y) ;
        //   }
        // ) ;
        // Very old version ...
        // Enumerable.Range(0,height).ForEachItem(
        //   y => {
        //     Enumerable.Range(0,width).ForEachItem(
        //       x => {
        //         imageBytes[
        //           jFirstByteNotYetWritten++
        //         ] = GetPixelValue(x,y) ;
        //       }
        //     ) ;
        //   }
        // ) ;
        // IntensityValues = imageBytes ;
        byte GetPixelValue ( int x, int y )
        {
          // Sin(x)/x around a point which is roughly at the centre,
          // but offset by an amount that represents a rotation 
          // around a circe centred on the centre point.
          // NOTE : Could avoid the sin() function and use
          // a Taylor expansion as an approximation for sin(x)/x :
          //   1 - (x^2/3!) + (x^4/5!) - (x^6/7!) ...
          // ... where 'x' is taken modulo 2-PI.
          // Can also avoid the overhead of an exact SQRT,
          // by running a few iterations of Newton's formula.
          // Could use integer arithmetic (faster?) if we
          // settle on x^6 as the highest power, and premultiply
          // by 7! to minimise the number of division operations.
          // Another option would be to use a pre-computed lookup table for sin(x)/x
          // with a set of say 1024 entries covering the likely range of 'x'.
          // That table could also accommodate the SQRT computation.
          int nominalCentreX = width / 2 ;
          int nominalCentreY = height / 2 ;
          int offsetX = (int) ( 
            fractionalRadialOffsetFromCentre * width * System.Math.Cos(
              fractionalRotationOfOffsettedCentrePoint * 2.0 * System.Math.PI 
            )
          ) ;
          int offsetY = (int) ( 
            fractionalRadialOffsetFromCentre * width * System.Math.Sin(
              fractionalRotationOfOffsettedCentrePoint * 2.0 * System.Math.PI 
            )
          ) ;
          int dx = x - nominalCentreX - offsetX ;
          int dy = y - nominalCentreY - offsetY ;
          double dxFrac01 = 2.0 * dx / height ; // Deliberately !! To keep circular symmetry
          double dyFrac01 = 2.0 * dy / height ;
          // Without the SQRT, the central blob is a lot wider ...
          double r = sincFactor * System.Math.Sqrt(
            dxFrac01 * dxFrac01
          + dyFrac01 * dyFrac01
          ) ;
          double h = System.Math.Abs(
            r == 0.0
            ? 1.0
            : System.Math.Sin(r) /  r
          ) ;
          byte greyValue = (byte) ( 
            maxIntensity 
          * System.Math.Pow(h,powerFactor) 
          ) ;
          return greyValue ;
        }
      }
    }

    public class CreatedWithRampingValues : IntensityMap
    {

      public CreatedWithRampingValues ( 
        int width      = 320,
        int height     = 240,
        int rampFactor = 2
      ) :
      base(
        new System.Drawing.Size(width,height)
      ) {
        IntensityValues = Common.GraphicsHelpers.ForEachPixel(
          nX             : width,
          nY             : height,
          pixelValueFunc : (x,y) => GetPixelValue(x,y)
        ).ToList() ;
        byte GetPixelValue ( int x, int y )
        {
          return (byte) ( x * rampFactor ) ;
        }
      }
    }

    public class CreatedAsOffsettedCircle : IntensityMap
    {

      public CreatedAsOffsettedCircle ( 
        int    width                                    = 320,
        int    height                                   = 240,
        double fractionalRadius                         = 0.1,
        double fractionalRadialOffsetFromCentre         = 0.2,
        double fractionalRotationOfOffsettedCentrePoint = 0.0,
        byte   backgroundPixel                          = 0x30,
        byte   foregroundPixel                          = 0xf0
      ) :
      base(
        new System.Drawing.Size(width,height)
      ) {
        IntensityValues = Common.GraphicsHelpers.ForEachPixel(
          nX             : width,
          nY             : height,
          pixelValueFunc : (x,y) => GetPixelValue(x,y)
        ).ToList() ;
        byte GetPixelValue ( int x, int y )
        {
          // Circular blob around a point which is roughly at the centre,
          // but offset by an amount that represents a rotation 
          // around a circe centred on the centre point.
          double nominalCentreX = width / 2 ;
          double nominalCentreY = height / 2 ;
          double offsetX = ( 
            fractionalRadialOffsetFromCentre * width * System.Math.Cos(
              fractionalRotationOfOffsettedCentrePoint * 2.0 * System.Math.PI 
            )
          ) ;
          double offsetY = ( 
            fractionalRadialOffsetFromCentre * width * System.Math.Sin(
              fractionalRotationOfOffsettedCentrePoint * 2.0 * System.Math.PI 
            )
          ) ;
          double offsettedCentreX = nominalCentreX + offsetX ;
          double offsettedCentreY = nominalCentreY + offsetY ;
          // Compute the fractional distance by which the current pixel
          // deviates from the offsetted 'centre point'. If it's less than
          // the specified radius, we consider this pixel to be 'inside'.
          double dx = x - offsettedCentreX ;
          double dy = y - offsettedCentreY ;
          double dxFrac01 = dx / height ; // Deliberately !! To keep circular symmetry
          double dyFrac01 = dy / height ;
          double deltaFrac01 = System.Math.Sqrt(
            dxFrac01 * dxFrac01
          + dyFrac01 * dyFrac01
          ) ;
          byte greyValue = ( 
            deltaFrac01 < fractionalRadius
            ? foregroundPixel // Inside the circle
            : backgroundPixel // Outside ...
          ) ;
          return greyValue ;
        }
      }
    }

    public class CreatedAsUniformPixelValue : IntensityMap
    {

      public CreatedAsUniformPixelValue ( 
        int   width      = 320,
        int   height     = 240,
        byte  pixelValue = 0x7f
      ) :
      base(
        new System.Drawing.Size(width,height)
      ) {
        IntensityValues = Common.GraphicsHelpers.ForEachPixel(
          nX             : width,
          nY             : height,
          pixelValueFunc : (x,y) => pixelValue
        ).ToList() ;
      }
    }

    public class CreatedFromCoordinatesXY : IntensityMap
    {

      public CreatedFromCoordinatesXY ( 
        System.Func<int,int,byte> getPixelValueFunc,
        int                       width      = 320,
        int                       height     = 240
      ) :
      base(
        new System.Drawing.Size(width,height)
      ) {
        IntensityValues = Common.GraphicsHelpers.ForEachPixel(
          nX             : width,
          nY             : height,
          pixelValueFunc : getPixelValueFunc
        ).ToList() ;
      }
    }

  }

}
