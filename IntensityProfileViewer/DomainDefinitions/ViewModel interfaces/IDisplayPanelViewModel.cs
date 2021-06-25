//
// IDisplayPanelViewModel.cs
//

namespace IntensityProfileViewer
{

  //
  // This is the 'main' or 'root' view model for the Viewer.
  //

  public interface IDisplayPanelViewModel : IViewModel
  {

    ISourceViewModel CurrentSource { get ; }

    IImagePresentationSettingsViewModel ImagePresentationSettings { get ; }

    IUserPreferencesViewModel UserPreferences { get ; }

    // This acts as a central 'hub' for pan/zoom changes

    PanAndZoomParameters PanAndZoomParameters { get ; } 

    // Use Messenger instead ?
    event System.Action? IntensityMapVisualisationHasChanged ;
    void RaiseIntensityMapVisualisationHasChangedEvent ( ) ;

    //
    // When this is 'true' the image panel updates itself to keep in sync with
    // the most recent image that has been sent to us by the camera. When 'false',
    // the image panel freezes. While it's frozen, the 'Save' button is enabled.
    //

    bool EnableRealTimeUpdates { get ; set ; }

  }

}
