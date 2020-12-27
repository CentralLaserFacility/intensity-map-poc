//
// BitmapLoadTimingTestPage.cs
//

namespace IntensityMapViewer
{

  public sealed partial class BitmapLoadTimingTestPage : Windows.UI.Xaml.Controls.Page
  {

    private IntensityMapViewer.BitmapLoadTimingTestViewModel ViewModel
    => NativeUwp_TestHarnessApp.ViewModels.ViewModelLocator.Current.BitmapLoadTimingTestViewModel ; 

    public BitmapLoadTimingTestPage ( )
    {
      this.InitializeComponent() ;
    }

  }

}
