//
// ISourceChoicesViewModel.cs
//

using System.Collections.Generic ;

namespace IntensityMapViewer
{

  public interface ISourceChoicesViewModel : IViewModel
  {

    IEnumerable<ISourceDescriptorViewModel> AllAvailableIntensityMapSourceDescriptors { get ; }

    ISourceDescriptorViewModel? CurrentlySelectedIntensityMapSource { get ; set ; }

  }

}
