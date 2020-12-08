//
// SystemConstants_UnitTests.cs
//

using Xunit ;
using FluentAssertions ;

namespace ModelClasses_UnitTests
{

  public class SystemConstants_UnitTests
  {

    [Fact]
    public void CanCreateSystemConstants ( )
    {
      var _ = IntensityMapViewer.SystemConstants.Default ;
    }

  }

}
