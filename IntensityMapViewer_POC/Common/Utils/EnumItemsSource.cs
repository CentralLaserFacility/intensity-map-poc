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

  public class EnumItemsSource<T> where T : struct, System.IConvertible
  {

    public string FullTypeString { get ; }

    public string Name { get ; }

    public T Value { get ; }

    public EnumItemsSource ( string name, T value, string fullTypeString )
    {
      if ( ! typeof(T).IsEnum )
      {
        throw new System.ArgumentException("EnumItemsSource only accept Enum type.");
      }
      Name           = name ;
      Value          = value ;
      FullTypeString = fullTypeString ;
    }

    //
    // Create a list of EnumItemsSource from an enum type.
    //

    public static List<EnumItemsSource<T>> ToList ( )
    {
      var namesList = System.Enum.GetNames(
        typeof(T)
      ) ;
      var valuesList = System.Enum.GetValues(
        typeof(T)
      ).Cast<T>().ToList() ;
      // Create EnumItemsSource list
      var enumItemsSourceList = new List<EnumItemsSource<T>>() ;
      for ( int i = 0 ; i < namesList.Length ; i++ )
      {
        enumItemsSourceList.Add(
          new EnumItemsSource<T>(
            namesList[i], 
            valuesList[i], 
            $"{typeof(T).Name}.{namesList[i]}"
          )
        ) ;
      }
      return enumItemsSourceList ;
    }

  }

}
