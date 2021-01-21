using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;

namespace WindowsTemplateStudioApp_02.Views
{
  public sealed partial class IntensityMap_UWPPage : Page, INotifyPropertyChanged
  {

    public event PropertyChangedEventHandler PropertyChanged ;

    private void Set<T> ( ref T storage, T value, [CallerMemberName] string propertyName = null )
    {
      if ( Equals(storage,value) )
      {
        return;
      }
      storage = value;
      OnPropertyChanged(propertyName) ;
    }

    private void OnPropertyChanged ( string propertyName)
    => PropertyChanged?.Invoke(
      this,
      new PropertyChangedEventArgs(propertyName)
    ) ;

    public IntensityMap_UWPPage ( )
    {
        InitializeComponent() ;
    }

  }

}
