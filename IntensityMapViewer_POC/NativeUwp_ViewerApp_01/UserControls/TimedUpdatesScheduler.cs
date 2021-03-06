//
// TimedUpdatesScheduler.cs
//

using System.Collections ;
using System.Collections.Generic ;
using System.Linq ;

namespace IntensityMapViewer
{

  //
  // This uses a timer to dynamically cycle through a set of pre-computed settings.
  //    

  public class TimedUpdatesScheduler 
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  , Common.IExpectsTimerWakeupNotifications
  {

    private double m_desiredWakeupPeriodMillisecs ;
    public double DesiredWakeupPeriodMillisecs 
    { 
      get => m_desiredWakeupPeriodMillisecs ;
      set {
        if (
          SetProperty(
            ref m_desiredWakeupPeriodMillisecs,
            value
          )
        ) {
          DesiredWakeupPeriodChanged?.Invoke() ;
          base.OnPropertyChanged(
            nameof(TimerPeriod_AsString)
          ) ;
          base.OnPropertyChanged(
            nameof(FramesPerSecond)
          ) ;
          base.OnPropertyChanged(
            nameof(FramesPerSecond_AsString)
          ) ;
        }
      }
    }

    public event System.Action? DesiredWakeupPeriodChanged ;

    public double TimerPeriodInMillisecs
    {
      get => DesiredWakeupPeriodMillisecs ;
      set => DesiredWakeupPeriodMillisecs = value ;
    }

    public double FramesPerSecond
    {
      // ( 1000.0 / 20mS ) ==> 50 fps
      get => 1000.0 / TimerPeriodInMillisecs ;
      // 50 fps ==> timer period of (1000/50) ==> 20mS 
      set => TimerPeriodInMillisecs = 1000.0 * ( 1.0 / value ) ;
    }

    // Hmm, pity that the x:Bind engine doesn't understand tuples ...
    // public (double Min,double Max) TimerPeriodValidRange => (20.0,2000.0) ;

    public double TimerPeriod_Min     { get ; } = 20.0 ;
    public double TimerPeriod_Max     { get ; } = 500.0 ;
    public double TimerPeriod_Default { get ; } = 100.0 ;

    public double FramesPerSecond_Max => 1000.0 / TimerPeriod_Min ; // 20mS => 50 fps
    public double FramesPerSecond_Min => 1000.0 / TimerPeriod_Max ; // 500mS => 2 fps

    public string TimerPeriod_AsString => $"Update requested every {TimerPeriodInMillisecs:F0}mS" ;

    public string FramesPerSecond_AsString => $"Frames per sec : {FramesPerSecond:F0}" ;

    public TimedUpdatesScheduler ( )
    {
      m_desiredWakeupPeriodMillisecs = TimerPeriod_Default ; 
      StartDynamicImageUpdates = new Microsoft.Toolkit.Mvvm.Input.RelayCommand(
        () => {
          m_performDynamicImageUpdates = true ;
          // base.IntensityMap = m_dynamicIntensityMapsSelector.GetCurrent_MoveNext() ;
          StartDynamicImageUpdates.NotifyCanExecuteChanged() ;
          StopDynamicImageUpdates.NotifyCanExecuteChanged() ;
        },
        () => m_performDynamicImageUpdates is false
      ) ;
      StopDynamicImageUpdates = new Microsoft.Toolkit.Mvvm.Input.RelayCommand(
        () => {
          m_performDynamicImageUpdates = false ;
          StartDynamicImageUpdates.NotifyCanExecuteChanged() ;
          StopDynamicImageUpdates.NotifyCanExecuteChanged() ;
        },
        () => m_performDynamicImageUpdates is true
      ) ;
      // base.IntensityMap = m_dynamicIntensityMapsSelector.GetCurrent_MoveNext() ;
      // base.IntensityMapLabel = "This will cycle through 60 variants" ;
      // base.ColourMapOption = ColourMapOption.JetColours ;
    }

    // private Common.CyclicSelector<IntensityMapViewer.IIntensityMap> m_dynamicIntensityMapsSelector = new(
    //   1 switch 
    //   {
    //   1 => IntensityMapViewer.IntensityMapSequence.CreateInstance_RippleRotatingAroundCircle(
    //       nIntensityMaps                   : 60,
    //       sincFactor                       : 10.0,
    //       fractionalRadialOffsetFromCentre : 0.2
    //     ).IntensityMaps,
    //   2 => IntensityMapViewer.IntensityMapSequence.CreateInstance_BlobRotatingAroundCircle(
    //       60
    //     ).IntensityMaps,
    //   _ => throw new System.ApplicationException()
    //   }
    // ) ;

    private bool m_performDynamicImageUpdates = false ;

    public Microsoft.Toolkit.Mvvm.Input.IRelayCommand StartDynamicImageUpdates { get ; }

    public Microsoft.Toolkit.Mvvm.Input.IRelayCommand StopDynamicImageUpdates { get ; }

    // When the timer fires, we replace the Dynamic image.

    // It's interesting to log the time taken to load a fresh image ...

    private List<long> m_bitmapLoadTimes = new () ;

    public string BitmapLoadTimes { get ; private set ; } = "" ;

    public string ActualTimerWakeupIntervals { get ; private set ; } = "" ;

    private List<long> m_actualTimerWakeupIntervals = new () ;

    private System.Diagnostics.Stopwatch m_timerTickStopwatch = new() ;

    private System.Diagnostics.Stopwatch m_timerTickReportStopwatch = new() ;

    public void OnWakeupNotification ( System.DateTime currentTime )
    {
      if ( m_timerTickStopwatch.IsRunning )
      {
        m_actualTimerWakeupIntervals.Add(
          m_timerTickStopwatch.ElapsedMilliseconds
        ) ;
        m_timerTickStopwatch.Reset() ;
      }
      else
      {
        m_timerTickStopwatch.Start() ;
        m_timerTickReportStopwatch.Start() ;
      }
      if ( m_timerTickReportStopwatch.ElapsedMilliseconds > 1000 )
      {
        ActualTimerWakeupIntervals = (
          "Actual wakeup intervals : "
        + string.Join(
            " ",
            m_actualTimerWakeupIntervals.OrderBy(
              time => time
            ).Select(
              time => time.ToString("F0")
            )
          )
        ) ;
        base.OnPropertyChanged(nameof(ActualTimerWakeupIntervals)) ;
        m_actualTimerWakeupIntervals.Clear() ;
        m_timerTickReportStopwatch.Restart() ;
      }
      if ( m_performDynamicImageUpdates )
      {
        System.Diagnostics.Stopwatch bitmapLoadingStopwatch = new() ;
        bitmapLoadingStopwatch.Start() ;
        // base.IntensityMap = m_dynamicIntensityMapsSelector.GetCurrent_MoveNext() ;
        m_bitmapLoadTimes.Add(
          bitmapLoadingStopwatch.ElapsedMilliseconds
        ) ;
        if ( m_bitmapLoadTimes.Count == 20 )
        {
          BitmapLoadTimes = (
            "Bitmap load times (mS) (ordered) : "
          + string.Join(
              " ",
              m_bitmapLoadTimes.OrderBy(
                time => time
              ).Select(
                time => time.ToString("F0")
              )
            )
          ) ;
          base.OnPropertyChanged(nameof(BitmapLoadTimes)) ;
          m_bitmapLoadTimes.Clear() ;
        }
      }
      // System.Diagnostics.Debug.WriteLine(
      //   $"LoadOrCreateWriteableBitmap took {elapsedMilliseconds} mS"
      // ) ;
      // If we set this to null, then a fresh PixelBuffer gets allocated for each Image
      // and memory usage increases until GC kicks in (every few seconds).
      // m_writeableBitmap = null ;
    }

  }

}
