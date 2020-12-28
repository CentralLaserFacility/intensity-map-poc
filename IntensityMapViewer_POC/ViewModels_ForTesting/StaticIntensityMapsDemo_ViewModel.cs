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
        new IntensityMap.CreatedFromSincFunction(
          sincFactor:5.0
        ).CreateCloneWithAddedRandomNoise(30),
        ColourMapOption.GreyScale,
        "Ripple, grey with noise"
      ),
      (
        new IntensityMap.CreatedFromSincFunction(),
        ColourMapOption.JetColours,
        "Ripple, JET colours"
      ),
      (
        new IntensityMap.CreatedAsUniformPixelValue(),
        ColourMapOption.ShadesOfRed,
        "Solid red"
      ),
      (
        new IntensityMap.CreatedAsUniformPixelValue(),
        ColourMapOption.ShadesOfGreen,
        "Solid green"
      ),
      (
        new IntensityMap.CreatedAsUniformPixelValue(),
        ColourMapOption.ShadesOfBlue,
        "Solid blue"
      ),
      (
        new IntensityMap.CreatedAsOffsettedCircle(),
        ColourMapOption.ShadesOfBlue,
        "Offsetted circle, shades of blue"
      ),
      (
        new IntensityMap.CreatedFromCoordinatesXY(
          (x,y) => (byte) ( (x*2) + (y*3) )
        ), 
        ColourMapOption.ShadesOfBlue,
        "Diagonals in blue"
      ),
      (
        new IntensityMap.CreatedFromSincFunction(sincFactor:15.0),
        ColourMapOption.ShadesOfBlue,
        "Ripple, blue"
      ),
      (
        new IntensityMap.CreatedWithRampingValues(),
        ColourMapOption.ShadesOfBlue,
        "Ramping values"
      )
    ) ;

    public StaticIntensityMapsDemo_ViewModel ( )
    {
      MoveToNextStaticImage = new Microsoft.Toolkit.Mvvm.Input.RelayCommand(
        () => (IntensityMap,ColourMapOption,IntensityMapLabel) = m_staticImagesSelector.GetCurrent_MoveNext()
      ) ;
      MoveToNextStaticImage.Execute(null) ;
    }

    public Microsoft.Toolkit.Mvvm.Input.IRelayCommand MoveToNextStaticImage { get ; }

  }

}
