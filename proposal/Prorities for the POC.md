## Priorities for the POC

The notes we've put together to describe the Viewer, really refer to a 'complete' version that we'd hope to build eventually, at CLF. For the POC, we'll be happy with a lot less than this.

<i>Features such as choosing from a set of alternative 'sources', changing the 'source' settings, changing the 'image presentation settings' (colour map etc), editing the user preferences, etc, are not required for the POC. We think we'll have no trouble adding these ourselves as they'll be pretty straightforward once we get the hang of things.</i>

Our priorities (negotiable!) would be 

1. Displaying one time-varying image, updating at a respectable rate eg 10-per-second. Working on Windows and Linux. Preferably using WinUI, but we're happy to work with UWP if that's a 'safer' option for the time being. The Viewer should be a distinct component packaged as a UserControl in a separate DLL, rather than being baked into the App project. 

1. Drawing a single time-varying 'profile' graph underneath the image, with the profile position indicated by an overlaid horizontal line. <br> <br> <i>The graph could be drawn either with 'graphic objects' on a Canvas (Line etc, with coordinates data-bound to the ViewModel) or with SkiaSharp if Matteo fancies using that instead. Might be nice to demonstrate it working both ways so we can compare and contrast ?</i>

1. Pan and zoom, with the Profile graph keeping in sync, and with the reference position for the Profile graphs changeable by clicking on the image or by dragging the overlaid line.

1. If time allows : changing the 'image presentation settings' (colour map etc), and a 'modal dialog' to edit the User Preferences.

We've already sketched out the Interface definitions (in C#) for the VM's, and Matteo should be able to get cracking on some XAML even before we've got the concrete classes ? That was the idea anyway. 

CLF will be entirely responsible for implementing the concrete ViewModel classes, and we'll kick this off once we've discussed the design with Matteo and taken his thoughts on board. We'll implement Tests for the ViewModel classes, so we'll be fairly sure they're working OK before we hand them over to Matteo. 

We'll plan to use 'Dependency Injection' to (A) let us easily switch between the initial 'dummy' ViewModels and real ones that will eventually acquire images from a camera ; and (B) to implement services such as Logging.
