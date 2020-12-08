//
// ISourceDescriptorViewModel.cs
//

using System.Collections.Generic;

namespace IntensityMapViewer
{

  //
  // Represents a potential 'source' of IntensityMap data.
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
    
  }

}
