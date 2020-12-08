//
// StringBuilder_ExtensionMethods.cs
//

namespace Common.ExtensionMethods
{

  public static partial class StringBuilder_ExtensionMethods
  {

    public static System.Text.StringBuilder AppendLines (
      this System.Text.StringBuilder stringBuilder,
      params string[]                linesToAppend
    ) {
      linesToAppend.ForEachItem(
        (line) => {
          if ( line != null )
          {
            stringBuilder.AppendLine(line) ;
          }
        }
      ) ;
      return stringBuilder ;
    }

  }

}
