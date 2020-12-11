//
// SourceViewModel.cs
//

using System.Collections.Generic ;

namespace IntensityMapViewer
{

  public class SourceViewModel 
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  , ISourceViewModel
  {

    public ISourceDescriptorViewModel SourceDescriptor { get ; }

    public ISourceSettingsViewModel SourceSettings { get ; } 

    private IIntensityMap? m_mostRecentlyAcquiredIntensityMap = null ;

    public IIntensityMap? MostRecentlyAcquiredIntensityMap => m_mostRecentlyAcquiredIntensityMap ;

    public void SetRecentlyAcquiredIntensityMap ( IIntensityMap? intensityMap ) 
    {
      if (
        SetProperty(
          ref m_mostRecentlyAcquiredIntensityMap,
          intensityMap,
          nameof(MostRecentlyAcquiredIntensityMap)
        )
      ) {
        NewIntensityMapAcquired?.Invoke() ;
      }
    }

    public event System.Action? NewIntensityMapAcquired ;

    public IProfileDisplaySettingsViewModel ProfileDisplaySettings { get ; }

    public IDisplayPanelViewModel Parent { get ; }

    public SourceViewModel ( IDisplayPanelViewModel parent )
    {
      Parent = parent ;
      // Create the 'child' view models, which are always present
      SourceDescriptor       = new SourceDescriptorViewModel(this) ;
      SourceSettings         = new SourceSettingsViewModel(this) ;
      ProfileDisplaySettings = new ProfileDisplaySettingsViewModel(this) ;
      // Create an initial IntensityMap to be displayed
      m_mostRecentlyAcquiredIntensityMap = IntensityMapHelpers.CreateSynthetic_UsingSincFunction() ;
    }

  }

}
