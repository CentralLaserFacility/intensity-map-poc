//
// DemoViewModel_CyclingThroughStaticIntensityMaps.cs
//

namespace NativeUwp_TestHarnessApp
{

    public class DemoViewModel_CyclingThroughStaticIntensityMaps : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
    {

      public string Title => "This demonstrates cycling through static Intensity Maps" ;

      public IntensityMapViewer.StaticIntensityMapsDemo_ViewModel
      Demo_ViewModel { get ; } = new IntensityMapViewer.StaticIntensityMapsDemo_ViewModel() ;

    }

}
