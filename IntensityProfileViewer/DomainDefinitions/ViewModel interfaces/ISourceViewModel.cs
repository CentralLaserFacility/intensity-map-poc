//
// ISourceViewModel.cs
//

using System.Collections.Generic;

namespace IntensityMapViewer
{

  public interface ISourceViewModel : IViewModel
  {

    IDisplayPanelViewModel Parent { get ; }

    ISourceDescriptorViewModel SourceDescriptor { get ; }

    // We can adjust the gain, exposure-time and so on

    ISourceSettingsViewModel SourceSettings { get ; }

    IProfileDisplaySettingsViewModel ProfileDisplaySettings { get ; }

    // The source holds the most recently acquired IntensityMap

    IIntensityMap? MostRecentlyAcquiredIntensityMap { get ; }

    void SetRecentlyAcquiredIntensityMap ( IIntensityMap? intensityMap ) ;

    // Events ??? Better to raise events in a centralised 'hub' ???
    // Or just rely on the PropertyChanged mechanism ???

    event System.Action? NewIntensityMapAcquired ;

  }

}
