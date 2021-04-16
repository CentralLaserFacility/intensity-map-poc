//
// EnumToStringConverter.cs
//

namespace UwpUtilities
{

  // public class EnumToStringConverter : IValueConverter
  // {
  //   public object Convert ( object enumValue, Type targetType, object parameter, string language )
  //   {
  //     // Hmm, could return a DisplayValue described by an attribute,
  //     // but that would complicate the 'ConvertBack' function as we'd
  //     // have to get from the DisplayValue to the enum ...
  //     return enumValue?.ToString() ?? DependencyProperty.UnsetValue ;
  //   }
  //   public object ConvertBack ( object stringValue, Type targetType, object parameter, string language )
  //   => (
  //     System.Enum.TryParse(
  //       targetType,
  //       stringValue as string,
  //       out var enumResult
  //     ) 
  //     ? enumResult
  //     : null 
  //   ) ;
  // }

}
