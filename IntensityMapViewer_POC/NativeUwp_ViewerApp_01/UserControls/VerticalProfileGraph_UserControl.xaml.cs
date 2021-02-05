using System;
using System.Collections.Generic;
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

namespace NativeUwp_ViewerApp_01
{
  public sealed partial class VerticalProfileGraph_UserControl : UserControl
  {

    // private IntensityMapViewer.ISourceViewModel ViewModel => DataContext as IntensityMapViewer.ISourceViewModel ;

    // private IntensityMapViewer.IDisplayPanelViewModel RootViewModel => ViewModel.Parent ;

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityMapViewer.ISourceViewModel), 
      typeof(VerticalProfileGraph_UserControl), 
      new PropertyMetadata(0)
    ) ;

    public IntensityMapViewer.ISourceViewModel ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityMapViewer.ISourceViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }

    public VerticalProfileGraph_UserControl()
    {
      this.InitializeComponent();
      // DataContextChanged += (s,e) => {
      //   System.Diagnostics.Debug.WriteLine(
      //     $"{this.GetType()} DataContext => {DataContext?.GetType().ToString()??"null"}"
      //   ) ;
      //   this.Bindings.Update() ; // Yikes - gotta call this explicitly ? WTF !!!
      // } ;
    }
  }
}
