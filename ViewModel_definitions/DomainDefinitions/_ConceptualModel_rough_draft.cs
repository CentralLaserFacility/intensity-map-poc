//
// _ConceptualModel_rough_draft.cs
//

using System.Collections.Generic ;

namespace IntensityMapViewer.ConceptualModel
{

  //
  // This was our initial working draft for a conceptual model of the UI,
  // expressed as interface definitions. Now somewhat out of date.
  //

  public interface IViewModelDefinition : System.ComponentModel.INotifyPropertyChanged
  { }

  public interface IIntensityMapDisplayPanel : IViewModelDefinition
  {

    IIntensityMap? IntensityMapToDisplay { get ; }

    IIntensityMapSourceSelector IntensityMapSourceSelector { get ; }

    IIntensityMapImagePresentationSettings PresentationSettings { get ; }

    IIntensityMapProfileDisplaySettings ProfileSettings { get ; }

    //
    // When this is 'true' the image panel updates itself to keep in sync with
    // the most recent image that has been sent to us by the camera. When 'false',
    // the image panel freezes. While it's frozen, the 'Save' button is enabled.
    //

    bool EnableRealTimeUpdates { get ; set ; }

    // Commands

    System.Windows.Input.ICommand ShowIntensityMapSourceSettingsPanel { get ; }

    System.Windows.Input.ICommand SaveCurrentImage { get ; }

    // Miscellaneous 'simple' properties

    double AverageUpdateRate_FramesPerSecond { get ; }

    byte MinimumIntensityValueInCurrentImage { get ; } // ???
    byte MaximumIntensityValueInCurrentImage { get ; }

    //
    // If the comms channel to the current Source is disconnected, 
    // we indicate that on the UI eg by displaying a coloured border.
    //

    CommsStatus CommsStatus { get ; }

  }

  public interface IIntensityMapSourceSelector : IViewModelDefinition
  {

    IEnumerable<IIntensityMapSourceDescriptor> AllAvailableIntensityMapSourceDescriptors { get ; }

    IIntensityMapSourceDescriptor? CurrentlySelectedIntensityMapSource { get ; }

  }

  public interface IIntensityMapImagePresentationSettings : IViewModelDefinition
  {

    NormalisationMode NormalisationMode { get ; set ; }

    //
    // The 'maximum expected intensity value' is settable via a slider if we're in 'Manual' mode.
    //
    // In 'Automatic' mode the slider position indicates the value, but the value can't be adjusted.
    //

    byte MaximumExpectedIntensityValue { get ; set ; }

    // bool MaximumExpectedIntensityValueCanBeManuallySet => NormalisationMode is NormalisationMode.Manual ;

  }

  public interface IIntensityMapProfileDisplaySettings : IViewModelDefinition
  {

    // IIntensityMapSourceDescriptor SourceDescriptor { get ; }

    // IIntensityMap IntensityMap { get ; }

    // int VerticalProfilePositionX { get ; set ; }
    // 
    // int HorizontalProfilePositionY { get ; set ; }

    System.Drawing.Point ProfilePosition { get ; set ; }

    System.Collections.Generic.IReadOnlyList<byte> VerticalProfileIntensityValues   { get ; }
    System.Collections.Generic.IReadOnlyList<byte> HorizontalProfileIntensityValues { get ; }

    bool ShouldShowProfileGraphs { get ; set ; }

    // Nice, but not required
    // bool ShouldShowProfileGraphTickMarksAndCoordinates { get ; set ; } 

  }

  public enum CommsStatus {
    Connected,
    Disconnected
  }

  //
  // Hmm, perhaps separate out the 'potentially available' source properties
  // from the properties of an 'active' Source that we're actually connected to ???
  //

  public interface IIntensityMapSourceDescriptor : IViewModelDefinition
  {
    //
    // Represents a source of IntensityMap data.
    //

    // The 'SummaryTextLine' is used to populate the ListBox or whatever that selects the source.

    string SummaryTextLine { get ; }

    string Name { get ; }

    // Information published by the camera that's capturing the image,
    // eg description, model number, serial number (all fixed),
    // and status info (might change occasionally)

    IEnumerable<string> CameraMetadataTextLines { get ; }

    // Is the connection alive ?

    CommsStatus CommsStatus { get ; }

    // Are we currently receiving a stream of images ?

    bool IsActive { get ; set ; }

    // Images from this Source always have this size

    System.Drawing.Size SourceImageDimensions { get ; } // Ask Chris - could this change ???

    IIntensityMapSourceSettings SourceSettings { get ; }

    IIntensityMap? MostRecentlyAcquiredImage { get ; }

    event System.Action NewImageAcquired ;

    // Hmm, we never actually 'request' a fresh image,
    // the camera gives us one when it feels like it
    // according to the Trigger Mode we've set
    // void RequestNewImageAcquisition ( ) ;

    // int VerticalProfilePositionX   { get ; set ; }
    // int HorizontalProfilePositionY { get ; set ; }

    // Common.ImagePointXY ProfilePosition { get ; set ; } // Derived ??

    // We can zoom in on a region, and this will be persisted.

    System.Drawing.Rectangle DisplayedRegion { get ; set ; }
  }


