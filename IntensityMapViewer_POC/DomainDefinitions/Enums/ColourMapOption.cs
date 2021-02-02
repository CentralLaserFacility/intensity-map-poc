//
// ColourMapOption.cs
//

namespace IntensityMapViewer
{

  //
  // The image can be displayed either as greyscale, or with the pixel intensities
  // mapped to a Colour. A typical option is the 'JET' scheme ...
  //
  //   https://stackoverflow.com/questions/7706339/grayscale-to-red-green-blue-matlab-jet-color-scale
  // 
  // We may want to offer a wider set of colour mapping options,
  // so the View should provide a ComboBox to define the selected option.
  //

  public enum ColourMapOption {
    GreyScale,
    JetColours,
    // Useful for testing. By mapping a byte value
    // to a shade of red/blue/green we can confirm
    // visually that our encoding of RGB is correct.
    ShadesOfRed,
    ShadesOfGreen,
    ShadesOfBlue,
    // As in LabView ; hmm, more like 'cyclic' ??
    Binary 
  }

  public static class EnumTypeOptions
  {

    static System.Collections.Generic.List<Common.EnumItemsSource<ColourMapOption>> ColourMapOptions
    => Common.EnumItemsSource<ColourMapOption>.ToList() ;

  }

}
