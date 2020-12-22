using Common;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Windows.UI.Xaml.Media;

namespace UkCentralLaserPoC.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private ImageSource _staticImage;

        public ImageSource StaticImage
        {
            get { return _staticImage; }
            set { SetProperty(ref _staticImage, value); }
        }

        private CyclicSelector<(ImageSource, string)> _staticImages;

        public CyclicSelector<(ImageSource, string)> StaticImages
        {
            get { return _staticImages; }
            set { SetProperty(ref _staticImages, value); }
        }

        public MainViewModel()
        {
            StaticImages = new CyclicSelector<(ImageSource, string)>(
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
        }
    }
}
