//
// ColourMapper_UnitTests.cs
//

using Xunit ;
using FluentAssertions ;
using System.Linq ;

namespace ModelClasses_UnitTests
{

  public class ColourMapper_UnitTests
  {

    [Fact]
    public void ColourMappingWorksAsExpected_ForGreyScale ( )
    {
      var mappedColours = IntensityMapViewer.ColourMapper.MapByteValuesToRgb(
        new byte[]{0x00,0x7f,0xff},
        IntensityMapViewer.ColourMapOption.GreyScale
      ) ;
      ( (uint) mappedColours[0].ToArgb() ).Should().Be(0xff000000) ;
      ( (uint) mappedColours[1].ToArgb() ).Should().Be(0xff7f7f7f) ;
      ( (uint) mappedColours[2].ToArgb() ).Should().Be(0xffffffff) ;
    }

  }

}
