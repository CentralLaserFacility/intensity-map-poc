//
// IntensityMap.cs
//

using System.Collections.Generic ;
using System.Linq ;

namespace IntensityMapViewer
{

  //
  // Hmm, got a feeling this will be useful. We'll need to react
  // to pan and zoom gestures arising from either the Image Panel
  // or the two Profile Graphs. The idea would be that pan/zoom adjustements
  // get submitted to our Coordinator, and broadcast to all three of
  // the interested Views.
  //

  public partial class PanAndZoomCoordinator
  {

    public event System.Action? PanAndZoomParametersChanged ;

    public float ZoomFactor {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public float PanOffset_X {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public float PanOffset_Y {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

  }

}
