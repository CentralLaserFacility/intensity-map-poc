//
// ExtensionMethods_UnitTests.cs
//

using Xunit ;
using FluentAssertions ;
using static FluentAssertions.FluentActions ;

using Common.ExtensionMethods ;
using System.Linq ;

namespace Common_UnitTests
{

  public sealed class ExtensionMethods_UnitTests
  {

    [Fact]
    public void Verified_WorksAsExpected ( )
    {
      3.Verified(x=>x==3).Should().Be(3) ;
    }

    [Fact]
    public void XX_WorksAsExpected ( )
    {
    }

  }

}
