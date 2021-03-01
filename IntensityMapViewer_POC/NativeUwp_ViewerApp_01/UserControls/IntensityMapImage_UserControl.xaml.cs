using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UwpSkiaUtilities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace NativeUwp_ViewerApp_01
{

  public sealed partial class IntensityMapImage_UserControl : UserControl
  {

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityMapViewer.ISourceViewModel), 
      typeof(IntensityMapImage_UserControl), 
      new PropertyMetadata(
        defaultValue : null,
        propertyChangedCallback : (dp,propertyChangedEventArgs) => {
          var userControlThatOwnsThisViewModelProperty = dp as IntensityMapImage_UserControl ;
          userControlThatOwnsThisViewModelProperty.OnViewModelPropertyChanged(
            propertyChangedEventArgs.OldValue as IntensityMapViewer.ISourceViewModel,
            propertyChangedEventArgs.NewValue as IntensityMapViewer.ISourceViewModel
          ) ;
        }
      )
    ) ;

    public IntensityMapViewer.ISourceViewModel ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityMapViewer.ISourceViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }

    private UwpSkiaUtilities.PanAndZoomAndRotationGesturesHandler m_panAndZoomAndRotationGesturesHandler ;

    public static bool SupportPanAndZoom = true ;

    public IntensityMapImage_UserControl ( )
    {
      InitializeComponent() ;
    }

    private void OnViewModelPropertyChanged ( 
      IntensityMapViewer.ISourceViewModel? oldViewModel,
      IntensityMapViewer.ISourceViewModel? newViewModel
    ) {
      if ( SupportPanAndZoom )
      {
        m_panAndZoomAndRotationGesturesHandler = new(
          m_skiaCanvas,
          new SkiaSceneRenderer(DrawIntensityMap){
            ShowTransformMatrixInfo = true,
            RenderHook = (canvas) => {
              SkiaSceneRenderer.LoadPanAndZoomParameters(
                newViewModel.Parent.PanAndZoomParameters,
                canvas.TotalMatrix
              ) ;
            },
          }
        ) {
          TouchActionDetected = TouchActionDetected
        } ;
      }
      else
      {
        m_skiaCanvas.PaintSurface += DrawSkiaContent ;
      }
      newViewModel.NewIntensityMapAcquired += () => PerformRepaint() ;
      newViewModel.ProfileDisplaySettings.ProfileGraphsReferencePositionChanged += () => {
        Common.DebugHelpers.WriteDebugLines(
          $"ProfileGraphsReferencePositionChanged => {newViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition}"
        ) ;
        PerformRepaint() ;
      } ;
      ViewModel.Parent.ImagePresentationSettings.PropertyChanged += (s,e) => {
        PerformRepaint() ;
      } ;
    }

    private void PerformRepaint ( )
    {
      m_skiaCanvas.Invalidate() ;
    }

    //
    // Dragging works as follows :
    //
    // When a 'touch' occurs, it is raised as an event on the skia Canvas.
    // Our 'PanAndZoomAndRotationGesturesHandler' will have hooked into that event,
    // and by default will pass it on to a helper class that will perform adjustments
    // to the Transform Matrix as required to implement panning and zooming.
    //
    // Ordinarily we'll allow the default 'pan/zoom' actions to be performed.
    // However under certain circimstances, we'll disable that behaviour and instead
    // use 'drag' operations to adjust the Reference Position.
    //
    
    private System.Drawing.Point? m_profileGraphsReferencePositionBeforeDragStarted = null ;

    private bool m_horizontalLineDraggingInProgress = false ;

    private bool m_verticalLineDraggingInProgress   = false ;

    private SkiaSharp.SKPoint? m_mostRecentlyNotifiedPointerPosition_sceneCoordinates = null ;
    private bool               m_inContact                                            = false ;

    private bool TouchActionDetected (
      TouchTracking.TouchActionType actionType, 
      SkiaSharp.SKPoint             positionInSceneCoordinates,
      bool                          inContact
    ) {
      bool handled = false ;
      m_inContact = inContact ;
      switch ( actionType )
      {
      case TouchTracking.TouchActionType.Entered:
        m_mostRecentlyNotifiedPointerPosition_sceneCoordinates = positionInSceneCoordinates ;
        break ;
      case TouchTracking.TouchActionType.Pressed:
        m_mostRecentlyNotifiedPointerPosition_sceneCoordinates = positionInSceneCoordinates ;
        if ( 
           m_horizontalLine?.CoincidesWithMousePosition(m_mostRecentlyNotifiedPointerPosition_sceneCoordinates.Value) is true  
        && ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.HasValue 
        ) {
          m_profileGraphsReferencePositionBeforeDragStarted = ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value ;
          m_horizontalLineDraggingInProgress = true ;
          handled = true ;
        }
        if ( 
           m_verticalLine?.CoincidesWithMousePosition(m_mostRecentlyNotifiedPointerPosition_sceneCoordinates.Value) is true  
        && ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.HasValue 
        ) {
          m_profileGraphsReferencePositionBeforeDragStarted = ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value ;
          m_verticalLineDraggingInProgress = true ;
          handled = true ;
        }
        break ;
      case TouchTracking.TouchActionType.Moved:
        m_mostRecentlyNotifiedPointerPosition_sceneCoordinates = positionInSceneCoordinates ;
        if ( m_horizontalLineDraggingInProgress )
        {
          var deltaDown = (int) (
            m_mostRecentlyNotifiedPointerPosition_sceneCoordinates.Value.Y
          - m_profileGraphsReferencePositionBeforeDragStarted.Value.Y
          ) ;
          var updatedReferencePosition = new System.Drawing.Point(
            m_profileGraphsReferencePositionBeforeDragStarted.Value.X,
            m_profileGraphsReferencePositionBeforeDragStarted.Value.Y + deltaDown
          ) ;
          Common.DebugHelpers.WriteDebugLines(
            $"Adjusting ProfileGraphsReferencePosition.Y by {deltaDown} => [{updatedReferencePosition.X},{updatedReferencePosition.Y}]"
          ) ;
          ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition = updatedReferencePosition ;
        }
        // if ( m_horizontalLine?.CoincidesWithMousePosition(m_mostRecentlyNotifiedPointerPosition_sceneCoordinates.Value) is true )
        // {
        //   handled = true ;
        // }
        // if ( m_verticalLine?.CoincidesWithMousePosition(m_mostRecentlyNotifiedPointerPosition_sceneCoordinates.Value) is true )
        // {
        //   handled = true ;
        // }
        break ;
      case TouchTracking.TouchActionType.Released:
        m_profileGraphsReferencePositionBeforeDragStarted = null ;
        m_horizontalLineDraggingInProgress = false ;
        m_verticalLineDraggingInProgress = false ;
        break ;
      case TouchTracking.TouchActionType.Cancelled:
        break ;
      case TouchTracking.TouchActionType.Exited:
        m_mostRecentlyNotifiedPointerPosition_sceneCoordinates = null ;
        break ;
      }
      PerformRepaint() ;
      return handled ;
    }

    private void DrawSkiaContent ( 
      object                                      sender, 
      SkiaSharp.Views.UWP.SKPaintSurfaceEventArgs paintSurfaceEventArgs
    ) {
      SkiaSharp.SKCanvas skiaCanvas = paintSurfaceEventArgs.Surface.Canvas ;
      SkiaSharp.SKRectI deviceClipBounds = skiaCanvas.DeviceClipBounds ;
      Common.DebugHelpers.WriteDebugLines(
        $"Skia.Canvas.DeviceClipBounds : [{deviceClipBounds.Left},{deviceClipBounds.Top}] size [{deviceClipBounds.Width}x{deviceClipBounds.Height}]"
      ) ;
      // All we need is the Canvas - we can query the ImageInfo to get the dimensions
      // but that gives us the same info we'll get from the 'device clip bounds'
      SkiaSharp.SKImageInfo imageInfo = paintSurfaceEventArgs.Info ;
      Common.DebugHelpers.WriteDebugLines(
        $"SKImageInfo : size [{imageInfo.Width}x{imageInfo.Height}]"
      ) ;
      // SkiaSharp.SKRect localClipBounds = skiaCanvas.LocalClipBounds ;
      // Common.DebugHelpers.WriteDebugLines(
      //   $"Skia.Canvas.LocalClipBounds : [{localClipBounds.Left},{localClipBounds.Top}] size [{localClipBounds.Width}x{localClipBounds.Height}]"
      // ) ;
      DrawIntensityMap(skiaCanvas) ;
    }

    private HorizontalLine? m_horizontalLine = null ;

    private VerticalLine?   m_verticalLine = null ;

    public class PixelToSceneCoordinatesMapper
    {
      public System.Drawing.Size PixelDimensions { get ; }
      public SkiaSharp.SKSize SceneDimensions { get ; }
      public PixelToSceneCoordinatesMapper (
        System.Drawing.Size pixelDimensions,
        SkiaSharp.SKSize    sceneDimensions
      ) {
        PixelDimensions = pixelDimensions ;
        SceneDimensions = sceneDimensions ;
      }
      public SkiaSharp.SKPoint? GetPointInSceneCoordinates ( 
        System.Drawing.Point? pointInPixelCoordinates
      ) => (
        pointInPixelCoordinates.HasValue
        ? new SkiaSharp.SKPoint(
            (int) Scale(
              pointInPixelCoordinates.Value.X,
              PixelDimensions.Width,
              SceneDimensions.Width
            ),
            (int) Scale(
              pointInPixelCoordinates.Value.Y,
              PixelDimensions.Height,
              SceneDimensions.Height
            )
          )
        : null
      ) ;
      private static double Scale ( double value, double nImagePixels, double nDisplayPixels )
      => (
        value * nDisplayPixels / nImagePixels
      ) ;
    }

    private void DrawIntensityMap ( SkiaSharp.SKCanvas skiaCanvas )
    { 
      ViewModel.Parent.RaiseIntensityMapVisualisationHasChangedEvent() ;
      var deviceClipBounds = skiaCanvas.DeviceClipBounds ;
      // Draw a diagonal line (debugging)
      // skiaCanvas.DrawLine(
      //   new SkiaSharp.SKPoint(
      //     deviceClipBounds.Location.X,
      //     deviceClipBounds.Location.Y
      //   ),
      //   new SkiaSharp.SKPoint(
      //     deviceClipBounds.Location.X + deviceClipBounds.Width  - 1,
      //     deviceClipBounds.Location.Y + deviceClipBounds.Height - 1
      //   ),
      //   new SkiaSharp.SKPaint(){
      //     Color = SkiaSharp.SKColors.Red
      //   }
      // ) ;
      if ( ViewModel != null )
      { 
        // Hmm, should try to eliminate this test ...
        var intensityMap = ViewModel.MostRecentlyAcquiredIntensityMap ;
        var bitmap = new SkiaSharp.SKBitmap(
          intensityMap.Dimensions.Width,
          intensityMap.Dimensions.Height
        ) ;
        var colourMapOption = ViewModel.Parent.ImagePresentationSettings.ColourMapOption ;
        var colourMapper = IntensityMapViewer.ColourMapper.InstanceFor(colourMapOption) ;
        bitmap.Pixels = intensityMap.IntensityValues.Select(
          intensity => new SkiaSharp.SKColor(
            colourMapper.MapByteValueToEncodedARGB(intensity)
            // red   : intensity,
            // green : intensity,
            // blue  : intensity
          )
        ).ToArray() ;
        SkiaSharp.SKRect rectInWhichToDrawBitmap = new SkiaSharp.SKRect(
          left   : 0.0f,
          top    : 0.0f,
          right  : deviceClipBounds.Width,
          bottom : deviceClipBounds.Height
        ) ;
        skiaCanvas.DrawBitmap(
          bitmap,
          rectInWhichToDrawBitmap
        ) ;
        var lineStyle = new SkiaSharp.SKPaint(){
          Color       = SkiaSharp.SKColors.Red,
          StrokeWidth = 3
        } ;
        if ( m_mostRecentlyNotifiedPointerPosition_sceneCoordinates.HasValue )
        {
          skiaCanvas.DrawCircle(
            m_mostRecentlyNotifiedPointerPosition_sceneCoordinates.Value,
            m_inContact ? 10.0f : 5.0f ,
            lineStyle
          ) ;
        }
        if ( ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.HasValue )
        {
          // var referencePosition = ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value ;
          // int xAlongFromLeft = Scale(
          //   referencePosition.X,
          //   intensityMap.Dimensions.Width,
          //   deviceClipBounds.Width
          // ) ;
          // int yDownFromTop = Scale(
          //   ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value.Y,
          //   intensityMap.Dimensions.Height,
          //   deviceClipBounds.Height
          // ) ;
          // var scaledReferencePoint = new SkiaSharp.SKPoint(
          //   xAlongFromLeft,
          //   yDownFromTop
          // ) ;

          PixelToSceneCoordinatesMapper pixelToSceneCoordinatesMapper = new PixelToSceneCoordinatesMapper(
            intensityMap.Dimensions,
            rectInWhichToDrawBitmap.Size
          ) ;
          var scaledReferencePoint = pixelToSceneCoordinatesMapper.GetPointInSceneCoordinates(
            ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition
          ).Value ;

          m_horizontalLine = new HorizontalLine(
            scaledReferencePoint,
            0.0f,
            deviceClipBounds.Width
          ) ;
          m_verticalLine = new VerticalLine(
            scaledReferencePoint,
            0.0f,
            deviceClipBounds.Height
          ) ;
          m_horizontalLine.Draw(skiaCanvas,lineStyle) ;
          m_verticalLine.Draw(skiaCanvas,lineStyle) ;
        }
        else
        {
          m_horizontalLine = null ;
          m_verticalLine   = null ;
        }
        static int Scale ( double value, double nImagePixels, double nDisplayPixels )
        => (int) (
          value * nDisplayPixels / nImagePixels
        ) ;
      }
    }

  }

}
