//
// IProfileDisplaySettingsViewModel.cs
//

namespace IntensityProfileViewer
{

  public interface IProfileDisplaySettingsViewModel : IViewModel
  {

    ISourceViewModel Parent { get ; }

    System.Drawing.Point? ProfileGraphsReferencePosition { get ; set ; }

    // Use Mediator instead ?
    event System.Action? ProfileGraphsReferencePositionChanged ;

    System.Collections.Generic.IReadOnlyList<byte> VerticalProfileIntensityValues   { get ; }

    System.Collections.Generic.IReadOnlyList<byte> HorizontalProfileIntensityValues { get ; }

    bool ShouldShowProfileGraphs { get ; set ; }

  }

}
