//
// IDisplayPanelViewModel.cs
//

namespace IntensityMapViewer
{

  //
  // This is the 'main' view model.
  //

  public interface IDisplayPanelViewModel : IViewModel
  {

    // This is NOT part of the main panel, we should 
    // keep it as a separate UI element eg a tree view or list box
    // ISourceChoicesViewModel IntensityMapSourceChoices { get ; }

    IIntensityMap? IntensityMapBeingDisplayed { get ; }

    ISourceDescriptorViewModel? SourceBeingDisplayed { get ; set ; }

    IImagePresentationSettingsViewModel ImagePresentationSettings { get ; }

    IProfileDisplaySettingsViewModel ProfileSettings { get ; }

    //
    // When this is 'true' the image panel updates itself to keep in sync with
    // the most recent image that has been sent to us by the camera. When 'false',
    // the image panel freezes. While it's frozen, the 'Save' button is enabled.
    //

    bool EnableRealTimeUpdates { get ; set ; }

    // Commands

    System.Windows.Input.ICommand ShowIntensityMapSourceSettingsPanel { get ; }

    System.Windows.Input.ICommand SaveCurrentImage { get ; }

    // Miscellaneous 'simple' properties

    double? AverageUpdateRate_FramesPerSecond { get ; }

    //
    // If the comms channel to the current Source is disconnected, 
    // we indicate that on the UI eg by displaying a coloured border.
    //

    CommsStatus? CommsStatus { get ; }

  }

}
