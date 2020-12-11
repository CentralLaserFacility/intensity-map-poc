//
// Object_ExtensionMethods.cs
//

using System.Collections.Generic ;
using System.Linq ;

namespace Common.ExtensionMethods
{

  public static partial class Object_ExtensionMethods
  {

    public static T Verified<T> ( this T value, System.Func<T,bool> verify  )
    {
      if ( verify(value) == false )
      {
        throw new System.Exception("Verification failed") ;
      }
      return value ;
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Verify<T> ( this T value, System.Func<T,bool> verify  )
    {
      if ( verify(value) == false )
      {
        throw new System.Exception("Verification failed") ;
      }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void VerifyTrue ( this bool value  )
    {
      if ( value == false )
      {
        throw new System.Exception("Verification failed") ;
      }
    }

    public static string ClassName ( this object x )
    => (
      x?.GetType().GetTypeName() ?? "(null)"
    ) ;

    public static string GetTypeName (
      this object objectInstance_or_type,
      string      nameToReturnIfNull = "(null)"
    ) {
      if ( objectInstance_or_type is null )
      {
        return nameToReturnIfNull ;
      }
      else
      {
        System.Type type = (
          objectInstance_or_type is System.Type suppliedType
          ? suppliedType
          : objectInstance_or_type.GetType()
        ) ;
        return type.GetTypeName() ;
      }
    }

    public static string ToString_AllowingNull (
      this object? x_canBeNull
    ) {
      if ( x_canBeNull is null )
      {
        return "(null)" ;
      }
      else if ( x_canBeNull is System.Type )
      {
        return x_canBeNull.GetType().Name ;
      }
      else if ( x_canBeNull is System.Exception exception )
      {
        return exception.Message ;
      }
      else if ( x_canBeNull is string s )
      {
        return s ;
      }
      else
      {
        System.Type type = x_canBeNull.GetType() ;
        if (
           type.IsPrimitive
        || type.IsEnum
        ) {
          return x_canBeNull.ToString()! ;
        }
        else if ( type.IsArray )
        {
          return string.Format(
            "(Array - {0} elements",
            ( x_canBeNull as System.Array )!.Length.ToString()
          ) ;
        }
        else
        {
          return x_canBeNull.ToString() ;
        }
      }
    }

    public static IEnumerable<T> ConcatenatedWith<T> ( this T item, IEnumerable<T> others )
    => (
      new []{
        item
      }.Concat(
        others
      )
    ) ;

  }

}
