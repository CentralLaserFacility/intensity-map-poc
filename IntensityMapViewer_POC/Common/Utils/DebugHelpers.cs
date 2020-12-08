//
// DebugHelpers.cs
//

using Common.ExtensionMethods ;

namespace Common
{

  public static class DebugHelpers
  { 

    [System.Diagnostics.Conditional("DEBUG")]
    public static void WriteDebugLines ( params string[] lines )
    {
      lines.ForEachItem(
        line => System.Diagnostics.Debug.WriteLine(line)
      ) ;
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void DontWriteDebugLines ( params string[] lines )
    {
      // Don't write, but can put a breakpoint here ...
    }

  }

}
