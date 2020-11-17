//
// SourceChoicesViewModel_UnitTests.cs
//

using Xunit ;
using FluentAssertions ;

namespace ModelClasses_UnitTests
{

  public class SourceChoicesViewModel_UnitTests
  {

    [Fact]
    public void CanCreateSourceChoicesViewModel ( )
    {
      var _ = new IntensityMapViewer.SourceChoicesViewModel() ;
    }

  }

}
