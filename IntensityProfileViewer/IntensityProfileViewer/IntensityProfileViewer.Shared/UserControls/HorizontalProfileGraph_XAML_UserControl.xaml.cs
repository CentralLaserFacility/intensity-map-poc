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
        // https://www.domysee.com/blogposts/canvas-rendering-out-of-bounds
        // Matteo : HELP, THIS DOESN'T WORK EITHER !!!
        // Aha, it's because of the TRANSFORM we've applied ...
        // It works when we apply the transform to the Path :)
        m_canvas.Clip = new RectangleGeometry() { 
          Rect = new Rect(
            0, 
            0, 
            m_canvas.ActualWidth, 
            m_canvas.ActualHeight
          )
        } ;
        this.Bindings.Update() ;
      } ;

      // Surely there's an easier way to do this ???
      System.Numerics.Matrix4x4 transform = m_canvas.TransformMatrix ;
      System.Numerics.Vector3 v = new(1,1,0) ;
      v = System.Numerics.Vector3.Transform(v,transform) ;

      m_executionTimingStopwatch.Start() ;
    }

    private readonly System.Diagnostics.Stopwatch m_executionTimingStopwatch = new() ;

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
      GraphPathTransform = new TransformGroup(){
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

      // This works, but it's better to apply the transform
      // to the PathData that we're binding to ...
      // m_graphPath.RenderTransform = GraphPathTransform ;
      this.Bindings.Update() ;
    }

    // https://docs.microsoft.com/en-us/dotnet/api/system.numerics.matrix4x4?view=net-5.0
    // https://docs.microsoft.com/en-us/dotnet/api/system.numerics.matrix4x4.transform?view=net-5.0
    // https://docs.microsoft.com/en-us/windows/uwp/design/layout/transforms
    // https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.transform

    // In principle we could x:Bind the Path.RenderTransform to this ...
    private Windows.UI.Xaml.Media.Transform GraphPathTransform = new ScaleTransform() ;

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

    // private GeometryCollection m_geometryCollection ;

    public Windows.UI.Xaml.Media.Geometry GetPathDataForGraph ( 
      IntensityProfileViewer.IIntensityMap mostRecentlyAcquiredIntensityMap 
    ) {

      IReadOnlyList<byte> intensityValues = mostRecentlyAcquiredIntensityMap.HorizontalSliceAtRow(
        ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value.Y
      ).WithNormalisationApplied(
        new IntensityProfileViewer.Normaliser(
          ViewModel.Parent.ImagePresentationSettings.NormalisationValue
        )
      ) ;
      int nPoints = intensityValues.Count ;      
      if ( nPoints < 2 )
      {
        return null ;
      }

      // if ( double.IsNaN(this.Width) )
      // {
      //   return null ; //////////////////////////////////
      // }

      // HACK - should get these from the Panel !!!
      // var panelWidth = 400.0 ;
      // var panelHeight = 100.0 ;
      var panelWidth  = m_canvas.ActualWidth ;
      var panelHeight = m_canvas.ActualHeight ;

      if ( 
         panelWidth  == 0.0 
      || panelHeight == 0.0 
      ) {
        // Hmm, this can happen !
        return null ;
      }

      // Point topLeftPoint     = new(0,0) ;    
      // Point topRightPoint    = new(panelWidth,0) ;    
      // Point bottomLeftPoint  = new(0,panelHeight) ;    
      // Point bottomRightPoint = new(panelWidth,panelHeight) ;

      // https://codedocu.com/Details?d=1549&a=9&f=181&l=0&v=d&t=UWP:-Determine-Point-Coordinates-of-elements-in-app-,-Position
      Point topLeftPoint = m_canvas.TransformToVisual(this).TransformPoint(
        new Point(0,0)
      ) ;

      Point topRightPoint    = topLeftPoint.MovedBy(panelWidth,0) ;    
      Point bottomLeftPoint  = topLeftPoint.MovedBy(0,panelHeight-1) ;    
      Point bottomRightPoint = topLeftPoint.MovedBy(panelWidth,panelHeight-1) ;   
      
      // We only want to make our graph as wide as the rectangle in which we're drawing the Image

      if ( IntensityMapImage_UserControl.RectInWhichToDrawBitmap.Width == 0.0 )
      {
        // This can happen !!
        return null ;
      }

      bottomRightPoint.X = bottomLeftPoint.X + IntensityMapImage_UserControl.RectInWhichToDrawBitmap.Width ;
      topRightPoint.X    = topLeftPoint.X    + IntensityMapImage_UserControl.RectInWhichToDrawBitmap.Width ;

      System.TimeSpan timeBeforePathDataBuildStarted = m_executionTimingStopwatch.Elapsed ;

      GeometryGroup verticalLines_geometryGroup = new() ;
      // GeometryGroup joinedOutlinePoints_geometryGroup = new() ;
      PathGeometry joinedOutlinePoints_pathGeometry = new() ;

      // if ( m_geometryCollection.Count != nPoints )
      // {
      //   m_geometryCollection.Clear() ;
      //   for ( int iLine = 0 ; iLine < nPoints ; iLine++ )
      //   {
      //     m_geometryCollection.Add(
      //       new LineGeometry()
      //     ) ;
      //   }
      // }
      // verticalLines_geometryGroup.Children = m_geometryCollection ;

      // LineGeometry[] lineGeometry = new LineGeometry[nPoints] ;
      // for ( int iLine = 0 ; iLine < nPoints ; iLine++ )
      // {
      //   lineGeometry[iLine] = new() ;
      // }

      // GeometryCollection geometryCollection = new() ;
      // LineGeometry[] lineGeometry = new LineGeometry[nPoints] ;
      // for ( int iLine = 0 ; iLine < nPoints ; iLine++ )
      // {
      //   lineGeometry[iLine] = new() ;
      // }

      ///////////////// verticalLines_geometryGroup.Children.Add(
      /////////////////   new LineGeometry() {
      /////////////////     StartPoint = bottomAnchorPoint,
      /////////////////     EndPoint   = bottomAnchorPoint.MovedBy(0,-lineLength)
      /////////////////   }
      ///////////////// ) ;

      List<Point> points = new(nPoints) ;
      Point GetPointAtFractionalPositionAlongLine(
        Point  startPoint,
        Point  endPoint,
        double frac01
      ) => new Point(
        startPoint.X + frac01 * ( endPoint.X - startPoint.X ),
        startPoint.Y + frac01 * ( endPoint.Y - startPoint.Y )
      ) ;
      // for ( int i = 0 ; i < intensityValues.Count ; i++ )
      // {
      //  var value = intensityValues[i] ;
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
          // Hmm, 'Children' is a GeometryCollection,
          // and there's no API that lets you Add multiple items,
          // we have to add them one at a time ...

          // TIMING TESTS ...
          // verticalLines_geometryGroup.Children.Add(new LineGeometry()) ;
          //geometryCollection.Add(lineGeometry[i]) ;

          // var line = (LineGeometry) m_geometryCollection[i] ;
          // line.StartPoint = bottomAnchorPoint ;
          // line.EndPoint   = bottomAnchorPoint.MovedBy(0,-lineLength) ;

          // This works, but it's VERY SLOW.
          // Just adding to a GeometryGroup is very slow.
          // Better to access the existing GeometryGroup
          // and change the lines' start and end points.
          verticalLines_geometryGroup.Children.Add(
            new LineGeometry() {
              StartPoint = bottomAnchorPoint,
              EndPoint   = bottomAnchorPoint.MovedBy(0,-lineLength)
            }
          ) ;

          ///
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

      bool drawOutline = true ;
      if ( drawOutline )
      {
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
          // https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.pathfigure?view=winrt-19041
          new PathFigure(){
            StartPoint = points[0],
            Segments   = segmentsCollection
          }
        ) ;
      }

      var result = new GeometryGroup() {
        Children = {
          verticalLines_geometryGroup,
          joinedOutlinePoints_pathGeometry
        },
        Transform = GraphPathTransform // Aha ! Better than applying transform to the Path element ...
      } ;

      System.TimeSpan timeAfterPathDataBuildCompleted = m_executionTimingStopwatch.Elapsed ;
      System.TimeSpan pathDataBuildTimeElapsed = timeAfterPathDataBuildCompleted - timeBeforePathDataBuildStarted ;
      Common.DebugHelpers.WriteDebugLines(
        $"Path data build time (mS) {pathDataBuildTimeElapsed.TotalMilliseconds:F3}"
      ) ;

      if ( m_savedResult is null )
      {
        m_savedResult = result ;
      }
      return (
        // result 
        m_savedResult 
      ) ;
    }

    GeometryGroup m_savedResult ;

  }

}
