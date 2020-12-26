//
// IntensityMapTestPage.cs
//

namespace IntensityMapViewer
{

  public sealed partial class IntensityMapTestPage : Windows.UI.Xaml.Controls.Page
  {

    private IntensityMapViewer.IntensityMapTestViewModel ViewModel
    => NativeUwp_TestHarnessApp.ViewModels.ViewModelLocator.Current.IntensityMapTestViewModel ; 

    public IntensityMapTestPage ( )
    {
      this.InitializeComponent() ;
    }

  }

}
