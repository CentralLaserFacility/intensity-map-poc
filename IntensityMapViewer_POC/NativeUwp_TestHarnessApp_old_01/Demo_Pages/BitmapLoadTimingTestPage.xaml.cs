//
// IntensityMapTestPage.cs
//

namespace IntensityMapViewer
{

  public sealed partial class BitmapLoadTimingTestPage : Windows.UI.Xaml.Controls.Page
  {

    private IntensityMapViewer.BitmapLoadTimingDemoViewModel ViewModel
    => NativeUwp_TestHarnessApp.ViewModels.ViewModelLocator.Current.BitmapLoadTimingDemoViewModel ; 

    public BitmapLoadTimingTestPage ( )
    {
      this.InitializeComponent() ;
    }

  }

}
