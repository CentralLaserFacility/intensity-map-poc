﻿using System;
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
  public sealed partial class IntensityMapImageAndProfileGraphs_UserControl : UserControl
  {

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityMapViewer.IDisplayPanelViewModel), 
      typeof(IntensityMapImageAndProfileGraphs_UserControl), 
      new PropertyMetadata(0)
    ) ;

    public IntensityMapViewer.IDisplayPanelViewModel ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityMapViewer.IDisplayPanelViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }

    public IntensityMapImageAndProfileGraphs_UserControl ( )
    {
      InitializeComponent() ;
    }

    public Visibility VisibleWhen ( bool visible )
    => (
      visible
      ? Visibility.Visible
      : Visibility.Collapsed
    ) ;

    public int ChooseIntegerValue ( 
      bool condition, 
      int valueWhenTrue, 
      int valueWhenFalse 
    ) => (
      condition 
      ? valueWhenTrue 
      : valueWhenFalse 
    ) ;

  }

}
