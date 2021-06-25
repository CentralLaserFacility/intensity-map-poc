//
// ISourceViewModel.cs
//

using System.Collections.Generic;

namespace IntensityProfileViewer
{

  public interface ISourceViewModel 
  : IViewModel
  , IChildViewModel<IDisplayPanelViewModel>

  {

    // IDisplayPanelViewModel Parent { get ; }

    ISourceDescriptorViewModel SourceDescriptor { get ; }

    // We can adjust the gain, exposure-time and so on

    ISourceSettingsViewModel SourceSettings { get ; }

    IProfileDisplaySettingsViewModel ProfileDisplaySettings { get ; }

    // The source holds the most recently acquired IntensityMap

    IIntensityMap? MostRecentlyAcquiredIntensityMap { get ; }

    void SetRecentlyAcquiredIntensityMap ( IIntensityMap? intensityMap ) ;

    // Events ??? Better to raise events in a centralised 'hub' ???
    // Or just rely on the PropertyChanged mechanism ???
    // Use Messenger instead ?
    event System.Action? NewIntensityMapAcquired ;

  }

}
