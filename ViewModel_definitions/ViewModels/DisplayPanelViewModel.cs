//
// DisplayPanelViewModel.cs
//

namespace IntensityMapViewer
{

  public class DisplayPanelViewModel 
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  , IDisplayPanelViewModel
  {

    public IIntensityMap? IntensityMapBeingDisplayed => throw new System.NotImplementedException() ;

    public ISourceDescriptorViewModel? SourceBeingDisplayed {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public IImagePresentationSettingsViewModel ImagePresentationSettings => throw new System.NotImplementedException() ;

    public IProfileDisplaySettingsViewModel ProfileSettings => throw new System.NotImplementedException() ;

    public bool EnableRealTimeUpdates {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    // Commands

    public System.Windows.Input.ICommand ShowIntensityMapSourceSettingsPanel => throw new System.NotImplementedException() ;

    public System.Windows.Input.ICommand SaveCurrentImage => throw new System.NotImplementedException() ;

    // Miscellaneous 'simple' properties

    public double? AverageUpdateRate_FramesPerSecond => throw new System.NotImplementedException() ;

    //
    // If the comms channel to the current Source is disconnected, 
    // we indicate that on the UI eg by displaying a coloured border.
    //

    public CommsStatus? CommsStatus => throw new System.NotImplementedException() ;

  }

}
