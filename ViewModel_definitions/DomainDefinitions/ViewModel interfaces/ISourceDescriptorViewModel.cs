//
// ISourceDescriptorViewModel.cs
//

using System.Collections.Generic ;

namespace IntensityMapViewer
{

  //
  // Represents a 'source' of IntensityMap data.
  //

  public interface ISourceDescriptorViewModel : IViewModel
  {

    //
    // The 'SummaryTextLine' is used to populate the ListBox or whatever that selects the source.
    // Typically it would show the Name and the CommsStatus.
    //
    // Hmm, not necessary - leave it to the View to decide what to display ??
    //
    // The advantage of defining it as a property is that the ChangeNotification mechanism
    // can tell us to repaint this when any of the contributing properties changes,
    // eg the 'CommsStatus'. Would that be easy to implement if the summary text
    // was constructed via an x:Bind conversion function ?? 
    //
    // Yes, turns out that will work fine, the result of the binding function
    // will be re-evaluated whenever any one of the supplied arguments changes, cool.
    //

    // string SummaryTextLine { get ; } // => $"{Name} ({CommsStatus}" ;

    string Name { get ; }

    // Information published by the camera that's capturing the image,
    // eg description, model number, serial number (all fixed),
    // and status info (might change occasionally)

    IEnumerable<string> CameraMetadataTextLines { get ; }

    // Is the connection alive ?

    CommsStatus CommsStatus { get ; }

    // Are we currently receiving a stream of images ?

    bool IsReceivingImageUpdates { get ; set ; }

    // Images from a particular Source always have this size.
    // Note that this will not necessarily be the case in a 'real' system,
    // as the image dimensions would be configurable by various means
    // and could potentially change from one image to the next.
    // However for the POC we can ignore this possibility.

    System.Drawing.Size SourceImageDimensions { get ; } 

    ISourceSettingsViewModel SourceSettings { get ; }

    IIntensityMap? MostRecentlyAcquiredIntensityMap { get ; }

    event System.Action? NewIntensityMapAcquired ;

    event System.Action? ProfileGraphsReferencePositionChanged ;

    System.Drawing.Point ProfileGraphsReferencePosition { get ; set ; } 

    // We can zoom in on a region, and this will be persisted.
    // Hmm, might be better to represent this as a centre-point and
    // and a zoom factor ?

    System.Drawing.Rectangle DisplayedRegion { get ; set ; }

  }

}
