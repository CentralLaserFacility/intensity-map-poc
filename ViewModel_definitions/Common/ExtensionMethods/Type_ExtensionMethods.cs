//
// Type_ExtensionMethods.cs
//

using System.Linq ;

namespace Common.ExtensionMethods
{

  public static partial class Type_ExtensionMethods
  {

    public static string GetTypeName ( this System.Type type, string nameToReturnIfNull = "(null)" )
    {
      string typeName ;
      try
      {
        if ( type is null )
        {
          typeName = nameToReturnIfNull ;
        }
        else if ( type.IsGenericParameter )
        {
          typeName = type.Name ;
        }
        else if ( type.IsArray )
        {
          typeName =  (
            GetTypeName(
              type.GetElementType()!
            )
          + "[]".Repeated(
              type.GetArrayRank()
            )
          ) ;
        }
        else if ( type.IsGenericType )
        {
          string result = type.Namespace + "." + type.Name.Split('`')[0] + "<" ;
          System.Type[] genericArguments = type.GetGenericArguments() ;
          foreach ( System.Type T in genericArguments )
          {
            result += (
              T.IsGenericParameter // T.ContainsGenericParameters
              ? T.Name
              : GetTypeName(T)
            ) + "," ;
          }
          typeName = result.TrimEnd(',') + ">" ;
        }
        else
        {
          typeName =  type.Namespace + "." + type.Name ;
        }
      }
      catch ( System.Exception x )
      {
        typeName = type.Name + x ;
      }
      return (
        typeName
        .Replace( typeof( bool   ).FullName! , "bool"   )
        .Replace( typeof( byte   ).FullName! , "byte"   )
        .Replace( typeof( short  ).FullName! , "short"  )
        .Replace( typeof( int    ).FullName! , "int"    )
        .Replace( typeof( long   ).FullName! , "long"   )
        .Replace( typeof( float  ).FullName! , "float"  )
        .Replace( typeof( double ).FullName! , "double" )
        .Replace( typeof( string ).FullName! , "string" )
        .Replace( typeof( char   ).FullName! , "char"   )
      ) ;
    }

    public static void ForEachEnumerableValue<T> (
      this System.Type enumType,
      System.Action<T> action
    )
    where T : System.Enum
    {
      System.Array enumValues = System.Enum.GetValues(
        typeof(T)
      ) ;
      foreach ( T enumValue in enumValues.Cast<T>() )
      {
        action(enumValue) ;
      }
    }

  }

}
