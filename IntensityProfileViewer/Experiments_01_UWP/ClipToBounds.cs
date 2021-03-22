using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Experiments_01_UWP
{
    
  //
  // This 'attached property' is meant to set the Clip of a Canvas
  // so that nothing gets painted outside of its nominal bounds.
  //
  // https://www.domysee.com/blogposts/canvas-rendering-out-of-bounds
  //
  // Hmm, doesn't seem to work ...
  //

  public class ClipToBoundsHelper
  {

    public static bool GetClipToBounds ( DependencyObject obj )
    {
      return (bool) obj.GetValue(ClipToBoundsProperty) ;
    }

    public static void SetClipToBounds ( DependencyObject obj, bool value )
    {
      obj.SetValue(ClipToBoundsProperty,value) ;
    }

    public static readonly DependencyProperty ClipToBoundsProperty 
    = DependencyProperty.RegisterAttached(
      "ClipToBounds", 
      typeof(bool), 
      typeof(ClipToBoundsHelper), 
      new PropertyMetadata(false,ClipToBoundsChanged)
    ) ;

    private static void ClipToBoundsChanged ( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
      // Use 'FrameworkElement' here because it is the highest abstraction
      // that contains a safe 'size'. UIElement does not contain 'size' data.
      if ( d is FrameworkElement element )
      {
        element.Loaded      += (s,evt) => ClipElement(element) ;
        element.SizeChanged += (s,evt) => ClipElement(element) ;
      }
    }

    private static void ClipElement ( FrameworkElement element )
    {
      if ( GetClipToBounds(element) )
      {
        var clip = new RectangleGeometry() { 
          Rect = new Rect(
            0, 
            0, 
            element.ActualWidth, 
            element.ActualHeight
          ) 
        } ;
        element.Clip = clip ;
      }
      else
      {
        element.Clip = null ; // Added STEVET
      }
    }

  }

}
