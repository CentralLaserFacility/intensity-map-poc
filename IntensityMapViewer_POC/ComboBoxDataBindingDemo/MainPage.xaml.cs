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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ComboBoxDataBindingDemo
{

  public enum MyEnumType {
    OptionA,
    OptionB,
    OptionC
  } ;

  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page, INotifyPropertyChanged
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

        public MainPage()
        {
            this.InitializeComponent();
            MyEnumTypeOption = MyEnumType.OptionC ;
        }

        public IEnumerable<
          Common.EnumItemsSource<MyEnumType>
        > MyEnumTypeOptions { get ; }
        = Common.EnumItemsSource<MyEnumType>.ToList() ;

        private MyEnumType m_myEnumTypeOption = MyEnumType.OptionB ;
        public MyEnumType MyEnumTypeOption {
          get => m_myEnumTypeOption ;
          set => Set(ref m_myEnumTypeOption,value) ;
        }

    }
}
