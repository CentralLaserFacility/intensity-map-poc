//
// DynamicIntensityMapsDemo_UserControl.xaml.cs
//

namespace Views_ForTesting
{

  public sealed partial class DynamicIntensityMapsDemo_UserControl : Windows.UI.Xaml.Controls.UserControl
  {

    private IntensityMapViewer.DynamicIntensityMapsDemo_ViewModel ViewModel
    { get ; } = new IntensityMapViewer.DynamicIntensityMapsDemo_ViewModel() ;

    private Windows.UI.Xaml.DispatcherTimer m_timer ;

    public DynamicIntensityMapsDemo_UserControl ( )
    {
      this.InitializeComponent() ;
      m_timer = new Windows.UI.Xaml.DispatcherTimer(){
        Interval = System.TimeSpan.FromMilliseconds(
          ViewModel.DesiredWakeupPeriodMillisecs
        )
      } ;
      ViewModel.DesiredWakeupPeriodChanged += () => {
        m_timer.Interval = System.TimeSpan.FromMilliseconds(
          ViewModel.DesiredWakeupPeriodMillisecs
        ) ;
      } ;
      m_timer.Tick += (s,e) => {
        ViewModel.OnWakeupNotification(
          System.DateTime.Now
        ) ;
      } ;
      m_timer.Start() ;
    }

  }

}
