//
// IDisplayPanelViewModel.cs
//

namespace IntensityMapViewer
{

  //
  // This is the 'main' view model for the Viewer.
  //

  public interface IDisplayPanelViewModel : IViewModel
  {

    ISourceViewModel CurrentSource { get ; }

    IImagePresentationSettingsViewModel ImagePresentationSettings { get ; }

    IUserPreferencesViewModel UserPreferences { get ; }

    //
    // When this is 'true' the image panel updates itself to keep in sync with
    // the most recent image that has been sent to us by the camera. When 'false',
    // the image panel freezes. While it's frozen, the 'Save' button is enabled.
    //

    bool EnableRealTimeUpdates { get ; set ; }

  }

}
