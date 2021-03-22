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

  public sealed partial class CanvasPanAndZoomExperiment_UserControl : UserControl
  {

    public CanvasPanAndZoomExperiment_UserControl ( )
    {
      this.InitializeComponent() ;
    }

    public void SetTransform ( double panX, double panY, double scale )
    {
      m_canvas.RenderTransform = new TransformGroup(){
        Children = {
          new TranslateTransform(){
            X = panX,
            Y = panY
          },
          new ScaleTransform(){
            ScaleX = scale,
            ScaleY = scale
          }
        }
      } ;
    }

  }

}