  public interface IIntensityMap : IViewModelDefinition
  {
    //
    // Provides the raw data for the image display panel
    //
    //
    // This provides an abstraction of the image data we'll acquire from a camera.
    // It defines the image pixels in a low level form that is platform-independent.
    //
    // A helper function will be used to map the image pixels into a WinUI 'BitmapSource'.
    //
    // In the demo, the concrete class will load an image from a '.pgm' file or similar.
    // In the 'real' app, images will be acquired from an Epics PV.
    //
    System.DateTime TimeStamp { get ; }
    System.Drawing.Size Dimensions { get ; }
    //int WidthX  { get /* => SourceDescriptor.ImageWidthX  */ ; }
    //int HeightY { get /* => SourceDescriptor.ImageHeightY */ ; }
    // The first byte represents the pixel at the top left, and subsequent bytes
    // scan along the top row and then step down, as if we're reading words on a page.
    // Each byte represents an unsigned greyscale value 0-255. 
    // System.Span<byte> PixelIntensities { get ; }
    // We can also access the intensity at a specified position.
    // byte this [ int x, int y ] { get ; }
    // Or we could be using a ReadOnlySpan<> as a safe way of accessing the data at a low level ??
    System.Collections.Generic.IReadOnlyList<byte> IntensityBytes { get ; }
    // Hmm, we could return Span<> instances representing horizontal and vertical slices ???
    // We can drill down into a 'descriptor' that provides
    // detailed information about the image being displayed.
    // IIntensityMapSourceDescriptor SourceDescriptor {  get ; }
  }

  public enum ImageAcquisitionTriggeringMode {
    Internal,
    External
  }

  //
  // This provides the ViewModel behind the 'Settings' panel
  // that configures the settings for a particular source ie a camera.
  //
  // Changes made to these settings take effect immediately.
  // The UI doesn't have a 'Submit' button or an 'Undo'.
  //

  public interface IIntensityMapSourceSettings : IViewModelDefinition 
  {
    // IIntensityMapSourceDescriptor SourceDescriptor {  get ; }
    (double Min, double Max) ValidGainValuesRange { get ; }
    double CurrentGainValue { get ; set ; }
    (double Min, double Max) ValidExposureTimesRange { get ; }
    double CurrenExposureTimeInMicroseconds { get ; set ; }
    ImageAcquisitionTriggeringMode TriggeringMode { get ; set ; }
    // (int,int,int,int) RegionOfInterest { get ; set ; }
  }

  public interface IUserPreferences : IViewModelDefinition
  {
    System.Drawing.Color CrossSectionLineColour { get ; set ; }
    float CrossSectionLineThickness { get ; set ; }
    PanAndZoomMode PanAndZoomMode { get ; set ; }
    PanAndZoomBehaviourOnSourceChange PanAndZoomBehaviourOnSourceChange { get ; set ; } 
    HowToDrawProfileGraph HowToDrawProfileGraph { get ; set ; } 
    HowToShowSourceSettingsPanel HowToShowSourceSettingsPanel { get ; set ; } 
    ThemeChoice ThemeChoice { get ; set ; }
    float FontSizeMultiplier { get ; set ; } 
    bool ShowHorizontalAndVerticalScrollBarsOnImagePanel { get ; set ; }
  }

  public enum ThemeChoice {
    NormalTheme_ForControlRoomOrOfficeEnvironment,
    SpecialTheme_ForLabEnvironment
  }

  public interface ISystemConstants
  {
    float MaxZoomInFactor ( PanAndZoomMode zoomMode ) ;
    float MaxZoomOutFactor ( PanAndZoomMode zoomMode ) ;
    // Hmm, these would best be defined in the Theme ??
    // But maybe the Skia rendering won't have easy access to the Theme values.
    float ProfileGraphLineThickness { get ; set ; }
    System.Drawing.Color ProfileGraphLineColour { get ; set ; }
  }

  public enum HowToShowSourceSettingsPanel {
    ShowAsModalDialog,
    ShowAsModelessDialog,
    ShowAsFlyout
  }

  public enum PanAndZoomBehaviourOnSourceChange {
    ResetToNominal,
    RestoreSavedSettings
  }

  public enum PanAndZoomMode {
    Constrained,
    Unconstrained
  }

  public enum HowToDrawProfileGraph {
    DrawSolidBarsCentredOnEachPixel,
    DrawLinesBetweenAdjacentPixels
  }

  //
  // The image can be displayed either as greyscale, or with the pixel intensities
  // mapped to a Colour. A typical option is the 'JET' scheme ...
  //
  //   https://stackoverflow.com/questions/7706339/grayscale-to-red-green-blue-matlab-jet-color-scale
  // 
  // We may want to offer a wider set of colour mapping options,
  // so the View should provide a ComboBox to define the selected option.
  //

  public enum ColourMapOption {
    GreyScale,
    JetColours,
    Binary // As in LabView ; hmm, more like 'cyclic' ??
  }

  public enum NormalisationMode {
    Automatic_FromBrightestIntensityInImageMap, // Detects highest intensity value
    Manual_FromUserDefinedValue                 // Highest intensity is specified via a slider
  }

  public static class Helpers
  {
    // This helper will be specified in the XAML with x:Bind, to convert from the
    // raw image data to a BitmapSource that will serve as the data source
    // for an Image control. Internally it will use a WriteableBitmap
    // whose PixelBuffer will be populated with the image data.
    // Each byte of the image will be mapped to an RGB Colour.
    public static 
    object /*Windows.UI.Xaml.Media.Imaging.BitmapSource*/
    BuildBitmapSource (
      IIntensityMap    rawImage,
      ColourMapOption colourMapOption       = ColourMapOption.JetColours,
      byte            highestIntensityValue = (byte) 255
    ) { 
      throw new System.NotImplementedException() ;
    }
  }

}
