using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using UkCentralLaserPoC.Core.Mvvm;

namespace UkCentralLaserPoC.IntensityMap
{
    public class IntensityMapDynamicViewModel: ViewModelBase
    {
        private Windows.UI.Xaml.DispatcherTimer m_timer = new();

        private double m_timerPeriodInMillisecs;
        public double TimerPeriodInMillisecs
        {
            get => m_timerPeriodInMillisecs;
            set
            {
                if (
                  SetProperty(
                    ref m_timerPeriodInMillisecs,
                    value
                  )
                )
                {
                    m_timer.Interval = System.TimeSpan.FromMilliseconds(m_timerPeriodInMillisecs);
                    base.RaisePropertyChanged(
                      nameof(TimerPeriod_AsString)
                    );
                    base.RaisePropertyChanged(
                      nameof(FramesPerSecond)
                    );
                    base.RaisePropertyChanged(
                      nameof(FramesPerSecond_AsString)
                    );
                }
            }
        }

        public double FramesPerSecond
        {
            // ( 1000.0 / 20mS ) ==> 50 fps
            get => 1000.0 / TimerPeriodInMillisecs;
            // 50 fps ==> timer period of (1000/50) ==> 20mS 
            set => TimerPeriodInMillisecs = 1000.0 * (1.0 / value);
        }

        // Hmm, pity that the binding engine doesn't understand tuples ...
        // public (double Min,double Max) TimerPeriodValidRange => (20.0,2000.0) ;

        public double TimerPeriod_Min { get; } = 20.0;
        public double TimerPeriod_Max { get; } = 500.0;
        public double TimerPeriod_Default { get; } = 100.0;

        public string TimerPeriod_AsString => $"Updates every {TimerPeriodInMillisecs:F0}mS";

        public string FramesPerSecond_AsString => $"Frames per sec : {FramesPerSecond:F0}";


        private Common.CyclicSelector<IntensityMapViewer.IIntensityMap> m_dynamicIntensityMapsSelector = new(
          IntensityMapViewer.IntensityMapSequence.CreateInstance_RippleRotatingAroundCircle(
            nIntensityMaps: 60,
            sincFactor: 10.0,
            fractionalRadialOffsetFromCentre: 0.2
          ).IntensityMaps
        );

        public IntensityMapDynamicViewModel()
        {
            m_timerPeriodInMillisecs = TimerPeriod_Default;
            m_timer.Interval = System.TimeSpan.FromMilliseconds(100);
            m_timer.Tick += TimerTick;
          
            StartDynamicImageUpdates = new DelegateCommand(
              () => {
                  m_timer.Start();
                  StartDynamicImageUpdates.RaiseCanExecuteChanged();
                  StopDynamicImageUpdates.RaiseCanExecuteChanged();
              },
              () => m_timer.IsEnabled is false
            );

            StopDynamicImageUpdates = new DelegateCommand(
              () => {
                  m_timer.Stop();
                  StartDynamicImageUpdates.RaiseCanExecuteChanged();
                  StopDynamicImageUpdates.RaiseCanExecuteChanged();
              },
              () => m_timer.IsEnabled is true
            );
        
            m_dynamicImageSource = UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
              new IntensityMapViewer.IntensityMap.CreatedFromSincFunction()
            );
        }


        private Windows.UI.Xaml.Media.ImageSource m_dynamicImageSource;
        public Windows.UI.Xaml.Media.ImageSource DynamicImageSource
        {
            get => m_dynamicImageSource;
            set => base.SetProperty(
              ref m_dynamicImageSource,
              value
            );
        }

        public DelegateCommand StartDynamicImageUpdates { get; }

        public DelegateCommand StopDynamicImageUpdates { get; }

        // When the timer fires, we replace the Dynamic image.

        // By re-using a single instance of 'WriteableBitmap', writing different values
        // into its PixelBuffer, we reduce the amount of memory that's allocated between GC passes.
        // This makes the memory usage stay more or less constant even for dynamic images.

        private Windows.UI.Xaml.Media.Imaging.WriteableBitmap? m_writeableBitmap = null;

        private void TimerTick(object sender, object e)
        {
            this.DynamicImageSource = UwpUtilities.BitmapHelpers.LoadOrCreateWriteableBitmap(
              ref m_writeableBitmap,
              m_dynamicIntensityMapsSelector.GetCurrent_MoveNext()
            );
            // If we set this to null, then a fresh PixelBuffer gets allocated for each Image
            // and memory usage increases until GC kicks in (every few seconds).
            // m_writeableBitmap = null ;
        }

    }
}