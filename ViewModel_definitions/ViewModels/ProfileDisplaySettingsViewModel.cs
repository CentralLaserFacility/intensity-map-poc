//
// IntensityMapProfileDisplaySettings.cs
//

namespace IntensityMapViewer
{

  public class ProfileDisplaySettingsViewModel 
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  , IProfileDisplaySettingsViewModel
  {

    public System.Drawing.Point ProfilePosition {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public System.Collections.Generic.IReadOnlyList<byte> VerticalProfileIntensityValues   => throw new System.NotImplementedException() ;

    public System.Collections.Generic.IReadOnlyList<byte> HorizontalProfileIntensityValues => throw new System.NotImplementedException() ;

    public bool ShouldShowProfileGraphs {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

  }

}
