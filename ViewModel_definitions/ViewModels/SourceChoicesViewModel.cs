//
// SourceChoices.cs
//

using System.Collections.Generic ;

namespace IntensityMapViewer
{

  //
  // In a 'real' implementation we'd build our list of 'available sources'
  // by scanning the network (??) and seeing what cameras are available.
  //
  // Here we'll just populate our list with some fake data.
  //

  public partial class SourceChoicesViewModel 
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  , ISourceChoicesViewModel
  {

    public IEnumerable<ISourceDescriptorViewModel> AllAvailableIntensityMapSourceDescriptors => throw new System.NotImplementedException() ;

    public ISourceDescriptorViewModel? CurrentlySelectedIntensityMapSource {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

  }

  // Implementation details

  public partial class SourceChoicesViewModel 
  {

    private SourceDescriptorViewModel[] m_sourceDescriptorViewModels = new[]{
      new SourceDescriptorViewModel(){
        Name = "FirstCamera"
      },
      new SourceDescriptorViewModel(){
        Name = "SecondCamera"
      },
    } ;

    public SourceChoicesViewModel ( ) => throw new System.NotImplementedException() ;

  }


}
