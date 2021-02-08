//
// IImagePresentationSettingsViewModel.cs
//

namespace IntensityMapViewer
{

  //
  // An editable implementation is not necessarily required for the POC, 
  // but would be nice to have, demonstrating best practice for connecting
  // UI elements to enums and numeric values.
  //

  public interface IImagePresentationSettingsViewModel : IViewModel
  {

    // Nasty hack in an attempt to make ComboBox binding work ...

    string ColourMapOptionName { get ; set ; }

    System.Collections.Generic.IEnumerable<string> ColourMapOptionNames { get ; }

    // ----------------------

    IDisplayPanelViewModel Parent { get ; }

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

    byte NormalisationValue { get ; }

    bool CanSetNormalisationValue { get ; }

    void SetNormalisationValue ( byte value ) ;

  }

}
