//
// IProfileDisplaySettingsViewModel.cs
//

namespace IntensityMapViewer
{

  public interface IProfileDisplaySettingsViewModel : IViewModel
  {

    System.Drawing.Point? ProfileGraphsReferencePosition { get ; set ; }

    event System.Action? ProfileGraphsReferencePositionChanged ;

    System.Collections.Generic.IReadOnlyList<byte> VerticalProfileIntensityValues   { get ; }

    System.Collections.Generic.IReadOnlyList<byte> HorizontalProfileIntensityValues { get ; }

    bool ShouldShowProfileGraphs { get ; set ; }

  }

}
