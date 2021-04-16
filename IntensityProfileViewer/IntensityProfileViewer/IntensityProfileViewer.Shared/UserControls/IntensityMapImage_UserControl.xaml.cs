﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using SkiaUtilities;
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
using System.Diagnostics.CodeAnalysis;
using Common.ExtensionMethods;
using IntensityProfileViewer.ExtensionMethods ;

// using static Microsoft.Toolkit.Mvvm.Messaging.IMessengerExtensions ;

namespace IntensityProfileViewer
{

  public sealed partial class IntensityMapImage_UserControl : UserControl
  {

    // MATTEO - please review ... as a general way of setting up the ViewModel.
    // We sometimes also need to be able to set the ViewModel from the DataContext
    // eg if the UserControl is specified in a DataTemplate for a ListView etc.
    // See below ...

    // Note : this seems like an ideal candidate for Source Generation,
    // where you could specify a [DependencyProperty] attribute on a partial declaration
    // and have the generator create the boilerplate code ...

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      nameof(ViewModel), 
      typeof(IntensityProfileViewer.ISourceViewModel), 
      typeof(IntensityMapImage_UserControl), // Matteo : is there a neat way to get the class type that owns the Property ?? OK, Reflection ...
      new PropertyMetadata(
        defaultValue : null,
        propertyChangedCallback : (dp,propertyChangedEventArgs) => {
          var userControlThatOwnsThisViewModelProperty = dp as IntensityMapImage_UserControl ;
          userControlThatOwnsThisViewModelProperty.OnViewModelPropertyChanged(
            propertyChangedEventArgs.OldValue as IntensityProfileViewer.ISourceViewModel,
            propertyChangedEventArgs.NewValue as IntensityProfileViewer.ISourceViewModel
          ) ;
        }
      )
    ) ;

