﻿//
// EnumValueDescriptor.cs
//

using System.Collections.Generic ;
using System.Linq ;

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

  public class EnumValueDescriptor
  {

    public string Name { get ; protected set ; } = "" ;

    public object ValueAsObject { get ; protected set ; }

    protected EnumValueDescriptor (
      string name,
      object valueAsObject
    ) {
      Name          = name ;
      ValueAsObject = valueAsObject ;
    }

    public override string ToString ( ) => Name ;

  }

  public class EnumValueDescriptor<TEnum> : EnumValueDescriptor where TEnum : struct
  {

    public EnumValueDescriptor ( TEnum value ) :
    base(
      value.ToString(),
      value
    ) {
      if ( ! typeof(TEnum).IsEnum )
      {
        throw new System.ArgumentException("EnumItemsSource only accepts an Enum type");
      }
      Value = value ;
      string fullNameOfValue = $"{typeof(TEnum).FullName}.{Name}" ;
      var ok = System.Enum.TryParse(
        fullNameOfValue,
        out TEnum enumResult
      ) ;
      ok = System.Enum.TryParse(
        Name,
        out TEnum enumResult2
      ) ;
    }

    public TEnum Value { get ; }

    //
    // Create a list of EnumItemsSource from an enum type.
    //

    public static IEnumerable<EnumValueDescriptor<TEnum>> CreateDescriptorsList ( )
    {
      return System.Enum.GetValues(
        typeof(TEnum)
      ).Cast<TEnum>().Select(
        enumValue => new EnumValueDescriptor<TEnum>(enumValue)
      ) ;
    }

  }

}
