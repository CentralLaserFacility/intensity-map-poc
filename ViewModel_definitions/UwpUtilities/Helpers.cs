//
// Helpers.cs
//

namespace UwpUtilities
{

  public static class Helpers
  {

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
      // Use a WriteableBitmap whose PixelBuffer will be populated with the image data ?
      throw new System.NotImplementedException() ;
    }

  }

}
