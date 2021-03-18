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

using Common.ExtensionMethods ;
using Microsoft.Toolkit.Mvvm.Messaging;
using IntensityProfileViewer.ExtensionMethods;
using UwpUtilities ;

namespace IntensityProfileViewer
{

  public sealed partial class HorizontalProfileGraph_XAML_UserControl : UserControl
  {

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityProfileViewer.ISourceViewModel), 
      typeof(HorizontalProfileGraph_UserControl), 
      new PropertyMetadata(
        defaultValue : null,
        propertyChangedCallback : (dp,propertyChangedEventArgs) => {
          var userControlThatOwnsThisViewModelProperty = dp as HorizontalProfileGraph_XAML_UserControl ;
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

    public HorizontalProfileGraph_XAML_UserControl()
    {
      this.InitializeComponent();
      this.Loaded += (s,e) => {
        // Hmm, expected the Height and Width to be available at this point ?
        // IS THERE A RENDER-COMPLETE EVENT THAT WE COULD HOOK INTO ???
        this.Bindings.Update() ;
      } ;
      this.SizeChanged += (s,e) => {
        // Matteo : HELP, THIS DOESN'T WORK EITHER !!!
        // Aha, it's because of the TRANSFORM we've applied ...
        m_canvas.Clip = new RectangleGeometry() { 
          Rect = new Rect(
            0, 
            0, 
            m_canvas.ActualWidth, 
            m_canvas.ActualHeight
          )
        } ;
        // Surely there's an easier way to do this ???
        System.Numerics.Matrix4x4 transform = m_canvas.TransformMatrix ;
        System.Numerics.Vector3 v = new(1,1,0) ;
        v = System.Numerics.Vector3.Transform(v,transform) ;
        this.Bindings.Update() ;
      } ;
    }

    // Nasty HACK ...

    public IntensityMapImage_UserControl IntensityMapImage_UserControl ;

    private void OnViewModelPropertyChanged ( 
      IntensityProfileViewer.ISourceViewModel? oldViewModel,
      IntensityProfileViewer.ISourceViewModel? newViewModel
    ) {
      // m_skiaCanvas.PaintSurface += (s,paintSurfaceEventArgs) => {
      //   DrawHorizontalProfileGraph_IndividualLines(
      //     paintSurfaceEventArgs.Surface.Canvas
      //   ) ;
      //   // SkiaUtilities.DrawingHelpers.DrawBoundingBox(
      //   //   paintSurfaceEventArgs.Surface.Canvas
      //   // ) ;
      // } ;
      newViewModel.NewIntensityMapAcquired += () => PerformRepaint() ;
      newViewModel.ProfileDisplaySettings.ProfileGraphsReferencePositionChanged += () => PerformRepaint() ;
      newViewModel.Parent.IntensityMapVisualisationHasChanged += () => PerformRepaint() ;
    }

    private void PerformRepaint ( )
    {
      // skiaCanvas.SetMatrix(
      //   SkiaSceneRenderer.GetTransformParameters_HorizontalOnly(
      //     ViewModel.Parent.PanAndZoomParameters
      //   )
      // ) ;
      var transform = SkiaSceneRenderer.GetTransformParameters_HorizontalOnly(
        ViewModel.Parent.PanAndZoomParameters
      ) ;
      // this.Scale = new(
      //   transform.ScaleX,
      //   transform.ScaleY,
      //   1.0f
      // ) ;
      // this.Translation = new(
      //   transform.TransX,
      //   transform.TransY,
      //   0.0f
      // ) ;
      // Aha, this works !!! Applying the transform to
      // the graph data rather than to the Canvas itself ...
      Windows.UI.Xaml.Media.Transform graphPathTransform = new TransformGroup(){
        Children = {
          new ScaleTransform(){
            ScaleX = transform.ScaleX,
            ScaleY = transform.ScaleY             
          },
          new TranslateTransform(){
            X = transform.TransX,
            Y = transform.TransY
          }
        }
      } ;
      m_graphPath.RenderTransform = graphPathTransform ;
      this.Bindings.Update() ;
    }

    // public Matrix TransformMatrix { get ; private set ; }

    public Windows.UI.Xaml.Media.Geometry MyPathData => new GeometryGroup(){
      Children = {
        new LineGeometry() {
          StartPoint = new(120,10),
          EndPoint   = new(120,50)
        },
        new LineGeometry() {
          StartPoint = new(130,10),
          EndPoint   = new(130,50)
        },
      }
    } ;

    public Windows.UI.Xaml.Media.Geometry GetPathDataForGraph ( 
      IntensityProfileViewer.IIntensityMap mostRecentlyAcquiredIntensityMap 
    ) {
      GeometryGroup verticalLines_geometryGroup = new() ;
      // GeometryGroup joinedOutlinePoints_geometryGroup = new() ;
      PathGeometry joinedOutlinePoints_pathGeometry = new() ;
      int nPoints = mostRecentlyAcquiredIntensityMap.Dimensions.Width ;      
      if ( nPoints < 2 )
      {
        return null ; //////////////////////////////////
      }

      // if ( double.IsNaN(this.Width) )
      // {
      //   return null ; //////////////////////////////////
      // }

      // HACK - should get these from the Panel !!!
      // var panelWidth = 400.0 ;
      // var panelHeight = 100.0 ;
      var panelWidth = m_canvas.ActualWidth ;
      var panelHeight = m_canvas.ActualHeight ;

      // Point topLeftPoint     = new(0,0) ;    
      // Point topRightPoint    = new(panelWidth,0) ;    
      // Point bottomLeftPoint  = new(0,panelHeight) ;    
      // Point bottomRightPoint = new(panelWidth,panelHeight) ;

      Point topLeftPoint     = m_canvas.TransformToVisual(this).TransformPoint(new(0,0)) ;   
      Point topRightPoint    = topLeftPoint.MovedBy(panelWidth,0) ;    
      Point bottomLeftPoint  = topLeftPoint.MovedBy(0,panelHeight) ;    
      Point bottomRightPoint = topLeftPoint.MovedBy(panelWidth,panelHeight) ;   
      
      // We only want to make our graph as wide as the rectangle in which we're drawing the Image

      bottomRightPoint.X = bottomLeftPoint.X + IntensityMapImage_UserControl.RectInWhichToDrawBitmap.Width ;
      topRightPoint.X    = topLeftPoint.X    + IntensityMapImage_UserControl.RectInWhichToDrawBitmap.Width ;

      List<Point> points = new() ;
      var intensityValues = mostRecentlyAcquiredIntensityMap.HorizontalSliceAtRow(
        ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value.Y
      ).WithNormalisationApplied(
        new IntensityProfileViewer.Normaliser(
          ViewModel.Parent.ImagePresentationSettings.NormalisationValue
        )
      ) ;
      Point GetPointAtFractionalPositionAlongLine(
        Point  startPoint,
        Point  endPoint,
        double frac01
      ) => new Point(
        startPoint.X + frac01 * ( endPoint.X - startPoint.X ),
        startPoint.Y + frac01 * ( endPoint.Y - startPoint.Y )
      ) ;
      intensityValues.ForEachItem(
        (value,i) => {
          double lineLength = (
            (
              (double) panelHeight 
            )
          * value / 255.0
          ) ;
          var bottomAnchorPoint = GetPointAtFractionalPositionAlongLine(
            bottomLeftPoint.MovedBy(0,1),
            bottomRightPoint.MovedBy(0,1),
            i / (double) ( nPoints - 1 ) // ??????????????
          ) ;
          verticalLines_geometryGroup.Children.Add(
            new LineGeometry() {
              StartPoint = bottomAnchorPoint,
              EndPoint   = bottomAnchorPoint.MovedBy(0,-lineLength)
            }
          ) ;
          // joinedOutlinePoints_geometryGroup.Children.Add(
          //   new PointCollection() {
          //     StartPoint = bottomAnchorPoint,
          //     EndPoint   = bottomAnchorPoint.MovedBy(0,-lineLength)
          //   }
          // ) ;
          // skiaCanvas.DrawVerticalLineUp(
          //   bottomAnchorPoint,
          //   lineLength,
          //   i == iSpecial ? special : normal
          // ) ;
          points.Add(
            bottomAnchorPoint.MovedBy(0,-lineLength)
          ) ;
        }
      ) ;
      // Surely there's a better way to build the Segments ???
      var segmentsCollection = new PathSegmentCollection() ;
      var segments = points.Skip(1).Select(
        p => new LineSegment(){
          Point = p
        }
      ) ;
      segments.ForEachItem(
        segment => {
          // Yay, CRAP !! The F1 help mentions an Append method
          // that returns void, but it doesn't mention 'Add'.
          // So you'd suppose that Append would be the one to use.
          // https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.pathsegmentcollection.append?view=winrt-19041
          // However, in reality 'Append' is an extension method on IEnumerable
          // that returns a new collection without modifying the original.
          // Turns out that 'Add' IS AVAILABLE AND DOES WORK !!!
          // segmentsCollection.Append(segment) ;
          segmentsCollection.Add(segment) ;
        }
      ) ;
      // Yikes, after 'appending' segments the count is still zero !!!
      int nSegments = segmentsCollection.Count ;
      joinedOutlinePoints_pathGeometry.Figures.Add(
        new PathFigure(){
          StartPoint = points[0],
          Segments   = segmentsCollection
        }
      ) ;
      return new GeometryGroup() {
        Children = {
          verticalLines_geometryGroup,
          joinedOutlinePoints_pathGeometry
        } 
      } ;
    }

  }

}
