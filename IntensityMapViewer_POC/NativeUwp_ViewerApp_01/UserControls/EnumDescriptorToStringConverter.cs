using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace NativeUwp_ViewerApp_01
{

  public class EnumDescriptorToStringConverter : IValueConverter
  {

    public object Convert ( object value, Type targetType, object parameter, string language )
    {
      // Hmm, could return a DisplayValue described by an attribute,
      // but that would complicate the 'ConvertBack' function as we'd
      // have to get from the DisplayValue to the enum ...
      return value?.ToString() ?? DependencyProperty.UnsetValue ;
    }
    public object ConvertBack ( object stringValue, Type targetType, object parameter, string language )
    { 
      bool ok = System.Enum.TryParse(
        targetType,
        stringValue as string,
        out var enumResult
      ) ;
      return (
        ok
        ? enumResult
        : null
      ) ;
    }
  }

}
