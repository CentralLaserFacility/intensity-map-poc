//
// DisplayPanelViewModel.cs
//

namespace IntensityMapViewer
{

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

    public DisplayPanelViewModel ( )
    {
      CurrentSource             = new SourceViewModel(this) ;
      ImagePresentationSettings = new ImagePresentationSettingsViewModel(this) ;
      UserPreferences           = new UserPreferencesViewModel() ;
    }

  }

}
