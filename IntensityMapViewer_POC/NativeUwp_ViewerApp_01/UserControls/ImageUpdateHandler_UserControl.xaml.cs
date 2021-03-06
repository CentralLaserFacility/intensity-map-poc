using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace NativeUwp_ViewerApp_01
{

  public sealed partial class ImageUpdateHandler_UserControl : UserControl
  {

    public IntensityMapViewer.NumericValueViewModel UpdateRateViewModel { get ; set ; } 

    private Windows.UI.Xaml.DispatcherTimer m_timer ;
    
    // System.Threading.Timer m_timer ;

    private IntensityMapViewer.TimedUpdatesScheduler m_timedUpdatesScheduler ;

    public ImageUpdateHandler_UserControl ( )
    {
      this.InitializeComponent() ;
      m_updateRateEditor.ViewModel = UpdateRateViewModel = new(){
        MinValue = 2.0,
        MaxValue = 50.0,
        NSteps   = 20,
        ValueChanged = (value)=> {
          m_timedUpdatesScheduler.FramesPerSecond = value ;
        }
      } ;
      m_timer = new Windows.UI.Xaml.DispatcherTimer(){
        Interval = System.TimeSpan.FromMilliseconds(
          m_timedUpdatesScheduler.DesiredWakeupPeriodMillisecs
        )
      } ;
      m_timedUpdatesScheduler.DesiredWakeupPeriodChanged += () => {
        m_timer.Interval = System.TimeSpan.FromMilliseconds(
          m_timedUpdatesScheduler.DesiredWakeupPeriodMillisecs
        ) ;
      } ;
      m_timer.Tick += (s,e) => {
        m_timedUpdatesScheduler.OnWakeupNotification(
          System.DateTime.Now
        ) ;
      } ;
      m_timer.Start() ;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {

    }

  }

}
