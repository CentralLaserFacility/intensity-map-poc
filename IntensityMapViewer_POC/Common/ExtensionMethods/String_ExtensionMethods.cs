//
// String_ExtensionMethods.cs
//

namespace Common.ExtensionMethods
{

  public static class String_ExtensionMethods
  {

    public static string Repeated ( this string s, int nRepeats )
    {
      // Hmm, surely there's a neater 'functional' way ???
      // Yes, but it would be much less efficient than this state-based method ...
      var stringBuilder = new System.Text.StringBuilder(s.Length*nRepeats) ;
      nRepeats.Repeat(
        () => stringBuilder.Append(s)
      ) ;
      return stringBuilder.ToString() ;
    }

    public static string PrefixedWith ( this string s, string prefix )
    => (
      prefix + s
    ) ;

    public static string Indented ( this string s, int nLevels, string indent = "  " )
    => (
      s.PrefixedWith(
        indent.Repeated(nLevels)
      )
    ) ;

    public static string PaddedWithSpacesToMinimumLength ( this string s, int length )
    {
      int nSpaces = length - s.Length ;
      if ( nSpaces > 0 )
      {
        s += new string(
          ' ',
          nSpaces
        ) ;
      }
      return s ;
    }

    public static bool IsNullOrEmpty ( this string? s )
    => (
      string.IsNullOrEmpty(s)
    ) ;

    public static bool IsEmpty ( this string s )
    => (
      s.Length == 0
    ) ;

    public static string TruncatedAtMaxLength ( this string s, int maxLength )
    {
      if ( s.Length <= maxLength )
      {
        // No truncation necessary
        return s ;
      }
      else
      {
        return s.Substring(0,maxLength) ;
      }
    }

    public static string EnclosedInQuotes ( this string s, string quotes = "\"\"" )
    => (
      $"{quotes[0]}{s}{quotes[1]}"
    ) ;

    public static string EnclosedInDoubleQuotes ( this string s )
    => (
      $"\"{s}\""
    ) ;

    public static string EnclosedInSingleQuotes ( this string s )
    => (
      $"'{s}'"
    ) ;

    public static string EnclosedInBrackets ( this string s, string brackets = "[]" )
    => (
      brackets.Length == 2
      ? $"{brackets[0]}{s}{brackets[1]}"
      : s
    ) ;

    public static string? PrefixedBy ( this string? s, string prefix )
    => (
      s == null
      ? null
      : prefix + s
    ) ;

    public static string[] SplitAtLineBoundaries ( this string lines )
    => (
      lines.Replace("\r","").Split('\n')
    ) ;

  }

}
