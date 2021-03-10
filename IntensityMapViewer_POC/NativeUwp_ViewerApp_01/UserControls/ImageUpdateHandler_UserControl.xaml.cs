using IntensityMapViewer;
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

    // public IntensityMapViewer.NumericValueViewModel UpdateRateViewModel { get ; set ; } 
    // 
    // public IntensityMapViewer.NumericValueViewModel UpdatePeriodViewModel { get ; set ; } 

    private Windows.UI.Xaml.DispatcherTimer m_timer ;
    
    private IntensityMapViewer.TimedUpdatesScheduler TimedUpdatesScheduler ;

    public static int SequenceType = 1 ;

    private Common.CyclicSelector<IntensityMapViewer.IIntensityMap> m_dynamicIntensityMapsSelector = new(
      SequenceType switch 
      {
      1 => IntensityMapViewer.IntensityMapSequence.CreateInstance_RippleRotatingAroundCircle(
          nIntensityMaps                   : 60,
          sincFactor                       : 10.0,
          fractionalRadialOffsetFromCentre : 0.2
        ).IntensityMaps,
      2 => IntensityMapViewer.IntensityMapSequence.CreateInstance_BlobRotatingAroundCircle(
          60
        ).IntensityMaps,
      _ => throw new System.ApplicationException()
      }
    ) ;

    public IIntensityMap CurrentIntensityMap { get ; private set ; }

    public System.Action? CurrentIntensityMapChanged ;

    public ImageUpdateHandler_UserControl ( )
    {
      this.InitializeComponent() ;
      TimedUpdatesScheduler = new IntensityMapViewer.TimedUpdatesScheduler(
        () => {
          CurrentIntensityMap = m_dynamicIntensityMapsSelector.GetCurrent_MoveNext() ;
          CurrentIntensityMapChanged?.Invoke() ;
        }
      ) ;
      // m_updateRateEditor.ViewModel = UpdateRateViewModel = new(){
      //   MinValue = 2.0,
      //   MaxValue = 50.0,
      //   NSteps   = 20,
      //   ValueChanged = (value)=> {
      //     TimedUpdatesScheduler.FramesPerSecond = value ;
      //   }
      // } ;
      m_timer = new Windows.UI.Xaml.DispatcherTimer(){
        Interval = System.TimeSpan.FromMilliseconds(
          TimedUpdatesScheduler.DesiredWakeupPeriodMillisecs
        )
      } ;
      TimedUpdatesScheduler.DesiredWakeupPeriodChanged += () => {
        m_timer.Interval = System.TimeSpan.FromMilliseconds(
          TimedUpdatesScheduler.DesiredWakeupPeriodMillisecs
        ) ;
      } ;
      m_timer.Tick += (s,e) => {
        TimedUpdatesScheduler.OnWakeupNotification(
          System.DateTime.Now
        ) ;
      } ;
      m_timer.Start() ;
    }

    private void Button_Click ( object sender, RoutedEventArgs e )
    {

    }

  }

}
