//
// SourceDescriptorViewModel.cs
//

using System.Collections.Generic ;

namespace IntensityMapViewer
{

  public class SourceDescriptorViewModel 
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  , ISourceDescriptorViewModel
  {

    public string SourceName { get ; set ; } = "Simulated source" ;

    public IEnumerable<string> CameraMetadataTextLines => new []{
      "Just a simulation",
      "But nice all the same"
    } ;

    private CommsStatus? m_commsStatus = IntensityMapViewer.CommsStatus.Connected ;

    public CommsStatus? CommsStatus => m_commsStatus ;

    // Not mentioned in the 'ISourceDescriptorViewModel',
    // because setting the status would just be a simulation, for the POC

    public void SetCommsStatus ( IntensityMapViewer.CommsStatus status )
    => SetProperty(
      ref m_commsStatus,
      status,
      nameof(CommsStatus)
    ) ;

    public ISourceViewModel? Parent { get ; } 
    
    public SourceDescriptorViewModel ( ISourceViewModel? parent = null )
    {
      Parent = parent ;
    }

  }

}
