//
// DisplayPanelViewModel.cs
//

namespace IntensityMapViewer
{

  //
  // The 'View' for this will be a UserControl
  // packaged as a component in it own DLL.
  //

  public class DisplayPanelViewModel 
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  , IDisplayPanelViewModel
  {

    public ISourceViewModel CurrentSource { get ; }

    public IImagePresentationSettingsViewModel ImagePresentationSettings { get ; } 

    public IUserPreferencesViewModel UserPreferences { get ; }

    private bool m_enableRealTimeUpdates ;

    public bool EnableRealTimeUpdates {
      get => m_enableRealTimeUpdates ;
      set => base.SetProperty(
        ref m_enableRealTimeUpdates,
        value
      ) ;
    }

    //
    // This constructor creates instances of all its 'child'
    // ViewModels, and those in turn create their own children.
    //

    public DisplayPanelViewModel ( )
    {
      CurrentSource             = new SourceViewModel(this) ;
      ImagePresentationSettings = new ImagePresentationSettingsViewModel(this) ;
      UserPreferences           = new UserPreferencesViewModel(this) ;
    }

  }

}
