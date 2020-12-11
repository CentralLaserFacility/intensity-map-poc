//
// IntensityMap_UnitTests.cs
//

using Xunit ;
using FluentAssertions ;

namespace ModelClasses_UnitTests
{

  public class IntensityMap_UnitTests
  {

    [Fact]
    public void CanCreateIntensityMapForTesting_16x12 ( )
    {
      var intensityMap = IntensityMapViewer.IntensityMapHelpers.CreateDummyInstanceForTesting_16x12() ;
      intensityMap.Dimensions.Width.Should().Be(16) ;
      intensityMap.Dimensions.Height.Should().Be(12) ;
      intensityMap.IntensityValues[0].Should().Be(0x00) ; 
      intensityMap.IntensityValues[16*12-1].Should().Be(16*12-1) ; 
      intensityMap.GetIntensityValueAt(xAcross:0,yDown:0).Should().Be(0x00) ;
      intensityMap.GetIntensityValueAt(xAcross:0,yDown:1).Should().Be(0x01) ;
    }

  }

}
