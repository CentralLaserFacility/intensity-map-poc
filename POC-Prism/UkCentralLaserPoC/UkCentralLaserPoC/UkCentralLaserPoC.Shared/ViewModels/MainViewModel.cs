using Common;
using PropertyChanged;
using UkCentralLaserPoC.Core.Mvvm;
using UkCentralLaserPoC.IntensityMap;
using Windows.UI.Xaml;

namespace UkCentralLaserPoC.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel : ViewModelBase
    {

        public CyclicSelector<(Windows.UI.Xaml.Media.ImageSource, string)> StaticImages { get; set; }

        public IntensityMapDynamicViewModel IntensityMapDynamicViewModel { get; set; }

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
