//
// IntensityMapTestViewModel.cs
//

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IntensityMapViewer
{

  //
  // This drives a UI that lets us exercise the visualisation of an IntensityMap.
  //
  // It provides a fixed (ie not time-varying) image from a selectable source eg
  //  - Ripple pattern, synthesised from fixed parameters
  //  - Loaded from a 'pgm' file
  //  - Loaded from EPICS (eventually)
  // ... with a selectable colour map :
  //   Solid colour (grey,r,g,b)
  //   JET colour
  //

  public class StaticIntensityMapsDemo_ViewModel 
  : IntensityMapsDemo_ViewModel
  {

    private Common.CyclicSelector<(IIntensityMap,ColourMapOption,string)> m_staticImagesSelector = new(
      (
        new IntensityMap.CreatedAsOffsettedCircle(),
        ColourMapOption.ShadesOfRed,
        "Solid red"
      ),
      (
        new IntensityMap.CreatedAsOffsettedCircle(),
        ColourMapOption.ShadesOfGreen,
        "Solid green"
      ),
      (
        new IntensityMap.CreatedAsOffsettedCircle(),
        ColourMapOption.ShadesOfBlue,
        "Solid blue"
      )
      // (
      //   UwpUtilities.BitmapHelpers_ForTesting.CreateWriteableBitmap_ForTesting_B(
      //     (x,y) => (0xff,(byte)(x*2),(byte)(y*2))
      //   ), 
      //   "Snazzy"
      // ),
      // (
      //   UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
      //     intensityMap : 
      //     new IntensityMapViewer.IntensityMap.CreatedAsOffsettedCircle(),
      //     colourMapOption : IntensityMapViewer.ColourMapOption.GreyScale
      //   ),
      //   "Synthesised greyscale blob"
      // ),
      // (
      //   UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
      //     intensityMap : 
      //     new IntensityMapViewer.IntensityMap.CreatedFromSincFunction(),
      //     colourMapOption : IntensityMapViewer.ColourMapOption.JetColours
      //   ),
      //   "Synthesised ripple with JET colours"
      // ),
      // (
      //   UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
      //     intensityMap : new IntensityMapViewer.IntensityMap.CreatedFromSincFunction(
      //       sincFactor : 5.0
      //     ),
      //     colourMapOption : IntensityMapViewer.ColourMapOption.GreyScale
      //   ),
      //   "Synthesised ripple with greyscale"
      // ),
      // (
      //   UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
      //     intensityMap : new IntensityMapViewer.IntensityMap.CreatedFromSincFunction(
      //       sincFactor                       : 15.0,
      //       fractionalRadialOffsetFromCentre : 0.2
      //     ).CreateCloneWithAddedRandomNoise(50),
      //     colourMapOption : IntensityMapViewer.ColourMapOption.ShadesOfRed
      //   ),
      //   "Coloured with shades of red, offset from centre, with noise"
      // ),
      // (
      //   UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
      //     intensityMap : new IntensityMapViewer.IntensityMap.CreatedWithRampingValues(),
      //     colourMapOption : IntensityMapViewer.ColourMapOption.ShadesOfBlue
      //   ),
      //   "Ramping, blue"
      // )
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

    // private bool m_performDynamicImageUpdates = false ;
    // 
    // private IIntensityMap m_intensityMap ;
    // public IIntensityMap IntensityMap {
    //   get => m_intensityMap ;
    //   set => SetProperty(ref m_intensityMap,value) ;
    // }
    // 
    // private ColourMapOption m_colourMapOption ;
    // public ColourMapOption ColourMapOption {
    //   get => m_colourMapOption ;
    //   set => SetProperty(ref m_colourMapOption,value) ;
    // }
    // 
    // private string m_intensityMapLabel ;
    // public string IntensityMapLabel 
    // {
    //   get => m_intensityMapLabel ;
    //   set => SetProperty(ref m_intensityMapLabel,value) ;
    // }

    public StaticIntensityMapsDemo_ViewModel ( )
    {
      MoveToNextStaticImage = new Microsoft.Toolkit.Mvvm.Input.RelayCommand(
        () => (IntensityMap,ColourMapOption,IntensityMapLabel) = m_staticImagesSelector.GetCurrent_MoveNext()
      ) ;
      MoveToNextStaticImage.Execute(null) ;
    }

    // private Windows.UI.Xaml.Media.ImageSource m_staticImageSource ;
    // public Windows.UI.Xaml.Media.ImageSource StaticImageSource 
    // {
    //   get => m_staticImageSource ;
    //   set => base.SetProperty(
    //     ref m_staticImageSource,
    //     value
    //   ) ;
    // }

    private string m_staticImageLabel = "Static image" ;
    public string StaticImageLabel 
    {
      get => m_staticImageLabel ;
      set => base.SetProperty(
        ref m_staticImageLabel,
        value
      ) ;
    }

    // private Windows.UI.Xaml.Media.ImageSource m_dynamicImageSource ;
    // public Windows.UI.Xaml.Media.ImageSource DynamicImageSource 
    // {
    //   get => m_dynamicImageSource ;
    //   set => base.SetProperty(
    //     ref m_dynamicImageSource,
    //     value
    //   ) ;
    // }

    public Microsoft.Toolkit.Mvvm.Input.IRelayCommand MoveToNextStaticImage { get ; }

  }

}
