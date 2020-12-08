//
// IntensityMapProfileDisplaySettings.cs
//

using System.Collections.Generic ;
using System.Linq ;

namespace IntensityMapViewer
{

  public class ProfileDisplaySettingsViewModel 
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  , IProfileDisplaySettingsViewModel
  {

    public event System.Action? ProfileGraphsReferencePositionChanged ;

    private System.Drawing.Point? m_profileGraphsReferencePosition ;

    public System.Drawing.Point? ProfileGraphsReferencePosition {
      get => m_profileGraphsReferencePosition ;
      set {
        if ( 
          SetProperty(
            ref m_profileGraphsReferencePosition,
            value
          )
        ) {
          ProfileGraphsReferencePositionChanged?.Invoke() ;
        }
      }
    }

    public System.Collections.Generic.IReadOnlyList<byte> VerticalProfileIntensityValues 
    => GetDummyProfileIntensityValues() ;

    public System.Collections.Generic.IReadOnlyList<byte> HorizontalProfileIntensityValues
    => GetDummyProfileIntensityValues() ;

    private bool m_shouldShowProfileGraphs = true ;

    public bool ShouldShowProfileGraphs {
      get => m_shouldShowProfileGraphs ;
      set => base.SetProperty(
        ref m_shouldShowProfileGraphs,
        value
      ) ;
    }

    private static System.Collections.Generic.IReadOnlyList<byte>? g_dummyProfileIntensityValues ;

    private static System.Collections.Generic.IReadOnlyList<byte> GetDummyProfileIntensityValues ( )
    => (
      g_dummyProfileIntensityValues 
      ??= Enumerable.Range(0,320).Select(
        i => (byte) ( i % 256 )
      ).ToList() 
    ) ;

    public SourceViewModel Parent { get ; }

    public ProfileDisplaySettingsViewModel ( SourceViewModel parent )
    {
      Parent = parent ;
    }

  }

}
