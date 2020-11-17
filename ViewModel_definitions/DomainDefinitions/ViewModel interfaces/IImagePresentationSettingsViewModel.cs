//
// IImagePresentationSettingsViewModel.cs
//

namespace IntensityMapViewer
{

  public interface IImagePresentationSettingsViewModel : IViewModel
  {

    ColourMapOption ColourMapOption { get ; set ; }

    NormalisationMode NormalisationMode { get ; set ; }

    //
    // The 'NormalisationValue' is settable via UI elements 
    // but only if if we're in 'Manual' mode.
    //
    // In 'Automatic' mode the UI indicates the value, but it is 
    // obtained from the current ImageMap and can't be adjusted.
    //
    // Attempting to 'set' the NormalisationValue in Automatic mode raises an exception.
    //

    byte NormalisationValue { get ; set ; }

  }

}
