//
// IProfileDisplaySettingsViewModel.cs
//

namespace IntensityMapViewer
{

  public interface IProfileDisplaySettingsViewModel : IViewModel
  {

    System.Drawing.Point ProfilePosition { get ; set ; }

    System.Collections.Generic.IReadOnlyList<byte> VerticalProfileIntensityValues   { get ; }
    System.Collections.Generic.IReadOnlyList<byte> HorizontalProfileIntensityValues { get ; }

    bool ShouldShowProfileGraphs { get ; set ; }

    // bool ShouldShowProfileGraphTickMarksAndCoordinates { get ; set ; } 

  }

}
