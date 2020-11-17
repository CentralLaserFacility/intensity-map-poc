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

    public string Name { get ; set ; }

    public IEnumerable<string> CameraMetadataTextLines => throw new System.NotImplementedException() ;

    public CommsStatus CommsStatus => throw new System.NotImplementedException() ;

    public bool IsReceivingImageUpdates {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public System.Drawing.Size SourceImageDimensions => throw new System.NotImplementedException() ;

    public ISourceSettingsViewModel SourceSettings => throw new System.NotImplementedException() ;

    public IIntensityMap? MostRecentlyAcquiredIntensityMap => throw new System.NotImplementedException() ;

    public event System.Action? NewIntensityMapAcquired ;

    public event System.Action? ProfileGraphsReferencePositionChanged ;

    public System.Drawing.Point ProfileGraphsReferencePosition {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public System.Drawing.Rectangle DisplayedRegion {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

  }

}
