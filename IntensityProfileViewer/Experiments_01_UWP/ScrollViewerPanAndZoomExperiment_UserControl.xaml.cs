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

  public sealed partial class ScrollViewerPanAndZoomExperiment_UserControl : UserControl
  {

    private int m_nChanges = 0 ;

    public ScrollViewerPanAndZoomExperiment_UserControl ( )
    {
      this.InitializeComponent() ;
      m_panAndZoomSliders.Changed = (x,y,scale) => {
        m_scrollViewer.ChangeView(
          x,
          y,
          (float) scale
        ) ;
        //
        // Matteo : which Transform is responsible for implementing the 'ChangeView' settings ??
        // Inspection of the Live Visual Tree shows all the likely Transforms as unity ... ???
        //

        m_nChanges++ ;

        Transform renderTransform = m_scrollViewer.RenderTransform ;
        if ( renderTransform is  MatrixTransform matrixTransform )
        {
          Common.DebugHelpers.WriteDebugLines(
            $"ScrollViewer RenderTransform ({m_nChanges}) :",
            $"  {matrixTransform.Matrix.M11:F3} {matrixTransform.Matrix.M12:F3}",
            $"  {matrixTransform.Matrix.M21:F3} {matrixTransform.Matrix.M22:F3}",
            $"  {matrixTransform.Matrix.OffsetX:F3} {matrixTransform.Matrix.OffsetY:F3}"
          ) ;
        }

        {
          System.Numerics.Matrix4x4 transformMatrix = m_scrollViewer.TransformMatrix ;
          Common.DebugHelpers.WriteDebugLines(
            $"ScrollViewer TransformMatrix ({m_nChanges}) :",
            $"  {transformMatrix.M11:F3} {transformMatrix.M12:F3} {transformMatrix.M13:F3} {transformMatrix.M14:F3}",
            $"  {transformMatrix.M21:F3} {transformMatrix.M22:F3} {transformMatrix.M23:F3} {transformMatrix.M24:F3}",
            $"  {transformMatrix.M31:F3} {transformMatrix.M32:F3} {transformMatrix.M33:F3} {transformMatrix.M34:F3}",
            $"  {transformMatrix.M41:F3} {transformMatrix.M42:F3} {transformMatrix.M43:F3} {transformMatrix.M44:F3}"
          ) ;
        }

        Transform canvasRenderTransform = m_canvas.RenderTransform ;
        if ( canvasRenderTransform is  MatrixTransform canvasMatrixTransform )
        {
          Common.DebugHelpers.WriteDebugLines(
            $"Canvas RenderTransform ({m_nChanges}) :",
            $"  {canvasMatrixTransform.Matrix.M11:F3} {canvasMatrixTransform.Matrix.M12:F3}",
            $"  {canvasMatrixTransform.Matrix.M21:F3} {canvasMatrixTransform.Matrix.M22:F3}",
            $"  {canvasMatrixTransform.Matrix.OffsetX:F3} {canvasMatrixTransform.Matrix.OffsetY:F3}"
          ) ;
        }

        // Transform layoutTransform = m_scrollViewer.La ;

      } ;
    }

  }

}
