//
// IntensityMapTestViewModel.cs
//

using System.Windows.Input;

using System.Collections.Generic ;
using System.Linq ;
using System.Runtime.InteropServices.WindowsRuntime;

namespace IntensityMapViewer
{

  public class BitmapLoadTimingDemoViewModel : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  {

    private Windows.UI.Xaml.DispatcherTimer m_timer = new() ;

    private double m_timerPeriodInMillisecs ;
    public double TimerPeriodInMillisecs
    {
      get => m_timerPeriodInMillisecs ;
      set {
        if (
          SetProperty(
            ref m_timerPeriodInMillisecs,
            value
          )
        ) {
          m_timer.Interval = System.TimeSpan.FromMilliseconds(m_timerPeriodInMillisecs) ;
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

    private bool m_performDynamicImageUpdates = false ;

    public BitmapLoadTimingDemoViewModel ( )
    {
      m_timerPeriodInMillisecs = TimerPeriod_Default ;
      m_timer.Interval = System.TimeSpan.FromMilliseconds(100) ;
      m_timer.Tick += TimerTick ;
      m_timer.Start() ;
      StartDynamicImageUpdates = new Microsoft.Toolkit.Mvvm.Input.RelayCommand(
        () => {
          m_performDynamicImageUpdates = true ;
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
    }

    private Windows.UI.Xaml.Media.ImageSource m_dynamicImageSource ;
    public Windows.UI.Xaml.Media.ImageSource DynamicImageSource 
    {
      get => m_dynamicImageSource ;
      set => base.SetProperty(
        ref m_dynamicImageSource,
        value
      ) ;
    }

    public Microsoft.Toolkit.Mvvm.Input.IRelayCommand StartDynamicImageUpdates { get ; }

    public Microsoft.Toolkit.Mvvm.Input.IRelayCommand StopDynamicImageUpdates { get ; }

    // When the timer fires, we replace the Dynamic image.

    // By re-using a single instance of 'WriteableBitmap', writing different values
    // into its PixelBuffer, we reduce the amount of memory that's allocated between GC passes.
    // This makes the memory usage stay more or less constant even for dynamic images.

    private Windows.UI.Xaml.Media.Imaging.WriteableBitmap? m_writeableBitmap ;

    private List<long> m_bitmapLoadTimes = new () ;

    public string BitmapLoadTimes { get ; private set ; } = "" ;

    public string ActualTimerWakeupIntervals { get ; private set ; } = "" ;

    private List<long> m_actualTimerWakeupIntervals = new () ;

    private System.Diagnostics.Stopwatch m_timerTickStopwatch = new() ;

    private System.Diagnostics.Stopwatch m_timerTickReportStopwatch = new() ;

    private uint m_pixelValueToWrite = 0x7f000000 ;

    private void TimerTick ( object sender, object e )
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
        m_writeableBitmap ??= new(320,240)  ;
        System.Diagnostics.Stopwatch bitmapLoadingStopwatch = new() ;
        bitmapLoadingStopwatch.Start() ;
        var pixelBuffer = m_writeableBitmap.PixelBuffer ;
        using var stream = pixelBuffer.AsStream() ;
        using var binaryWriter = new System.IO.BinaryWriter(stream) ;
        int nPixels = m_writeableBitmap.PixelWidth * m_writeableBitmap.PixelHeight ;
        for ( int jPixel = 0 ; jPixel < nPixels ; jPixel++ )
        {
          binaryWriter.Write(
            m_pixelValueToWrite
          ) ;
        }
        m_pixelValueToWrite += 0x00302010 ;
        m_pixelValueToWrite |= 0xff000000 ;
        m_writeableBitmap.Invalidate() ;
        this.DynamicImageSource = m_writeableBitmap ;
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
