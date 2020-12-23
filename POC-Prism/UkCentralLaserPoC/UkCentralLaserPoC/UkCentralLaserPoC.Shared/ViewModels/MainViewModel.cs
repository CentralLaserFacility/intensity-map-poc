using Common;

using UkCentralLaserPoC.Core.Mvvm;
using UkCentralLaserPoC.IntensityMap;
using Windows.UI.Xaml;

namespace UkCentralLaserPoC.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private CyclicSelector<(Windows.UI.Xaml.Media.ImageSource, string)> _staticImages;

        public CyclicSelector<(Windows.UI.Xaml.Media.ImageSource, string)> StaticImages
        {
            get { return _staticImages; }
            set { SetProperty(ref _staticImages, value); }
        }

        private IntensityMapDynamicViewModel _intensityMapDynamicViewModel;

        public IntensityMapDynamicViewModel IntensityMapDynamicViewModel
        {
            get { return _intensityMapDynamicViewModel; }
            set { SetProperty(ref _intensityMapDynamicViewModel, value); }
        }

        public MainViewModel(IntensityMapDynamicViewModel intensityMapDynamicViewModel)
        {
            StaticImages = new CyclicSelector<(Windows.UI.Xaml.Media.ImageSource, string)>(
              (
                UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
                  intensityMap: new IntensityMapViewer.IntensityMap.CreatedFromSincFunction(
                    sincFactor: 10.0
                  ),
                  colourMapOption: IntensityMapViewer.ColourMapOption.JetColours
                ),
                "Synthesised ripple with JET colours"
              ),
              (
                UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
                  intensityMap: new IntensityMapViewer.IntensityMap.CreatedFromSincFunction(
                    sincFactor: 5.0
                  ),
                  colourMapOption: IntensityMapViewer.ColourMapOption.GreyScale
                ),
                "Synthesised ripple with greyscale"
              ),
              (
                UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
                  intensityMap: new IntensityMapViewer.IntensityMap.CreatedFromSincFunction(
                    sincFactor: 15.0,
                    fractionalRadialOffsetFromCentre: 0.2
                  ).CreateCloneWithAddedRandomNoise(50),
                  colourMapOption: IntensityMapViewer.ColourMapOption.ShadesOfRed
                ),
                "Coloured with shades of red, offset from centre, with noise"
              ),
              (
                UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
                  intensityMap: new IntensityMapViewer.IntensityMap.CreatedWithRampingValues(),
                  colourMapOption: IntensityMapViewer.ColourMapOption.ShadesOfBlue
                ),
                "Ramping, blue"
              )
            );

            IntensityMapDynamicViewModel = intensityMapDynamicViewModel;
        }
    }
}
