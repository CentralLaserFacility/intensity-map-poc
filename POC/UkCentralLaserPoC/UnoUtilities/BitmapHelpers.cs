//
// BitmapHelpers.cs
//

using System.Collections.Generic;
using System.Linq;

using Common.ExtensionMethods;
using static System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions;

namespace UwpUtilities
{

  //
  // To populate the pixels in a 'WriteableBitmap', you have to get its PixelBuffer
  // as a System.IO.Stream and then write 4-byte pixel values in the correct format.
  //
  //   Windows.UI.Xaml.Media.Imaging.WriteableBitmap bitmap = new (widthX,heightY) ;
  //   var pixelBuffer = bitmap.PixelBuffer ;
  //   using var stream = pixelBuffer.AsStream() ;
  //
  // Hmm, the only place the stream format seems to be documented (?!!) is in this example :
  // https://github.com/microsoftarchive/msdn-code-gallery-microsoft/blob/master/Official%20Windows%20Platform%20Sample/XAML%20images%20sample/%5BC%23%5D-XAML%20images%20sample/C%23/Scenario4.xaml.cs
  //
  // Bytes need to be written in the order B G R A :
  //
  //   stream.WriteByte(pixelByteValue_BLUE)  ; // B
  //   stream.WriteByte(pixelByteValue_GREEN) ; // G
  //   stream.WriteByte(pixelByteValue_RED)   ; // R
  //   stream.WriteByte(0xff)                 ; // A (most significant byte)
  //
  // It's more efficient to write packed 4-byte 'uint' values,
  // and this is best done with a BinaryWriter :
  //
  //   using var binaryWriter = new System.IO.BinaryWriter(stream) ;
  //
  //   uint encodedPixelValue_ARGB = EncodeARGB(r,g,b) ;
  //
  //   binaryWriter.Write(encodedPixelValue_ARGB) ;
  //
  // ... where the endoing is as follows :
  //
  //   public static uint EncodeARGB ( byte red, byte green, byte blue )
  //   => (uint) (
  //     ( 0xff  << 24 ) // A : most significant byte is 'alpha'
  //   | ( red   << 16 ) // R
  //   | ( green << 8  ) // G
  //   | ( blue  << 0  ) // B
  //   ) ;
  //

  public static class BitmapHelpers
  {

    //
    // This helper converts from the raw image data supplied by an IntensityMap
    // to a BitmapSource that will serve as the data source for an Image control. 
    //
    // Each byte of the image is mapped to an RGB colour.
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

    // RENAME => CreateImageSource_WriteableBitmap

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
      // TODO : Since this is performance-critical,
      // should profile it versus a raw 'for' loop ...
      Enumerable.Range(
        0,
        intensityMap.IntensityValues.Count
      ).ForEachItem(
        jPixel => {
          byte pixelByteValue = intensityMap.IntensityValues[jPixel] ;
          binaryWriter.Write(
            colourMapper.MapByteValueToEncodedARGB(pixelByteValue)
          ) ;
        }
      ) ;
      return bitmap ;
    }

    public static Windows.UI.Xaml.Media.Imaging.WriteableBitmap LoadOrCreateWriteableBitmap (
      ref Windows.UI.Xaml.Media.Imaging.WriteableBitmap? bitmap,
      IntensityMapViewer.IIntensityMap                   intensityMap,
      IntensityMapViewer.ColourMapOption                 colourMapOption = IntensityMapViewer.ColourMapOption.JetColours,
      byte?                                              highestIntensityValue = null
    ) { 
      if ( 
         bitmap?.PixelWidth  != intensityMap.Dimensions.Width
      || bitmap?.PixelHeight != intensityMap.Dimensions.Height
      ) {
        bitmap = null ;
      }
      bitmap ??= new Windows.UI.Xaml.Media.Imaging.WriteableBitmap(
        intensityMap.Dimensions.Width,
        intensityMap.Dimensions.Height
      ) ;
      double? gainFactorToApply = (
        (
           highestIntensityValue.HasValue 
        && intensityMap.MaximumIntensityValue != 0
        )
        ? (
            (double) highestIntensityValue
          / intensityMap.MaximumIntensityValue
          )
        : null
      ) ;
      var colourMapper = IntensityMapViewer.ColourMapper.InstanceFor(colourMapOption) ;
      var pixelBuffer = bitmap.PixelBuffer ;
      using var stream = pixelBuffer.AsStream() ;
      using var binaryWriter = new System.IO.BinaryWriter(stream) ;
      // TODO : Since this is performance-critical,
      // should profile LINQ versus a raw 'for' loop.
      // Also ... we start with the IReadOnlyList of intensity bytes,
      // and need to apply (A) a gain factor, then (B) the colour mapping
      // turning each byte into a 'uint', which is what actually gets written
      // to the stream via the BinaryWriter. What's the quickest way to apply
      // these transformations ??
      #if true
        int nPixels = intensityMap.IntensityValues.Count ;
        for ( int jPixel = 0 ; jPixel < nPixels ; jPixel++ )
        {
          byte pixelByteValue = intensityMap.IntensityValues[jPixel] ;
          if ( gainFactorToApply.HasValue )
          {
            pixelByteValue = (byte) (
              gainFactorToApply * pixelByteValue
            ) ;
          }
          binaryWriter.Write(
            colourMapper.MapByteValueToEncodedARGB(pixelByteValue)
          ) ;
        }
      #else
        Enumerable.Range(
          0,
          intensityMap.IntensityValues.Count
        ).ForEachItem(
          jPixel => {
            byte pixelByteValue = intensityMap.IntensityValues[jPixel] ;
            binaryWriter.Write(
              colourMapper.MapByteValueToEncodedARGB(pixelByteValue)
            ) ;
          }
        ) ;
      #endif
      // Enumerable.Range(
      //   0,
      //   intensityMap.IntensityValues.Count
      // ).ForEachItem(
      //   jPixel => {
      //     byte pixelByteValue = intensityMap.IntensityValues[jPixel] ;
      //     binaryWriter.Write(
      //       colourMapper.MapByteValueToEncodedARGB(pixelByteValue)
      //     ) ;
      //   }
      // ) ;
      bitmap.Invalidate() ;
      return bitmap ;
    }

  }

}
