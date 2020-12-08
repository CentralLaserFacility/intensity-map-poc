//
// SourceViewModel.cs
//

using System.Collections.Generic ;

namespace IntensityMapViewer
{

  public class SourceViewModel 
  : SourceDescriptorViewModel
  , ISourceViewModel
  {

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

    public DisplayPanelViewModel Parent { get ; }

    public SourceViewModel ( DisplayPanelViewModel parent )
    {
      Parent = parent ;
      SourceSettings = new SourceSettingsViewModel(this) ;
      ProfileDisplaySettings = new ProfileDisplaySettingsViewModel(this) ;
      m_mostRecentlyAcquiredIntensityMap = IntensityMap.CreateSynthetic_UsingSincFunction() ;
    }

  }

}
