# Priorities for the POC

The proof-of-concept project demonstrates the bare bones of an interactive 'Viewer' component for Intensity Map images.

Elsewhere we've described the features of a 'full' Viewer that we plan to develop in a follow-on project. The POC will implement just the subset outlined here, which nevertheless will prove the viability of the technology for CLF's applications.

The features to be tackled for the POC are prioritised as follows :

1. Displaying one time-varying image, updating at a respectable rate eg 10-per-second. <br> <br> Running on various platforms :

   - Windows desktop (UWP)
   - Linux desktop (Ubuntu 20)
   - Android tablet
   - Web page

2. Drawing a single time-varying 'profile' graph underneath the image, with the profile position indicated by an overlaid horizontal line.

   *The graph could be drawn either with 'graphic objects' on a Canvas (Line etc, with coordinates data-bound to the ViewModel) or with SkiaSharp if Matteo fancies using that instead. Might be nice to demonstrate it working both ways so we can compare and contrast ?.*

   *We might consider using a 3rd party Graph control eg from Syncfusion, but part of our motivation here is for CLF to learn how to implement a simple graphical component 'from scratch' so that we'll be well placed to develop the customised displays that the users of our science-oriented applications require.* 

3. Pan and zoom, with the Profile graph keeping in sync, and with the reference position for the Profile graphs changeable by clicking on the image or by dragging the overlaid line.

4. If time allows (priorities tbd)

   - changing the 'image presentation settings' (colour map etc) ?
   - providing an indication of 'comms failure' (simulated) ?
   - providing a 'modal dialog' to edit the User Preferences ?
   - implementing an alternative 'theme' with colours appropriate for users who are wearing protective goggles ?

Features that are definitely *not* required for the POC :

- Choosing from a set of alternative 'sources' ; there will be one source, providing 'simulated' image data (eventually the Viewer will connect to live sources of real-time data).

- Changing the 'source' settings

- Editing the user preferences

We think we'll have no trouble adding these ourselves as they'll be pretty straightforward once we get the hang of things.


### Viewer Component and App

The project will develop a reusable component, the 'IntensityMapViewer'. The Viewer component will be packaged as an Assembly (ie a DLL) that is referenced by a 'demo' app.

The appearance of the Intensity Map Viewer is shown below in the Wireframes section. 

The app window has two sections : the Viewer itself, and a Navigation control that selects the source of the data that's being displayed by the Viewer.


## Wireframes

This is a screen shot of our existing display :

![](images/existing-software-snapshot.png)

We don't wish to replicate this entirely. The following wireframes indicate the aspects we're most interested in for the POC.

Note that for the POC, the panels for selecting the camera and configuring the camera settings are not actually required. Also it would be adequate to have just the Horizontal graph underneath the image display. It would be nice though if the POC could show the visual elements for these, even if they are non-functional.

