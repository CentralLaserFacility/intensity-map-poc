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

namespace Experiments_01_UWP
{

  public sealed partial class PanAndZoomSliders_UserControl : UserControl
  {

    public PanAndZoomSliders_UserControl ( )
    {
      this.InitializeComponent() ;
    }

    // Use Mediator instead ?
    public System.Action<double,double,double>? Changed ;

    private void Slider_ValueChanged ( object sender, RangeBaseValueChangedEventArgs e )
    {
      Common.DebugHelpers.WriteDebugLines(
        $"PanX={m_panX.Value} PanY={m_panY.Value} Scale={m_scale.Value}"
      ) ;
      Changed?.Invoke(
        m_panX.Value,
        m_panY.Value,
        m_scale.Value
      ) ;
    }

  }

}
