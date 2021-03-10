//
// TimedUpdatesScheduler.cs
//

using System.Collections ;
using System.Collections.Generic ;
using System.Linq ;

namespace IntensityMapViewer
{

  //
  // This uses an externally supplied timer to regularly invoke an 'update' action,
  // eg to dynamically cycle through a set of pre-computed settings.
  //    
  // The scheduler maintains a property that represents the desired 'wakeup period',
  // together with min and max values. The desired wakeup period can be set between 
  // those limits, either in terms of milliseconds or as frames-per-second.
  //
  // The external client is expected to invoke the 'wakeup' function at 
  // the regular intervals indicated by the DesiredWakeupPeriod property.
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

    private readonly System.Action m_updateActionToBePerformed ;

    public TimedUpdatesScheduler ( System.Action updateActionToBePerformed )
    {
      m_updateActionToBePerformed = updateActionToBePerformed ;
      m_desiredWakeupPeriodMillisecs = TimerPeriod_Default ; 
      StartTimedUpdates = new Microsoft.Toolkit.Mvvm.Input.RelayCommand(
        () => {
          m_enableTimedUpdates = true ;
          // base.IntensityMap = m_dynamicIntensityMapsSelector.GetCurrent_MoveNext() ;
          StartTimedUpdates.NotifyCanExecuteChanged() ;
          StopTimedUpdates.NotifyCanExecuteChanged() ;
        },
        () => m_enableTimedUpdates is false
      ) ;
      StopTimedUpdates = new Microsoft.Toolkit.Mvvm.Input.RelayCommand(
        () => {
          m_enableTimedUpdates = false ;
          StartTimedUpdates.NotifyCanExecuteChanged() ;
          StopTimedUpdates.NotifyCanExecuteChanged() ;
        },
        () => m_enableTimedUpdates is true
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

    private bool m_enableTimedUpdates = false ;

    public Microsoft.Toolkit.Mvvm.Input.IRelayCommand StartTimedUpdates { get ; }

    public Microsoft.Toolkit.Mvvm.Input.IRelayCommand StopTimedUpdates { get ; }

    // It can be interesting to log the time taken
    // to perform the operation we're invoking.

    private List<long> m_updateExecutionTimes = new () ;

    public string UpdateExecutionTimes { get ; private set ; } = "" ;

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
      if ( m_enableTimedUpdates )
      {
        System.Diagnostics.Stopwatch executionTimingStopwatch = new() ;
        executionTimingStopwatch.Start() ;
        m_updateActionToBePerformed() ;
        m_updateExecutionTimes.Add(
          executionTimingStopwatch.ElapsedMilliseconds
        ) ;
        if ( m_updateExecutionTimes.Count == 20 )
        {
          UpdateExecutionTimes = (
            "Action execution times (mS) (ordered) : "
          + string.Join(
              " ",
              m_updateExecutionTimes.OrderBy(
                time => time
              ).Select(
                time => time.ToString("F0")
              )
            )
          ) ;
          base.OnPropertyChanged(nameof(UpdateExecutionTimes)) ;
          m_updateExecutionTimes.Clear() ;
        }
      }
    }

  }

}
