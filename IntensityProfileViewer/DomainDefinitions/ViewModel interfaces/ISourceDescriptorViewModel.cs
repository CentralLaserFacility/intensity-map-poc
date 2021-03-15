//
// ISourceDescriptorViewModel.cs
//

using System.Collections.Generic;

namespace IntensityProfileViewer
{

  //
  // Represents a potential 'source' of IntensityMap data.
  //
  // In a future version of the IntensityMapViewer, we'll be presenting
  // a list of available Sources that the user can choose from.
  // We'll display just the Descriptors, and once a source has been
  // chosen, we'll create a Source. But it's perfectly possible
  // in that scenario for a 'descriptor' to have a null Parent.
  //

  public interface ISourceDescriptorViewModel : IViewModel
  {

    string SourceName { get ; }

    // Information published by the camera that's capturing the image,
    // eg description, model number, serial number (all fixed),
    // and status info (might change occasionally)

    IEnumerable<string> CameraMetadataTextLines { get ; }

    // Is the connection to the source 'alive' ?

    CommsStatus? CommsStatus { get ; }

    ISourceViewModel? Parent { get ; } 
    
  }

}
