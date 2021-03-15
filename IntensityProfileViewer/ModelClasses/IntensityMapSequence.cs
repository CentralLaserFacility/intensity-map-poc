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
      sincFactorRange ??= (3.0,10.0) ;
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

    public static IntensityMapSequence CreateInstance_RippleRotatingAroundCircle ( 
      int                  nIntensityMaps,
      double               sincFactor,
      double               fractionalRadialOffsetFromCentre,
      byte                 maxIntensity = 180,
      System.Drawing.Size? dimensions   = null
    ) {
      dimensions ??= new System.Drawing.Size(320,240) ;
      return new IntensityMapSequence(
        Enumerable.Range(
          0,
          nIntensityMaps
        ).Select(
          i => {
            double rotation_frac01 = i * (
              1.0 
            / nIntensityMaps
            ) ;
            return new IntensityMap.CreatedFromSincFunction(
              dimensions.Value.Width,
              dimensions.Value.Height,
              sincFactor,
              fractionalRadialOffsetFromCentre         : fractionalRadialOffsetFromCentre,
              fractionalRotationOfOffsettedCentrePoint : rotation_frac01,
              maxIntensity                             : ComputeInterpolatedIntensity()
            ) ;
            byte ComputeInterpolatedIntensity ( )
            {
              // As 'rotation_frac01' goes from 0.0 to 1.0,
              // we want the 'intensity_frac01' to go from
              // 0.0 -> 1.0 -> 0.0, once per cycle
              var intensity_frac01 = (
                2.0 
              * System.Math.Abs(
                  rotation_frac01 - 0.5 // goes from -0.5 to +0.5
                )
              ) ;
              var minIntensity = maxIntensity * 0.1 ;
              return (byte) (
                minIntensity 
              + ( maxIntensity - minIntensity ) * intensity_frac01
              ) ;
            }
          }
        )
      ) ;
    }

    public static IntensityMapSequence CreateInstance_BlobRotatingAroundCircle ( 
      int                  nIntensityMaps,
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
          i => new IntensityMap.CreatedAsOffsettedCircle(
            fractionalRotationOfOffsettedCentrePoint : i * fractionalRotationPerStepOfOffsettedCentrePoint
          )
        )
      ) ;
    }

  }

}
