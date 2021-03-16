using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UwpUtilities
{

  public class ViewModelBase : INotifyPropertyChanged
  {

    public event PropertyChangedEventHandler PropertyChanged ;

    private void Set<T> ( 
      ref T                                                     storage, 
      T                                                         value, 
      [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null
    ) {
      if ( Equals(storage,value) )
      {
        return ;
      }
      storage = value ;
      OnPropertyChanged(propertyName) ;
    }

    private void OnPropertyChanged ( string propertyName ) => PropertyChanged?.Invoke(
      this, 
      new PropertyChangedEventArgs(propertyName)
    ) ;

  }

  public class ViewModelForMyUserControlEx : ViewModelBase
  {
    public string Name => "Just testing" ;
  }

  //
  // This abstract 'UserControlEx' base class defines a ViewModelBase dependency property,
  // which we can x:Bind to in the container's XAML and access as a strongly typed
  // ViewModel property in the concrete subclass.
  //

  public abstract class UserControlEx : UserControl, INotifyPropertyChanged
  {

    public event PropertyChangedEventHandler PropertyChanged ;

    //
    // In the XAML that instantiates an instance of a class derived from 
    // this 'UserControlEx' base class, we can use x:Bind to populate the ViewModel.
    // Then in the derived class we can define a strongly-typed ViewModel property.
    //

    public static readonly DependencyProperty ViewModelBaseProperty = DependencyProperty.Register(
      nameof(ViewModelBase), 
      typeof(INotifyPropertyChanged), 
      typeof(UserControlEx), 
      new PropertyMetadata(0,OnViewModelBasePropertyChanged)
    ) ;

    private static void OnViewModelBasePropertyChanged ( DependencyObject sender, DependencyPropertyChangedEventArgs e )
    {
      // Hmm, we really need to raise a PropertyChanged pertaining to the 'ViewModel' property
      // that we'll have in the derived class ... but how to do this ???
      if ( sender is UserControlEx userControlEx )
      {
        userControlEx.PropertyChanged?.Invoke(
          sender,
          new PropertyChangedEventArgs("ViewModel")
        ) ;
      }
    }

    public INotifyPropertyChanged ViewModelBase
    {
      get { return base.GetValue(ViewModelBaseProperty) as INotifyPropertyChanged ; }
      set { base.SetValue(ViewModelBaseProperty,value) ; }  
    }

  }

  //
  // Would have been nice to define a strongly-typed ViewModel property,
  // but this attempt failed. Because the XAML for a subclass, such as
  // 
  //  class MyUserControlEx : UserControlEx<ViewModelForMyUserControlEx>
  //
  // ... needs to specify the base class at the top level,
  // and whilst this can be donefor a non-generic base class such as
  //
  //  class MyUserControlEx : UserControlEx
  //
  //  <local:UserControlEx
  //    x:Class="UwpUtilities.MyUserControlEx"
  //
  // ... theres no way to specify a type parameter.
  //

  // public class UserControlEx<TViewModel> : UserControl
  // {
  // 
  //   public TViewModel XViewModel
  //   {
  //     get { return (TViewModel) base.GetValue(XViewModelProperty) ; }
  //     set { base.SetValue(XViewModelProperty,value) ; }  
  //   }
  // 
  //   public static readonly DependencyProperty XViewModelProperty = DependencyProperty.Register(
  //     nameof(XViewModel), 
  //     typeof(TViewModel), 
  //     typeof(UserControlEx<TViewModel>), 
  //     new PropertyMetadata(0)
  //   ) ;
  // 
  // }

  public sealed partial class MyUserControlEx : UserControlEx
  {

    // Hmm, better than nothing ...

    public ViewModelForMyUserControlEx ViewModel => ViewModelBase as ViewModelForMyUserControlEx ;

    public MyUserControlEx ( )
    {
      this.InitializeComponent() ;
      // ViewModelBase = new ViewModelForMyUserControlEx() ;
    }

  }

}
