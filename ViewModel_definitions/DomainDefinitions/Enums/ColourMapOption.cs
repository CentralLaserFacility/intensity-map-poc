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
    Binary // As in LabView ; hmm, more like 'cyclic' ??
  }

}
