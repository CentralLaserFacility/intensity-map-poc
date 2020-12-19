//
// IntensityMapTestViewModel.cs
//

using System.Windows.Input;

using System.Collections.Generic ;
using System.Linq ;
using IntensityMapViewer.ViewModel_interfaces;

namespace UkCentralLaserPoC.IntensityMap
{

  //
  // This drives a UI that lets us exercise the visualisation
  // of an IntensityMap as a WriteableBitmap.
  //
  // 1. A static image from a selectable source eg
  //    - Ripple pattern, synthesised from fixed parameters
  //    - Loaded from a 'pgm' file
  //    - Loaded from EPICS (eventually)
  //   With a selectable colour map :
  //     Solid colour (grey,r,g,b)
  //     JET colour
  //
  // 2. A dynamically evolving 'ripple' pattern, 
  //    to demonstrate smooth real time performance
  //    and the absence of memory-allocation issues.
  //    
  // 3. A real time image captured from EPICS (eventually).
  //    
  // Wickedly we're making this ViewModel have a dependency on platform-specific types
  // such as DispatcherTimer and WriteableBitmap. That is perfectly appropriate here
  // as its purpose is for testing stuff on UWP, and making it platform independent
  // would add quite a bit of complexity to the View.
  //

  public class IntensityMapTestViewModel : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
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

    // Hmm, pity that the binding engine doesn't understand tuples ...
    // public (double Min,double Max) TimerPeriodValidRange => (20.0,2000.0) ;

    public double TimerPeriod_Min     { get ; } = 20.0 ;
    public double TimerPeriod_Max     { get ; } = 500.0 ;
    public double TimerPeriod_Default { get ; } = 100.0 ;

    public double FramesPerSecond_Max => 1000.0 / TimerPeriod_Min ; // 20mS => 50 fps
    public double FramesPerSecond_Min => 1000.0 / TimerPeriod_Max ; // 500mS => 2 fps

    public string TimerPeriod_AsString => $"Update requested every {TimerPeriodInMillisecs:F0}mS" ;

    public string FramesPerSecond_AsString => $"Frames per sec : {FramesPerSecond:F0}" ;

    private Common.CyclicSelector<(Windows.UI.Xaml.Media.ImageSource,string)> m_staticImagesSelector = new(
      (
        UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
          intensityMap : 
          new IntensityMapViewer.IntensityMap.CreatedAsOffsettedCircle(),
          colourMapOption : IntensityMapViewer.ColourMapOption.GreyScale
        ),
        "Synthesised greyscale blob"
      ),
      (
        UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
          intensityMap : 
          new IntensityMapViewer.IntensityMap.CreatedFromSincFunction(),
          colourMapOption : IntensityMapViewer.ColourMapOption.JetColours
        ),
        "Synthesised ripple with JET colours"
      ),
      (
        UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
          intensityMap : new IntensityMapViewer.IntensityMap.CreatedFromSincFunction(
            sincFactor : 5.0
          ),
          colourMapOption : IntensityMapViewer.ColourMapOption.GreyScale
        ),
        "Synthesised ripple with greyscale"
      ),
      (
        UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
          intensityMap : new IntensityMapViewer.IntensityMap.CreatedFromSincFunction(
            sincFactor                       : 15.0,
            fractionalRadialOffsetFromCentre : 0.2
          ).CreateCloneWithAddedRandomNoise(50),
          colourMapOption : IntensityMapViewer.ColourMapOption.ShadesOfRed
        ),
        "Coloured with shades of red, offset from centre, with noise"
      ),
      (
        UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
          intensityMap : new IntensityMapViewer.IntensityMap.CreatedWithRampingValues(),
          colourMapOption : IntensityMapViewer.ColourMapOption.ShadesOfBlue
        ),
        "Ramping, blue"
      )
    ) ;

    private Common.CyclicSelector<IntensityMapViewer.IIntensityMap> m_dynamicIntensityMapsSelector = new(
      2 switch 
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

    private bool m_performDynamicImageUpdates = false ;

    public IntensityMapTestViewModel ( )
    {
      m_timerPeriodInMillisecs = TimerPeriod_Default ;
      m_timer.Interval = System.TimeSpan.FromMilliseconds(100) ;
      m_timer.Tick += TimerTick ;
      m_timer.Start() ;
      MoveToNextStaticImage = new Microsoft.Toolkit.Mvvm.Input.RelayCommand(
        () => (StaticImageSource,StaticImageLabel) = m_staticImagesSelector.GetCurrent_MoveNext()
      ) ;
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
      (m_staticImageSource,m_staticImageLabel) = m_staticImagesSelector.GetCurrent_MoveNext() ;
      // UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
      //  IntensityMapViewer.IntensityMapHelpers.CreateSynthetic_UsingSincFunction()
      // ) ;
      m_dynamicImageSource = UwpUtilities.BitmapHelpers.LoadOrCreateWriteableBitmap(
        ref m_writeableBitmap,
        m_dynamicIntensityMapsSelector.Current
      ) ; 
    }

    private Windows.UI.Xaml.Media.ImageSource m_staticImageSource ;
    public Windows.UI.Xaml.Media.ImageSource StaticImageSource 
    {
      get => m_staticImageSource ;
      set => base.SetProperty(
        ref m_staticImageSource,
        value
      ) ;
    }

    private string m_staticImageLabel = "Static image" ;
    public string StaticImageLabel 
    {
      get => m_staticImageLabel ;
      set => base.SetProperty(
        ref m_staticImageLabel,
        value
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

    public Microsoft.Toolkit.Mvvm.Input.IRelayCommand MoveToNextStaticImage { get ; }

    public Microsoft.Toolkit.Mvvm.Input.IRelayCommand StartDynamicImageUpdates { get ; }

    public Microsoft.Toolkit.Mvvm.Input.IRelayCommand StopDynamicImageUpdates { get ; }

    // When the timer fires, we replace the Dynamic image.

    // By re-using a single instance of 'WriteableBitmap', writing different values
    // into its PixelBuffer, we reduce the amount of memory that's allocated between GC passes.
    // This makes the memory usage stay more or less constant even for dynamic images.

    private Windows.UI.Xaml.Media.Imaging.WriteableBitmap? m_writeableBitmap = null ;

    private List<long> m_bitmapLoadTimes = new () ;

    public string BitmapLoadTimes { get ; private set ; } = "" ;

    public string ActualTimerWakeupIntervals { get ; private set ; } = "" ;

    private List<long> m_actualTimerWakeupIntervals = new () ;

    private System.Diagnostics.Stopwatch m_timerTickStopwatch = new() ;

    private System.Diagnostics.Stopwatch m_timerTickReportStopwatch = new() ;

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
        System.Diagnostics.Stopwatch bitmapLoadingStopwatch = new() ;
        bitmapLoadingStopwatch.Start() ;
        this.DynamicImageSource = UwpUtilities.BitmapHelpers.LoadOrCreateWriteableBitmap(
          ref m_writeableBitmap,
          m_dynamicIntensityMapsSelector.GetCurrent_MoveNext()
        ) ; 
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
