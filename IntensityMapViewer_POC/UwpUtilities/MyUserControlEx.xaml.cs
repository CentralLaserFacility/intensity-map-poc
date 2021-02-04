using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

  public class UserControlEx : UserControl
  {
  }

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
    public string Name => "Hello" ;
  }

  public sealed partial class MyUserControlEx : UserControl
  {

    public ViewModelForMyUserControlEx ViewModel
    {
      get { return (ViewModelForMyUserControlEx) GetValue(ViewModelProperty) ; }
      set { SetValue(ViewModelProperty,value) ; }  
    }

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      nameof(ViewModel), 
      typeof(ViewModelForMyUserControlEx), 
      typeof(MyUserControlEx), 
      new PropertyMetadata(0)
    ) ;

    public MyUserControlEx ( )
    {
      this.InitializeComponent() ;
      ViewModel = new ViewModelForMyUserControlEx() ;
    }

  }

}
