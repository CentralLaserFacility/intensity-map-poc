//
// EnumItemsSource.cs
//

using System.Collections.Generic;
using System.Linq;

namespace Common
{

  //
  // Helper class to bind an Enum type as an ItemsSource for a control .
  //
  // https://stackoverflow.com/questions/39209844/bind-uwp-combobox-itemssource-to-enum
  //
  // In a ViewModel :
  //
  //   public List<EnumItemsSource<AccountType>> AccountTypes => EnumItemsSource<AccountType>.ToList() ;
  //
  // In a View :
  //
  //   <ComboBox
  //     ItemsSource="{Binding AccountTypes}"
  //     DisplayMemberPath="Name"
  //     SelectedValuePath="Value"
  //     SelectedValue="{Binding Path=Account.Type, Mode=TwoWay}"
  //   />
  //
  // Hmm, this wasn't as successful as you'd hope ...
  //

  public class EnumItemsSource_OLD_DO_NOT_USE<T> where T : struct, System.IConvertible
  {

    public string Name { get ; }

    public T Value { get ; }

    // public string FullTypeString { get ; }

    public EnumItemsSource_OLD_DO_NOT_USE ( 
      string name, 
      T value
      // , string fullTypeString 
    ) {
      if ( ! typeof(T).IsEnum )
      {
        throw new System.ArgumentException("EnumItemsSource only accepts an Enum type");
      }
      Name           = name ;
      Value          = value ;
      // FullTypeString = fullTypeString ;
    }

    //
    // Create a list of EnumItemsSource from an enum type.
    //

    public static List<EnumItemsSource_OLD_DO_NOT_USE<T>> ToList ( )
    {
      var namesList = System.Enum.GetNames(
        typeof(T)
      ) ;
      var valuesList = System.Enum.GetValues(
        typeof(T)
      ).Cast<T>().ToList() ;
      var enumItemsSourceList = new List<EnumItemsSource_OLD_DO_NOT_USE<T>>() ;
      for ( int i = 0 ; i < namesList.Length ; i++ )
      {
        enumItemsSourceList.Add(
          new EnumItemsSource_OLD_DO_NOT_USE<T>(
            namesList[i], 
            valuesList[i]
            // , $"{typeof(T).Name}.{namesList[i]}"
          )
        ) ;
      }
      return enumItemsSourceList ;
    }

  }

}
