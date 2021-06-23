//
// DisplayPanelViewModel.cs
//

using System.Runtime.CompilerServices;

namespace IntensityProfileViewer
{

  //
  // The 'View' for this will be a UserControl
  // packaged as a component in its own DLL.
  //

  // Hmm, rename to IntensityProfileViewerRootViewModel ??
  // or just 'RootViewModel' as we're in the IntensityProfileViewer namesapce.

  public class DisplayPanelViewModel 
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableRecipient
  , IDisplayPanelViewModel
  {

    public ISourceViewModel CurrentSource { get ; }

    public IImagePresentationSettingsViewModel ImagePresentationSettings { get ; } 

    public PanAndZoomParameters PanAndZoomParameters { get ; } = new PanAndZoomParameters() ;

    //
    // MATTEO : this is an example of a place where it'd be better to use Mediator.
    // Instead of raising this event (to which another ViewModel would have subscribed)
    // we'll raise an event on the singleton 'Mediator' at the root level, and any
    // interested ViewModel could subscribe to the message. That has the advantage of
    // using the WeakEvent pattern, so we needn't rely on unsubscription.
    //

    // Use Mediator instead ?
    public event System.Action? IntensityMapVisualisationHasChanged ;

    public void RaiseIntensityMapVisualisationHasChangedEvent ( )
    => IntensityMapVisualisationHasChanged?.Invoke() ;

    public IUserPreferencesViewModel UserPreferences { get ; }

    private bool m_enableRealTimeUpdates ;

    public bool EnableRealTimeUpdates {
      get => m_enableRealTimeUpdates ;
      set => SetProperty(
        ref m_enableRealTimeUpdates,
        value,
        broadcast : true // Broadcasts a PropertyChangedMessage<T> ie providing (oldValue,newValue,propertyName)
      ) ;
    }

    //
    // This constructor creates instances of all its 'child'
    // ViewModels, and those in turn create their own children.
    //

    // public Microsoft.Toolkit.Mvvm.Messaging.IMessenger Messenger => base.Messenger ;

    //
    // In order to support multiple instances of the Viewer, each with independent message handling,
    // we'll want to pass in a specific instance of an IMessenger that will be used by all nested ViewModels.
    // For the time being we can live with just the single default instance ...
    //

    public DisplayPanelViewModel (
      Microsoft.Toolkit.Mvvm.Messaging.IMessenger? messenger = null
    ) : 
    base(
      messenger ?? Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger.Default
    ) {
      // Hmm, should use Dependency Injection here !
      // But this hard-wired approach seems appropriate for the POC.
      CurrentSource             = new SourceViewModel(this) ;
      ImagePresentationSettings = new ImagePresentationSettingsViewModel(this) ;
      UserPreferences           = new UserPreferencesViewModel(this) ;
    }

  }

}
