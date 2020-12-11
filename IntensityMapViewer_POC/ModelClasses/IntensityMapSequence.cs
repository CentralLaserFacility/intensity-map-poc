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

    private int m_currentIndex = 0 ;

    public IntensityMap GetCurrent_MoveNext ( ) 
    => m_intensityMaps[
      m_currentIndex++
    % m_intensityMaps.Count
    ] ;
    
    public IntensityMapSequence ( 
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
      m_intensityMaps = Enumerable.Range(
        0,
        nIntensityMaps
      ).Select(
        i => IntensityMapHelpers.CreateSynthetic_UsingSincFunction(
          dimensions.Value.Width,
          dimensions.Value.Height,
          (
            sincFactorRange.Value.minSincFactor 
          + i * sincFactorDelta
          )
        )
      ).ToList() ;
    }

    public IntensityMapSequence ( 
      int                  nIntensityMaps,
      double               sincFactor,
      byte                 noiseAmplitude,
      System.Drawing.Size? dimensions = null
    ) {
      dimensions ??= new System.Drawing.Size(320,240) ;
      var masterInstance = IntensityMapHelpers.CreateSynthetic_UsingSincFunction(
        dimensions.Value.Width,
        dimensions.Value.Height,
        sincFactor : sincFactor
      ) ;
      m_intensityMaps = Enumerable.Range(
        0,
        nIntensityMaps
      ).Select(
        i => masterInstance.CreateCloneWithAddedRandomNoise(
          noiseAmplitude
        )
      ).ToList() ;
    }

  }

}