    public IntensityProfileViewer.ISourceViewModel ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityProfileViewer.ISourceViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }

    private UwpSkiaUtilities.PanAndZoomGesturesHandler m_panAndZoomGesturesHandler ;

    public static bool SupportPanAndZoom = true ;

    public IntensityMapImage_UserControl ( )
    {
      InitializeComponent() ;
      // MATTEO - is this the right way to assign the ViewModel
      // From a DataContext that would be specified via a DataTemplate ??
      // this.DataContextChanged += (s,e) => {
      //   ViewModel = e.NewValue as IntensityProfileViewer.ISourceViewModel ;
      // } ;
    }

    // private SkiaSharp.SKCanvas? m_skiaCanvas ; // No no no ...

    private void OnViewModelPropertyChanged ( 
      IntensityProfileViewer.ISourceViewModel? oldViewModel,
      IntensityProfileViewer.ISourceViewModel? newViewModel
    ) {
      if ( SupportPanAndZoom )
      {
        m_panAndZoomGesturesHandler = new(
          m_skiaXamlCanvas,
          new SkiaSceneRenderer(DrawIntensityMap){
            ShowTransformMatrixInfo = false,
            RenderHookAction = (skiaCanvas) => {
              // Hmm, this seems to be the *only* way we can
              // get access to the Skia Canvas associated with
              // the Xaml Canvas ...    
              // BUT ... Yikes !! The 'skiaCanvas' given to us here
              // is only valid for the duration of this call.
              // Accessing it outside of this context triggers
              // a memory access exception !!!
              // m_skiaCanvas = skiaCanvas ;
              SkiaSceneRenderer.LoadPanAndZoomParameters(
                newViewModel.Parent.PanAndZoomParameters,
                skiaCanvas.TotalMatrix
              ) ;
            },
          }
        ) {
          TouchActionDetected = TouchActionDetected
        } ;
      }
      else
      {
        m_skiaXamlCanvas.PaintSurface += DrawSkiaContent ;
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
      m_skiaXamlCanvas.Invalidate() ;
    }

    public void ResetPanAndZoom ( )
    {
      m_panAndZoomGesturesHandler.ResetPanAndZoom() ;
      PerformRepaint() ;
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
           m_horizontalLine?.CoincidesWithMousePosition(m_mostRecentlyNotifiedPointerPosition_sceneCoordinates) is true  
        && ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.HasValue 
        ) {
          m_profileGraphsReferencePositionBeforeDragStarted = ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value ;
          m_horizontalLineDraggingInProgress = true ;
          handled = true ;
        }
        if ( 
           m_verticalLine?.CoincidesWithMousePosition(m_mostRecentlyNotifiedPointerPosition_sceneCoordinates) is true  
        && ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.HasValue 
        ) {
          m_profileGraphsReferencePositionBeforeDragStarted = ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value ;
          m_verticalLineDraggingInProgress = true ;
          handled = true ;
        }
        break ;
      case TouchTracking.TouchActionType.Moved:
        m_mostRecentlyNotifiedPointerPosition_sceneCoordinates = positionInSceneCoordinates ;
        int deltaRight = 0 ;
        int deltaDown = 0 ;
        if ( m_horizontalLineDraggingInProgress )
        {
          if ( 
            m_pixelToSceneCoordinatesMapper.CanGetPointInPixelCoordinates(
              m_mostRecentlyNotifiedPointerPosition_sceneCoordinates,
              out var mostRecentlyNotifiedPointerPosition_pixelCoordinates
            )
          ) {
            deltaDown = (
              mostRecentlyNotifiedPointerPosition_pixelCoordinates.Value.Y
            - m_profileGraphsReferencePositionBeforeDragStarted.Value.Y
            ) ;
          }
        }
        if ( m_verticalLineDraggingInProgress )
        {
          if ( 
            m_pixelToSceneCoordinatesMapper.CanGetPointInPixelCoordinates(
              m_mostRecentlyNotifiedPointerPosition_sceneCoordinates,
              out var mostRecentlyNotifiedPointerPosition_pixelCoordinates
            )
          ) {
            deltaRight = (
              mostRecentlyNotifiedPointerPosition_pixelCoordinates.Value.X
            - m_profileGraphsReferencePositionBeforeDragStarted.Value.X
            ) ;
          }
        }
        if ( 
           deltaRight != 0 
        || deltaDown  != 0
        ) {
          var updatedReferencePosition = new System.Drawing.Point(
            (
              m_profileGraphsReferencePositionBeforeDragStarted.Value.X + deltaRight
            ).ClampedToInclusiveRange(
              0,
              ViewModel.MostRecentlyAcquiredIntensityMap.Dimensions.Width - 1
            ),
            (
              m_profileGraphsReferencePositionBeforeDragStarted.Value.Y + deltaDown
            ).ClampedToInclusiveRange(
              0,
              ViewModel.MostRecentlyAcquiredIntensityMap.Dimensions.Height - 1
            )
          ) ;
          Common.DebugHelpers.WriteDebugLines(
            $"Adjusting ProfileGraphsReferencePosition by [{deltaRight},{deltaDown}] => [{updatedReferencePosition.X},{updatedReferencePosition.Y}]"
          ) ;
          ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition = updatedReferencePosition ;
          // Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send(
          //   new ReferencePositionChangedMessage(
          //     updatedReferencePosition.X,
          //     updatedReferencePosition.Y
          //   )
          // ) ;
        }
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

    #if __ANDROID__
    #elif NETFX_CORE
    #endif

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

    private SkiaUtilities.HorizontalLine? m_horizontalLine = null ;

    private SkiaUtilities.VerticalLine?   m_verticalLine   = null ;

    private SkiaUtilities.PixelToSceneCoordinatesMapper m_pixelToSceneCoordinatesMapper ;

    public SkiaSharp.SKRect RectInWhichToDrawBitmap { get ; private set ; }

    private void DrawIntensityMap ( SkiaSharp.SKCanvas skiaCanvas )
    { 
      ViewModel.Parent.RaiseIntensityMapVisualisationHasChangedEvent() ;
      // if ( m_performMatrixResetOnNextRepaint )
      // {
      //   // Hmm, this works - but of course it doesn't affect the transform
      //   // that has been set on the Scene ... so on the next repaint,
      //   // we'll see the previously active transform ...
      //   skiaCanvas.SetMatrix(
      //     SkiaSharp.SKMatrix.CreateIdentity()
      //   ) ;
      //   m_performMatrixResetOnNextRepaint = false ;
      // }
      var deviceClipBounds = skiaCanvas.DeviceClipBounds ;
      // if ( ViewModel != null )
      { 
        // Hmm, should try to eliminate this test ...
        var intensityMap = ViewModel.MostRecentlyAcquiredIntensityMap ;
        var bitmap = new SkiaSharp.SKBitmap(
          intensityMap.Dimensions.Width,
          intensityMap.Dimensions.Height
        ) ;
        var colourMapOption = ViewModel.Parent.ImagePresentationSettings.ColourMapOption ;
        var colourMapper = IntensityProfileViewer.ColourMapper.InstanceFor(colourMapOption) ;
        #if true
          // Hmm, creating the pixels array typically takes 10-15mS for a 320x240 image,
          // so we should optimise this by using Span<> and amalgamating the Normalisation
          // with the Colour Mapping computations ...
          bitmap.Pixels = intensityMap.IntensityValues.WithNormalisationApplied(
            new IntensityProfileViewer.Normaliser(
              ViewModel.Parent.ImagePresentationSettings.NormalisationValue
            )
          ).Select(
            intensity => new SkiaSharp.SKColor(
              colourMapper.MapByteValueToEncodedARGB(
                intensity
              )
            )
          ).ToArray() ;
        #else
          // Just for testing - leave the pixel values unset, and
          // see how long it takes to render the bitmap.
          var normalisationValue = ViewModel.Parent.ImagePresentationSettings.NormalisationValue ;
          var normalisationGainValue = 255.0 / normalisationValue ;
          // static byte ApplyNormalisationValue ( byte nominalIntensity, double normalisationGainValue )
          // {
          //   double intensityWithGainApplied = nominalIntensity * normalisationGainValue ;
          //   return (
          //     intensityWithGainApplied > 255.0
          //     ? (byte) 255
          //     : (byte) intensityWithGainApplied
          //   ) ;
          // }
          // -------------------------------------
          // bitmap.Pixels = intensityMap.IntensityValues.Select(
          //   intensity => new SkiaSharp.SKColor(
          //     // colourMapper.MapByteValueToEncodedARGB(
          //     //   ApplyNormalisationValue(
          //     //     intensity,
          //     //     normalisationGainValue
          //     //   )
          //     // )
          //     red   : intensity,
          //     green : intensity,
          //     blue  : intensity
          //   )
          // ).ToArray() ;
        #endif
        // We'll want to preserve the aspect ratio
        float expansionFactorX = deviceClipBounds.Width  / (float) intensityMap.Dimensions.Width ;
        float expansionFactorY = deviceClipBounds.Height / (float) intensityMap.Dimensions.Height ;
        float expansionFactor = System.MathF.Min(
          expansionFactorX,
          expansionFactorY
        ) ;
        RectInWhichToDrawBitmap = new SkiaSharp.SKRect(
          left   : 0.0f,
          top    : 0.0f,
          // right  : deviceClipBounds.Width,
          right : intensityMap.Dimensions.Width * expansionFactor, 
          // bottom : deviceClipBounds.Height
          bottom : intensityMap.Dimensions.Height * expansionFactor 
        ) ;
        m_pixelToSceneCoordinatesMapper = new SkiaUtilities.PixelToSceneCoordinatesMapper(
          intensityMap.Dimensions,
          RectInWhichToDrawBitmap.Size
        ) ;
        skiaCanvas.DrawBitmap(
          bitmap,
          RectInWhichToDrawBitmap
        ) ;
        var dragMarkerStyle = new SkiaSharp.SKPaint(){
          Color       = SkiaColourChoices.ImageDragMarkerColour
          // StrokeWidth = 3
        } ;
        float zoomCompensationFactor = 1.0f / skiaCanvas.TotalMatrix.ScaleX ; 
        if ( m_mostRecentlyNotifiedPointerPosition_sceneCoordinates.HasValue )
        {
          float ovalDiameter = SkiaColourChoices.ImageDragMarkerDiameter ; // 4.0f ;
          skiaCanvas.DrawOval(
            m_mostRecentlyNotifiedPointerPosition_sceneCoordinates.Value,
            new SkiaSharp.SKSize(
              ovalDiameter * zoomCompensationFactor,
              ovalDiameter * zoomCompensationFactor
              // m_inContact ? 10.0f : 5.0f,
              // m_inContact ? 10.0f : 5.0f
            ),
            dragMarkerStyle
          ) ;
        }
        if ( 
           ViewModel.Parent.CurrentSource.ProfileDisplaySettings.ShouldShowProfileGraphs
        && m_pixelToSceneCoordinatesMapper.CanGetPointInSceneCoordinates(
             ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition,
             out var referencePointInSceneCoordinates
           )
        ) {
          // m_pixelToSceneCoordinatesMapper.CanGetPointInPixelCoordinates(
          //   referencePointInSceneCoordinates,
          //   out var referencePositionInPixels 
          // ) ;
          m_horizontalLine = new SkiaUtilities.HorizontalLine(
            referencePointInSceneCoordinates.Value,
            0.0f,
            RectInWhichToDrawBitmap.Width
          ) ;
          m_verticalLine = new SkiaUtilities.VerticalLine(
            referencePointInSceneCoordinates.Value,
            0.0f,
            RectInWhichToDrawBitmap.Height
          ) ;
          var horizontalLineStyle = new SkiaSharp.SKPaint(){
            Color       = SkiaColourChoices.ImageReferenceLineColour,
            StrokeWidth = zoomCompensationFactor * (
              m_horizontalLine.CoincidesWithMousePosition(m_mostRecentlyNotifiedPointerPosition_sceneCoordinates)  
              ? SkiaColourChoices.ImageReferenceLineHighlightedWidth
              : SkiaColourChoices.ImageReferenceLineNominalWidth 
            )
          } ;        
          var verticalLineStyle = new SkiaSharp.SKPaint(){
            Color       = SkiaColourChoices.ImageReferenceLineColour,
            StrokeWidth = zoomCompensationFactor * (
              m_verticalLine.CoincidesWithMousePosition(m_mostRecentlyNotifiedPointerPosition_sceneCoordinates)   
              ? SkiaColourChoices.ImageReferenceLineHighlightedWidth
              : SkiaColourChoices.ImageReferenceLineNominalWidth 
            )
          } ;
          m_horizontalLine.Draw(skiaCanvas,horizontalLineStyle) ;
          m_verticalLine.Draw(skiaCanvas,verticalLineStyle) ;
        }
        else
        {
          m_horizontalLine = null ;
          m_verticalLine   = null ;
        }
        SkiaSharp.SKPaint textPaint = new() { 
          Color       = SkiaColourChoices.ReferencePositionTextColour,
          IsAntialias = true,
          TextSize    = zoomCompensationFactor * 16.0f,
          Typeface    = SkiaSharp.SKTypeface.FromFamilyName(
            "Courier",
            SkiaSharp.SKFontStyle.Normal
          )
        } ;
        // skiaCanvas.DrawCircle(
        //  cx     : 0.0f,
        //  cy     : 0.0f,
        //  radius : 10.0f,
        //  redPaint
        //  ;
        if ( 
           m_mostRecentlyNotifiedPointerPosition_sceneCoordinates.HasValue
        && m_pixelToSceneCoordinatesMapper.CanGetPointInPixelCoordinates(
             m_mostRecentlyNotifiedPointerPosition_sceneCoordinates,
             out var pointerPositionInPixels 
           )
        ) { 
          var intensityValue = ViewModel.MostRecentlyAcquiredIntensityMap.GetIntensityValueAt(
            pointerPositionInPixels.Value.X,
            pointerPositionInPixels.Value.Y
          ) ;
          // string label = $"[{ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value.X},{ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value.Y}] : {intensityValue}" ;
          string label = $"{pointerPositionInPixels.Value.ToPixelPositionString()} {intensityValue}" ;
          skiaCanvas.DrawText(
            label,
            m_mostRecentlyNotifiedPointerPosition_sceneCoordinates.Value.MovedBy(
              +10.0f * zoomCompensationFactor,
              -20.0f * zoomCompensationFactor
            ),
            textPaint
          ) ;
        }

      }

    }

  }

}
