//
// IntensityMapSequence.cs
//

using System.Collections.Generic;
using System.Linq;

namespace IntensityMapViewer
{

  public class IntensityMapSequence
  {

    private IReadOnlyList<IntensityMap> m_intensityMaps ;

    public IEnumerable<IIntensityMap> IntensityMaps => m_intensityMaps ;

    private int m_currentIndex = 0 ;

    public IntensityMap GetCurrent_MoveNext ( ) 
    => m_intensityMaps[
      m_currentIndex++
    % m_intensityMaps.Count
    ] ;

    private IntensityMapSequence ( IEnumerable<IntensityMap> intensityMaps )
    {
      m_intensityMaps = intensityMaps.ToList() ;
    }
    
    public static IntensityMapSequence CreateInstance_WithProgressivelyIncreasingSincFactor ( 
      int                                          nIntensityMaps,
      (double minSincFactor,double maxSincFactor)? sincFactorRange = null,
      System.Drawing.Size?                         dimensions      = null
    ) {
      dimensions ??= new System.Drawing.Size(320,240) ;
      sincFactorRange ??= (5.0,10.0) ;
      double sincFactorDelta = (
        sincFactorRange.Value.maxSincFactor
      - sincFactorRange.Value.minSincFactor
      ) / ( nIntensityMaps - 1 ) ;
      return new IntensityMapSequence(
        Enumerable.Range(
          0,
          nIntensityMaps
        ).Select(
          i => new IntensityMap.CreatedFromSincFunction(
            dimensions.Value.Width,
            dimensions.Value.Height,
            (
              sincFactorRange.Value.minSincFactor 
            + i * sincFactorDelta
            )
          )
        )
      ) ;
    }

    public static IntensityMapSequence CreateInstance_WithNoiseAdded ( 
      int                  nIntensityMaps,
      double               sincFactor,
      byte                 noiseAmplitude,
      System.Drawing.Size? dimensions = null
    ) {
      dimensions ??= new System.Drawing.Size(320,240) ;
      var masterInstance = new IntensityMap.CreatedFromSincFunction(
        dimensions.Value.Width,
        dimensions.Value.Height,
        sincFactor : sincFactor
      ) ;
      return new IntensityMapSequence(
          Enumerable.Range(
          0,
          nIntensityMaps
        ).Select(
          i => masterInstance.CreateCloneWithAddedRandomNoise(
            noiseAmplitude
          )
        )
      ) ;
    }

    public static IntensityMapSequence CreateInstance_RotatingAroundCircle ( 
      int                  nIntensityMaps,
      double               sincFactor,
      double               fractionalRadialOffsetFromCentre,
      System.Drawing.Size? dimensions      = null
    ) {
      dimensions ??= new System.Drawing.Size(320,240) ;
      double fractionalRotationPerStepOfOffsettedCentrePoint = (
        1.0 
      / nIntensityMaps
      ) ;
      return new IntensityMapSequence(
        Enumerable.Range(
          0,
          nIntensityMaps
        ).Select(
          i => new IntensityMap.CreatedFromSincFunction(
            dimensions.Value.Width,
            dimensions.Value.Height,
            sincFactor,
            fractionalRadialOffsetFromCentre : fractionalRadialOffsetFromCentre,
            fractionalRotationOfOffsettedCentrePoint : i * fractionalRotationPerStepOfOffsettedCentrePoint
          )
        )
      ) ;
    }

  }

}