#### Standard view and camera settings panel
![Standard view](https://share.balsamiq.com/c/xn4A533GMav6RNe7CDFEja.png)

#### View with no profile graphs
![No profile view](https://share.balsamiq.com/c/pdh5f64LMTKwGK2HKLmRkt.png)

#### View when communication with the source is lost
![Loss of comms](https://share.balsamiq.com/c/Wy8aNRwXEXAyiAg8jrgSE.png)

## Domain Model

- Laser beam: a narrow beam of light that is manipulated by reflection off of mirrors. To change the direction of light so that it goes to the desired location, we adjust the angle of the mirror it reflect off. This process is known as laser alignment. 

- Sources: the laser beam is infra-red, and thus not visible by the naked eye (even if it was, it would be dangerous to look at it as it's too intense). To know where the laser beam is, we place a camera or other detector in the beam. We use the term source as a generalisation of camera, as for other applications of the Intensity Map viewer the source may be a different piece of technology that generates data of the same format. For the purposes of this POC source and camera are interchangeable and equivalent.

- Intensity Map: a 2D array of either 8- or 16-bit values that encode the intensity of the light at a given point on the Camera detection area. 

- Image: A representation of the intensity map that can be rendered by the user interface. It combines the intensity values with a colour palette.

- Profile graphs ('cross sections'): 1D cross-sections through the intensity data that show the intensity value at each pixel.

- Laser Scientist: the person responsible for ensuring the laser beam is correctly setup, and who uses the intensity map on a camera to do this.


The intensity data is always in 'grey-scale' format ; typically 320x240 or 640x480 pixels with 8 bit pixels that represent an intensity (ie brightness') from 0 (black) to 255 (bright white). Typically, the pixels in a captured dataset don't cover the entire range of values - in some cases we'll see values between zero and about 20, which makes for a very dim image if these are shown as greyscale pixels on the display. The visual representation can be improved by adjusting the camera settings (gain and/or exposure time) but as a convenience we provide a facility for stretching the data in the Viewer itself, by a process we call 'Normalisation'.

When an image is displayed on screen, a mapping formula is applied which typically (A) normalises the pixel values to a specified maximum value, and (B) maps the normalised values into false colours via a palette. Option (B) is all we require for the POC.

## Use Cases

Briefly, what does a Laser Scientist want to do with the Viewer?

### Check the laser alignment

- Change to the appropriate source (ie a particular camera).
- Adjust the settings (Gain and/or Exposure Time) for that source until the maximum intensity value in the captured image is suitable, say between 100 and 200 as a rough guide.
- Check that the current beam position is in the correct location. Just check the pixel position of the beam in this case. Adjust the mirrors as necessary.
- Switch to another source and do the same.

### Check the laser beam spatial profile 

- First, to look at overall picture, select the source you want.
- Set the intensity scaling to auto.
- Reposition the profile location so that the cross-sections are taken through the laser spot.
- Look at the cross-section graphs to check they look good. In the real world, would adjust the laser setup until the graphs are ok.
- Then look at low level signals 
- Change from automatic to manual gain, and increase the gain so that the low-level noise in the image is visible. The higher intensity parts of the image may saturate, but that is acceptable here.
- Inspect the (rescaled) cross-section plots and the image, to check that not too much signal is in the area outside of the main laser spot.

### Save an image of the laser spot

- Set the gain/display values to show the laser beam nicely (max intensity say 100 - 200 as above).
- Apply the colour palette of choice so the image is coloured as desired.
- Export the current image (with false colours applied) to a standard file format.

### Check the laser spot for abnormalities

- Select desired source.
- Adjust the gain and maximum intensity values to get a clear image.
- Pan/zoom in to the region around the laser spot in the image.
- Check the image and cross-sections for smoothness. Are there any sudden variations in intensity ? We don't want images and profiles to be too 'spiky'.

## Details of the required functionality

This section describes how the user should interact with the UI. 

- Interacting with the intensity map
- Choosing how the image is displayed 
- Enabling/disabling the cross-section graphs 
- Choosing the X/Y coordinates for the cross-section graphs 
- Pan and zoom on the image
- Simulating errors such as loss-of-comms 
- Resizing the window 
- Closing and re-starting
- Selecting a different Theme

Here's a sketch to identify the regions we'll be talking about, with '???' indicating panels that are 'optional' ie that will be implemented if there's time ...

    Intensity-Map-Panel
    +---------------------------------------------------------------------+
    | +-----------------------------------------+ +---------------------+ |
    | |                                         | | Image               | |    
    | |  Intensity-Map-Display                  | | Presentation        | |
    | |                                         | | Settings            | |
    | |  ### +-------------------------------+  | | [ ??? ]             | |
    | |  ### |        |                      |  | +---------------------+ |
    | |  ### |        |                      |  | +---------------------+ |
    | |  ### | - - - -+- - - - - - - - - - - |  | | Profile             | |
    | |  ### |        |                      |  | | Graph               | |
    | |  ### |        |                      |  | | Settings            | |
    | |  ### |        |                      |  | | [ ??? ]             | |
    | |  ### |        |                      |  | +---------------------+ |
    | |  ### +-------------------------------+  |                         |
    | |                                         |                         |
    | |      #################################  |                         |
    | |      #################################  |                         |
    | |                                         |                         |
    | +-----------------------------------------+                         |
    | +-----------------------------------------------------------------+ |
    | | Other Miscellaneous UI elements here ?                          | |
    | +-----------------------------------------------------------------+ |
    +---------------------------------------------------------------------+

The Intensity-Map-Panel is a UserControl that is composed of several other UserControls :
- An Intensity-Map-Display. This presents an image in either grey scale or false colours, and also (if enabled) graphs of the horizontal and vertical intensity profiles.
- An 'Image-Presentation-Settings' panel to configure how the underlying 8-bit pixel data is presented as an RGB image.
- A 'Profile-Graph-Settings' panel that configures the graph displays below and to the left of the image.
- A button to bring up a panel that configures the settings of the current Source, including the camera's gain and exposure time.
- Miscellaneous elements tbd, such as a check box to simulate loss-of-comms.

Generally speaking, **when you hover over something a useful tooltip pops up.**
- When you hover over the image, the tooltip shows the X-Y position (in terms of Source pixel indeces) and the un-normalised intensity value.
- When you hover over a button, the tooltip provides a brief explanation of what will happen if you click.
- When you hover over a UI element that is disabled (greyed-out) the tooltip explains why that feature is not available. For example if the Source Settings button is greyed out because that Panel is already active, the tooltip will mention that.

Keyboard shortcuts will be provided for common actions, tbd. For example, zoom-out-completely ; play/pause.

We might also make use of the mouse wheel for making certain adjustments, possibly in conjunction with the Shift and Ctrl keys. Details tbd, but the scroll wheel will be very handy for zooming in and out of the image, and also might provide a very convenient way to tweak the 'max intensity value' for image normalisation that we'll describe later.

### The 'intensity map' display

This is the most interesting and important element in the UI. It shows a greyscale or false-colour image that represents the 2D map of the pixel value as acquired by the selected Source. 

Alongside the image are graphs that show cross-sections through the dataset at particular X and Y coordinates, which we refer to as the 'cross-section coordinates'. 
- The left hand graph shows the intensity values along a vertical slice passing through the specified point - that is, values which all have the same X coordinate. 
- The graph at the bottom shows values along a horizontal slice - that is, values which all have the same Y coordinate.

The X-Y coordinates of the horizontal and vertical cross-sections are indicated by coloured lines that are overlaid onto the image, as indicated in the ASCII-art above. The colour and thickness of the 'cross-section lines' can be configured in the Preferences panel.


The displayed coordinates and values always pertain to the underlying intensity map, not to the coordinates in the displayed image or the RGB values.

### Choosing how the intensity map data is presented 

<b>[ Nice-to-have for the POC. ]</b>

This panel lets you adjust the Normalisation and select the Colour Map. These parameters determine the way that the intensity data (0-255) is translated into image pixels (RGB).

Normalisation can be selected as either Manual or Automatic. 

- In 'manual' mode you explicitly specify a 'maximum intensity value', say 100, and the pixel values in the intensity map will all be scaled such that those with a nominal value of 100 or above will get mapped to the highest intensity value, equivalent to 255, when the data is converted to visual pixels. 
- In 'auto' mode, the 'maximum intensity value' is determined by scanning the intensity map and finding the pixel with the highest value.

The maximum intensity value can be set via a sider or via text entry. In 'auto' mode, the slider and the text box are read-only, and they indicate the maximum value that has been found. When you transition from 'auto' to 'manual' mode, the value stays the same until you change it.

As an optional extra, it might be useful to be able to also adjust the 'manual' value using the mouse's scroll wheel. Perhaps while the mouse is hovering over the image with 'shift' held down, or while the mouse is hovering over the 'Presentation' panel.

Probably a replication of how default windows controls work is a good way to start, where when the slider control is selected, the scrolling of mouse wheel change the slider's value.

The 'colour map' chooser is a combo box that offers a choice of three options :
- Greyscale
- The 'JET' colour scheme
- Binary (as implemented by National Instruments: https://zone.ni.com/reference/en-XX/help/370281AG-01/nivisionconcepts/palettes/)

Binary mode is a nice-to-have, not essential. It would be implemented entirely in the ViewModel layer.


### Panning and zooming

This is a fairly high priority for the POC, because implementing it is not necessarily straightforward :)

You can pan and zoom the image, to zero in on interesting features. Panning and zooming will be controlled by mouse gestures or touch gestures, and possibly via keyboard shortcuts (tbd). In large part, we would like to replicate standard (Windows Photos app) behaviour.

With a mouse :
- Panning is achieved by holding the mouse left button down over the image, and dragging. When you drag, the image follows the mouse position.
- Zooming is achieved by rolling the mouse wheel while the pointer is over the image. A forward roll 'zooms in', a backward roll 'zooms out', keeping the same piece of the image 'under' the mouse pointer at all times.
- You can pan and zoom at the same time.

An optional improvement would be to select a rectangular 'rubber-band' region via click-and-drag, with that becoming the zoomed-in area when you terminate the gesture.

On a touch screen, panning and zooming are achieved with 'pinch' gestures.

An alternative will be needed for mice that don't have scroll wheels, eg keyboard shortcuts and right-click menu options. Another possibility would be to make a double-click 'zoom-in' by 2x, and a shift-double-click 'zoom-out', or buttons for zoom in/out/reset.

Constraints on pan and zoom are as for Windows Photos:
- Maximum zoom is constrained to some amount (e.g. 10x)
- User can only zoom out to the point that the full image fits into available space
- User can pan only as far as the edge of the image aligns with the edge of the viewable area (so only pan e.g. left until the left edge of the image comes into view)
- As far as possible, the zoom in/out takes place around the pixel under the mouse pointer when the scroll wheel is used.

**As you pan and zoom, the lines representing the 'cross-section coordinates' move along with the underlying image, and the Profile graphs update to keep in sync with the region that's being displayed.**

It is essential that the proper aspect ratio is preserved in the image display that is generated from the dataset. So depending on how the user has panned and zoomed, the area assigned for the display might need to show 'blank' areas that lie outside the bounds of the IntensityMap. 

When you're zoomed in to a subsection of the image, the cross-section graphs should only show data for the region that is visible.


Typical scenarios are illustrated below.

 Scenario 1

Only showing the 'valid' portion of horizontal graph, may create confusion for the user, one suggestion is that we can always keep our horizontal and vertical graphs of same size as the image area for 1-1 mapping and where there is a blank area it will just show no values in the profile graphs. So there is no confusion of having image within an image.

        +---------------------------------+
    ##  |::::::::|:::::::::::::::::::     |     The image area is wider than is necessary
    ##  |::::::::|:::::::::::::::::::     |     to display the intensity map, so to preserve
    ##  |::::::::|:::::::::::::::::::     |     the aspect ratio, a blank area is shown
    ##  |::::::::|:::::::::::::::::::     |     at the right.
    ##  |--------|-------------------     |
    ##  |::::::::|:::::::::::::::::::     |     The graph at the left occupies the full
    ##  |::::::::|:::::::::::::::::::     |     available height, whereas the graph below
    ##  |::::::::|:::::::::::::::::::     |     only occupies the relevant portion of the width, 
    ##  |::::::::|:::::::::::::::::::     |     directly underneath the 'valid' part of the image.
        +---------------------------------+
         ############################
         ############################


Scenario 2

        +---------------------------------+
    ##  |::::::::|::::::::::::::::::::::::|     We've zoomed in a long way, so we're showing 
    ##  |::::::::|::::::::::::::::::::::::|     just a small part of the dataset - but it occupies
    ##  |::::::::|::::::::::::::::::::::::|     the entire available space.
    ##  |::::::::|::::::::::::::::::::::::|      
    ##  |--------|------------------------|
    ##  |::::::::|::::::::::::::::::::::::|     Both graphs occupy the entire available space,
    ##  |::::::::|::::::::::::::::::::::::|     but they only encompass the points that are
    ##  |::::::::|::::::::::::::::::::::::|     being displayed in the image.
    ##  |::::::::|::::::::::::::::::::::::|     
        +---------------------------------+
         #################################
         #################################


Scenario 3

        +---------------------------------+
    ##  |:::::::::::::::::::::::::::::::::|     We're zoomed in a long way, and we've panned 
    ##  |:::::::::::::::::::::::::::::::::|     right so that the 'vertical' cross-section coordinate
    ##  |:::::::::::::::::::::::::::::::::|     is outside the visible area.
    ##  |:::::::::::::::::::::::::::::::::|      
    ##  |---------------------------------|
    ##  |:::::::::::::::::::::::::::::::::|     We're still showing the left hand graph, however it shows no data
    ##  |:::::::::::::::::::::::::::::::::|     
    ##  |:::::::::::::::::::::::::::::::::|     
    ##  |:::::::::::::::::::::::::::::::::|     
        +---------------------------------+
         #################################
         #################################

### Enabling/disabling the cross-section graphs

If a user disables the graph display, the Image expands to fill the available space - preserving the aspect ratio so that square pixels stay square, and a circular beam profile stays circular.

### Choosing X/Y coordinates on the cross-section graphs

The graphs indicate the intensity along a particular slice of the intensity map, as determined by the X-Y position.

You can adjust the cross-section coordinates in a number of ways : 
- via the controls in the 'Profile-Settings' panel
- left mouse click/finger tap on the image, which sets the cross-section coordinates to that point.
- by dragging the vertical line sideways, or by dragging the horizontal line up or down, or by dragging the point where the two lines intersect.

The cross section coordinates are constrained to lie at valid pixel indeces, eg 0-639 for the X coordinate with a 640-width image.

It would be nice to show tick marks and numeric labels on the y-axis, but this is not essential if the information can be obtained via tooltips.

The graphs update when the image changes, when the cross-section coordinates change, or when the user pans/zooms.


### Simulating errors such as loss-of-comms

Maybe have a check box on the status bar ?

This lets us demonstrate how loss-of-comms is indicated in the UI, eg by changing the background colour.


### Resizing the window 

The image display changes size to make best use of the available space.


### Selecting a different theme

<b>[ Not required for the POC, but nice if we can demonstrate this. ]</b>

An optional extra would be to provide different colour themes settable by preferences.

The Viewer will be used in offices, and also in laboratories where safety considerations require that users wear special goggles to protect their eyes from the laser. The optimum colour for things such as warning and error indicators (eg 'loss of comms') is different in those two environments. For example in a control room 'red' is a good choice, but in the lab 'magenta' is strongly preferred as it shows up more distinctly through goggles.

In addition to the colour, the user could wish to set other visual attributes such as font size. This would be done via Preferences rather than via a Theme, although the 'Lab' theme might configure a larger font size as the default.

### Preference settings

<b>[ These will be hard coded : displaying and editing them is not required for the POC. ]</b>

Settings relating to the Viewer :

- Line thickness and colour of the X-Y profile position indicator.


Settings relating to the app generally :

- Theme, selectable as ControlRoomTheme or LabTheme.
- Font Size scaling. As the Viewer will be used with screens of various sizes and with the user at varying distances (particularly in the lab), it will be useful to be able to rescale the font sizes over a range of say 0.5x to 2x the nominal size, via a slider. The space taken up by the various panels will adapt appropriately.

Changes get applied instantly in the UI, there's no 'submit' or 'undo'.

Nice if the current settings are persisted (as a JSON file), and restored when you close and restart the Viewer.

----

## Technical notes

Ideas for the internals and the implementation, for discussion.

### WinUI-3 framework instead of UWP ?

If time allows, and if the WinUI-3 preview looks to be stable enough, we might consider migrating to WinUI as part of the POC ; however the primary target here is UWP, which is well proven and stable.

Future development at CLF will almost certainly use WinUI once it's generally available.

### Operations applied to the IntensityMap acquired from the Source

In the POC, a 'typical' intensity map will be loaded from a file, with values in the range 0-255. Random noise will be added so as to make successive datasets slightly different and give the illusion that we're actually capturing live images.

For the avoidance of doubt, here's a picture of the pipeline of operations that are applied in the POC to the Intensity Maps that we load from a file that represents a 'Source'. 

All the relevant maths happens either in the ViewModels or in the helper function we'll provide for mapping the 8-bit values to RGB.

The names in the boxes below are the ones used in the ViewModel definitions.

    +-------------------+
    | Intensity map     |  8 bit pixel values 0-255
    | dataset on disk   |  Typically 640x480
    +-------------------+
         |
         | Add some random noise
         |
    +------------------+
    |   Perturbed      |  With noise added, successive datasets are 
    |   IntensityMap   |  slightly different (as if from a real camera)
    +------------------+
         |
         | Scale by some function of     
         | Gain and Exposure  
         |
    +------------------+
    |   Acquired       |  Simulating what we'd have acquired from a camera
    |   IntensityMap   |  8 bit pixel values 0-255
    |                  |                   
    |                ------------> This is the data that we use to drive 
    |                  |           the Graph displays, and the Intensity 
    +------------------+           values that we show on the Image tooltip
         |
         | Normalise to  
         | max intensity  
         |
    +------------------+
    |   Normalised     |  8 bit pixel values 0-255
    |   IntensityMap   |
    +------------------+
         |
         | Apply the ColourMap palette
         | 8 bit data -> RGB pixels
         | and transform to UI coordinate space
         |
    +---------------------+
    | Falsely coloured    | 32 bit pixel values (ARGB).
    | IntensityMapImage   | Dimensions defined by the UI.
    +---------------------+

The final output from this pipeline is what gets displayed as an RGB image in the Viewer, within the available area that the user might have adjusted by resizing the window.

The pixel dimensions of the displayed image will almost certainly not match the pixel dimensions of the dataset, so some kind of resampling will need to be performed. Best if we do this via the view API's rather than in our code, as that will be more efficient. Ideally the stretching will be performed by duplicating pixel values rather than a technique such as bilinear interpolation, as this will make 'dead' pixels more clearly obvious.

The logic to present the appropriate segments of the dataset to the View layer, according to the requested Pan and Zoom parameters, will be implemented in the ViewModel layer and the associated helper functions. This will allow us to keep the View itself as straightforward and 'dumb' as possible ; the View it will just see an Image that needs to be painted in the assigned area.

### View Models

The UI for the Viewer will be backed by ViewModel classes coded in C#, for which CLF will provide an initial implementation and associated Tests. We'll use the MVVM library from the Community Toolkit.

Where we're simulating 'live' data such as streams of camera images, an internal timer will trigger the ViewModels to cycle through a baked-in sequence of snapshots loaded from resource files, at a rate of up to 30fps.

Rather than have the entire UI defined in a single 'flat' ViewModel, we'll structure things so that there are distinct ViewModel classes for distinct aspects of the overall Viewer control. This will make it easier for us to develop variants of the Viewer in future, for example one that displays several Intensity Map images side by side in a grid.

Provisionally, we'll define ViewModels for
- Viewer (top level VM is composed of other VM's plus some simple flags and Commands)
- ImageDisplay, with optional 'cross sectional profile' graphs and Pan/Zoom functionality
- PresentationSettings (false-colour selection, enabling profile graphs etc)
- Preferences

Probably each of these VM's will be associated with a UserControl that provides the visuals. The 'main' UserControl for the Viewer will contain instances of the other UserControls, mirroring the composition structure of the ViewModel classes.

*Just ideas, we'll take advice on this !!*

### Exception handling

Nothing should cause a crash, any uncaught exceptions should be caught at the top level and logged somewhere useful.

### Dependency injection

We'll use DI to instantiate concrete implementations that are accessed via Interfaces. Also the 'Mediator' pattern to facilitate interactions between ViewModels.

### Cross section graphs

We would prefer that the graphs for the POC are implemented using custom code in SkiaSharp, as this will give us an excuse to see how to do graphics in Skia. Also our particular requirements don't align particularly well with the features in standard Graph packages from 3rd parties.

The graphs will probably be created using the Skia pixel-oriented API's to write onto bitmaps, as part of the process that builds the coloured 'image' bitmap from the active area of the dataset.

https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/basics/pixels

The graphs could either (A) be created as separate Images, which would reside in a distinct area of the View, or (B) the two graphs and the intensity-map itself could be drawn onto a single bitmap painted into a single area. Need to explore the tradeoffs. 


---
## Possible extensions

### A second application:  Test Harness for the Viewer and other UI controls

    +-----------------------------------------------+---+---+
    | Test Harness App                              | - | x |
    +--------------+--------------------------------+---+---+
    | Menu bar ?                                            |
    +--------------+----------------------------------------+
    |              |                                        |
    |  Navigation  |                                        |
    |  panel       |  Main panel                            |
    |              |                                        |
    |  Selects     |  Shows one of various 'content panels' |
    |  content     |  as selected by the Navigation panel   |
    |  of          |                                        |
    |  main        |  1. Viewer plus a Source selector      |
    |  panel       |  2. Panel displaying debug messages    |
    |              |  3. Other panels (in future versions)  |    
    |              |                                        |
    +--------------+----------------------------------------+
    | Status bar ?                                          |
    +-------------------------------------------------------+

This app has a Navigation panel at the left, where the selection made determines the content shown in the Main panel. 

For the demo there will be just two choices : (1) a panel that shows the same content as the Viewer app, and (2) a panel that shows textual debug messages generated by the Viewer. In future versions we'll add further panels that exercise other UI controls that we develop at CLF, for example controls that render a Synoptic, a Motor Control panel, and a Dependency Network. 

By having two apps that use the same Viewer component, we'll be demonstrating that we've mastered the techniques necessary to encapsulate the code and resources for a particular UI in a distinct Assembly.

*In a future WinUI-3 version the 'debug messages' panel would be a distinct Window that can be dragged onto second monitor. This is painful to achieve in UWP because of threading issues (but would be a good 'nice-to-have').*

### Alternative zoom modes ?

There are two possible idioms here :
1. As described above

2. Unconstrained panning and zooming, with an 'infinite canvas' where the dataset is rendered with the top left corner at the canvas origin, and zooming/panning adjust the position and size of the viewport through which you look at the canvas. You can zoom in and out indefinitely, and pan to anywhere you want, with no constraints. Constraints might seem useful (eg limiting the zoom out factor to x10), but with constraints, strange and non-intuitive behaviours ensue when you encounter edge cases. In particular, it's not possible to keep the same part of the image 'under' the mouse pointer at all times.

The app should implement #1, the 'constrained' option. An extension would be to support mode #2, and switch to this using a choice available via a Preferences setting.

*Hmm, on UWP and WinUI, and Android, do we have any say in how this works, or is the behaviour baked into the framework ? I'm basing this on how mouse based zooming and dragging is done in WPF.*

*In some pan/zoom designs, scroll bars appear at the right and bottom when you're zoomed in. This is not deemed necessary or useful.*


Example of mode #2 

        +---------------------------------+
        |            *                    |     Zoomed out a very long way.
        |            *                    |
        |            *                    |     The graphs show the profiles as usual,
        |            *                    |     but squished up into a small space.
    ##  |************::|::****************|     The cross section cordinates are shown as
    ##  |            --+--                |     coloured lines overlaid on the tiny image, as usual.
    ##  |            ::|::                |
        |            *                    |     Note the additional lines drawn as '*'.
        |            *                    |     These indicate the 'origin' of the dataset
        +---------------------------------+     coordinate space, so that if you're zoomed out
                     #####                      and panned a long way off so that nothing of the
                     #####                      image is in view, you can still see where you are




### Cross section graph appearance

The default graphs should draw straight lines between each data point. An alternative that could be set via preferences is to draw vertical lines to represent the data at each point. 



