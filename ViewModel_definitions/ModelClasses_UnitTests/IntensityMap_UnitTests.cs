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
    public void CanCreateIntensityMapFromSpecialTestFile ( )
    {
      var intensityMap = IntensityMapViewer.IntensityMap.CreateFromPortableGreyMapFile(
        "ImageFile_16x16_withAcendingByteValues.pgm" // Or embed the textual data as a string ??
      ) ;
      intensityMap.Dimensions.Width.Should().Be(16) ;
      intensityMap.Dimensions.Height.Should().Be(16) ;
      intensityMap.IntensityValues[0].Should().Be(0x00) ; 
      intensityMap.IntensityValues[255].Should().Be(0xff) ; 
      intensityMap.GetIntensityValueAt(xAcross:0,yDown:0).Should().Be(0x00) ;
      intensityMap.GetIntensityValueAt(xAcross:0,yDown:1).Should().Be(0x01) ;
    }

  }

}
