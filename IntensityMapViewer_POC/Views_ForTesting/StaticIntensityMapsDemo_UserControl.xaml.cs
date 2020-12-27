//
// StaticIntensityMapsDemo_UserControl.cs
//

namespace Views_ForTesting
{

  public sealed partial class StaticIntensityMapsDemo_UserControl : Windows.UI.Xaml.Controls.UserControl
  {

    private IntensityMapViewer.StaticIntensityMapsDemo_ViewModel ViewModel
    { get ; } = new IntensityMapViewer.StaticIntensityMapsDemo_ViewModel() ;

    public StaticIntensityMapsDemo_UserControl ( )
    {
      this.InitializeComponent() ;
    }

  }

}
