//
// BitmapHelpers.cs
//

using System.Collections.Generic ;
using System.Linq ;

using Common.ExtensionMethods ;
using static System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions ;

namespace UwpUtilities
{

  public static class BitmapHelpers_old_01
  {

    // Aha, with 'IsExternalInit' defined in AssemblyInfo.cs
    // this proves that C# 9 features are enabled !!!
  
    public record AA ( int X ) ;

    //
    // This helper converts from the raw image data supplied by an IntensityMap
    // to a BitmapSource that will serve as the data source for an Image control. 
    //
    // Each byte of the image will be mapped to an RGB Colour.
    //

    public static Windows.UI.Xaml.Media.Imaging.BitmapSource BuildBitmapSource (
      IntensityMapViewer.IIntensityMap   intensityMap,
      IntensityMapViewer.ColourMapOption colourMapOption = IntensityMapViewer.ColourMapOption.JetColours,
      byte                               highestIntensityValue = (byte) 255
    ) { 
      return CreateWriteableBitmap(
        intensityMap,
        colourMapOption,
        highestIntensityValue
      ) ;
    }

    public static Windows.UI.Xaml.Media.Imaging.WriteableBitmap CreateWriteableBitmap ( 
      IntensityMapViewer.IIntensityMap   intensityMap,
      IntensityMapViewer.ColourMapOption colourMapOption = IntensityMapViewer.ColourMapOption.JetColours,
      byte                               highestIntensityValue = (byte) 255
    ) { 
      var bitmap = new Windows.UI.Xaml.Media.Imaging.WriteableBitmap(
        intensityMap.Dimensions.Width,
        intensityMap.Dimensions.Height
      ) ;
      var colourMapper = IntensityMapViewer.ColourMapper.InstanceFor(colourMapOption) ;
      var pixelBuffer = bitmap.PixelBuffer ;
      using var stream = pixelBuffer.AsStream() ;
      using var binaryWriter = new System.IO.BinaryWriter(stream) ;
      Enumerable.Range(
        0,
        intensityMap.IntensityValues.Count
      ).ForEachItem(
        jPixel => {
          byte pixelByteValue = intensityMap.IntensityValues[jPixel] ;
          binaryWriter.Write(
            colourMapper.MapByteValueToEncodedARGB(pixelByteValue)
          ) ;
          // It's more efficient to write 4-byte 'uint' values,
          // but this is how we would write individual bytes
          // if we wanted 'greyscale' values ...
          // stream.WriteByte(pixelByteValue) ; // B
          // stream.WriteByte(pixelByteValue) ; // G
          // stream.WriteByte(pixelByteValue) ; // R
          // stream.WriteByte(255)            ; // A
        }
      ) ;
      return bitmap ;
    }

    public static Windows.UI.Xaml.Media.Imaging.WriteableBitmap 
    CreateWriteableBitmap_ForTesting_A ( 
      int width  = 256,
      int height = 256
    ) { 
      // Hmm, the only place the stream format seems to be documented (?!!) is in this example :
      // https://github.com/microsoftarchive/msdn-code-gallery-microsoft/blob/master/Official%20Windows%20Platform%20Sample/XAML%20images%20sample/%5BC%23%5D-XAML%20images%20sample/C%23/Scenario4.xaml.cs
      var imageBytes = new byte[width*height*4] ;
      int jFirstByteNotYetWritten = 0 ;
      Enumerable.Range(0,width).ForEachItem(
        x => {
          Enumerable.Range(0,height).ForEachItem(
            y => {
              var pixelValue = GetPixelValuesRGB(x,y) ;
              imageBytes[jFirstByteNotYetWritten++] = pixelValue.blue ; // B
              imageBytes[jFirstByteNotYetWritten++] = 0 ;               // G
              imageBytes[jFirstByteNotYetWritten++] = 0 ;               // R
              imageBytes[jFirstByteNotYetWritten++] = 255 ;             // A
            }
          ) ;
        }
      ) ;
      Windows.UI.Xaml.Media.Imaging.WriteableBitmap bitmap 
      = new Windows.UI.Xaml.Media.Imaging.WriteableBitmap(
        width,
        height
      ) ;
      using ( var stream = bitmap.PixelBuffer.AsStream() )
      {
        stream.Write(
          imageBytes,
          0,
          imageBytes.Length
        ) ;
      }
      return bitmap ;
      (byte red, byte green, byte blue) GetPixelValuesRGB ( int x, int y )
      {
        // Sin(x)/x around the centre
        int dx = x - width / 2 ;
        int dy = y - height / 2 ;
        double dxFrac01 = 2.0 * dx / width ;
        double dyFrac01 = 2.0 * dy / height ;
        double r = System.Math.Sqrt(
          dxFrac01 * dxFrac01
        + dyFrac01 * dyFrac01
        ) ;
        r *= 10.0 ;
        double h = System.Math.Abs(
          r == 0.0
          ? 1.0
          : System.Math.Sin(r) /  r
        ) ;
        byte greyValue = (byte) ( 
          255 
        * System.Math.Pow(h,2.0) 
        ) ;
        return (greyValue,greyValue,greyValue) ;
        // return (
        //   red   : (byte) ( x + y ),
        //   green : (byte) ( x + y ),
        //   blue  : (byte) ( x + y )
        // ) ;
      }
    }

    public static Windows.UI.Xaml.Media.Imaging.WriteableBitmap 
    CreateWriteableBitmap_ForTesting_B ( 
      int width  = 320,
      int height = 240
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
                byte pixelByteValue = (
                  // 127
                  GetPixelValue(x,y) 
                ) ;
                stream.WriteByte(0)              ; // B
                stream.WriteByte(0)              ; // G
                stream.WriteByte(pixelByteValue) ; // R
                stream.WriteByte(255)            ; // A
              }
            ) ;
          }
        ) ;
      }
      return bitmap ;
      byte GetPixelValue ( int x, int y )
      {
        return (byte) ( ( x + y ) % 256 ) ;
      }
    }

    public static Windows.UI.Xaml.Media.Imaging.WriteableBitmap 
    CreateWriteableBitmap_ForTesting_C ( 
      int width  = 320,
      int height = 240
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
                byte pixelByteValue = (
                  // 127
                  GetPixelValue(x,y) 
                ) ;
                stream.WriteByte(pixelByteValue) ; // B
                stream.WriteByte(0)              ; // G
                stream.WriteByte(0)              ; // R
                stream.WriteByte(255)            ; // A
              }
            ) ;
          }
        ) ;
      }
      return bitmap ;
      byte GetPixelValue ( int x, int y )
      {
        return (byte) ( ( x + y ) % 256 ) ;
      }
    }

    public static Windows.UI.Xaml.Media.Imaging.WriteableBitmap 
    CreateWriteableBitmap_ForTesting_D ( 
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
