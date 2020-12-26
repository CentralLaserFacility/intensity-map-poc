//
// BitmapHelpers_ForTesting.cs
//

using System.Linq;

using Common.ExtensionMethods;
using static System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions;

namespace UwpUtilities
{

  public static class BitmapHelpers_ForTesting
  {

    //
    // Build a 1-D array and then write it to the bitmap.
    //

    public static Windows.UI.Xaml.Media.Imaging.WriteableBitmap 
    CreateWriteableBitmap_ForTesting_A_SolidColour ( 
      (byte r, byte g, byte b) rgb,
      int width  = 256,
      int height = 256
    ) { 
      var imageBytesArray = new byte[width*height*4] ;
      int jFirstByteNotYetWritten = 0 ;
      Enumerable.Range(0,width).ForEachItem(
        x => {
          Enumerable.Range(0,height).ForEachItem(
            y => {
              imageBytesArray[jFirstByteNotYetWritten++] = rgb.b ; // B
              imageBytesArray[jFirstByteNotYetWritten++] = rgb.g ; // G
              imageBytesArray[jFirstByteNotYetWritten++] = rgb.r ; // R
              imageBytesArray[jFirstByteNotYetWritten++] = 0xff  ; // A
            }
          ) ;
        }
      ) ;
      Windows.UI.Xaml.Media.Imaging.WriteableBitmap bitmap = new(
        width,
        height
      ) ;
      using ( var stream = bitmap.PixelBuffer.AsStream() )
      {
        stream.Write(
          imageBytesArray,
          0,
          imageBytesArray.Length
        ) ;
      }
      return bitmap ;
    }

    //
    // Write bytes to the bitmap directly.
    //

    public static Windows.UI.Xaml.Media.Imaging.WriteableBitmap 
    CreateWriteableBitmap_ForTesting_B ( 
      System.Func<int,int,(byte r, byte g, byte b)> getRgbFunc,
      int                                           width  = 320,
      int                                           height = 240
    ) { 
      var bitmap = new Windows.UI.Xaml.Media.Imaging.WriteableBitmap(
        width,
        height
      ) ;
      var pixelBuffer = bitmap.PixelBuffer ;
      using ( var stream = pixelBuffer.AsStream() )
      {
        Enumerable.Range(0,height).ForEachItem(
          y => {
            Enumerable.Range(0,width).ForEachItem(
              x => {
                var (r,g,b) = getRgbFunc(x,y) ;
                stream.WriteByte(b)     ; // B
                stream.WriteByte(g)     ; // G
                stream.WriteByte(r)     ; // R
                stream.WriteByte(0xff)  ; // A
              }
            ) ;
          }
        ) ;
      }
      return bitmap ;
      // byte GetPixelValue ( int x, int y )
      // {
      //   return (byte) ( ( x + y ) % 256 ) ;
      // }
    }

    // Write a solid colour

    public static Windows.UI.Xaml.Media.Imaging.WriteableBitmap 
    CreateWriteableBitmap_ForTesting_C ( 
      uint colour_argb,
      int  width  = 320,
      int  height = 240
    ) { 
      var bitmap = new Windows.UI.Xaml.Media.Imaging.WriteableBitmap(
        width,
        height
      ) ;
      var pixelBuffer = bitmap.PixelBuffer ;
      using var stream = pixelBuffer.AsStream() ;
      using var binaryWriter = new System.IO.BinaryWriter(stream) ;
      int nPixelsNotYetWritten = width * height ;
      while ( nPixelsNotYetWritten-- != 0 )
      {
        binaryWriter.Write(colour_argb) ;
      }
      return bitmap ;
    }

  }

}
