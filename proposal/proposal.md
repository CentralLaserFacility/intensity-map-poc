# UNO - CLF POC

The proof-of-concept project demonstrates an interactive 'Viewer' component for Intensity Map images.

We would like this run, in order of importance, on Windows desktop, Linux desktop (Ubuntu 20), Android tablet, Webpage

The images that we work with in this demo are simulated, rather than being acquired in real time from a camera. The demo will evolve into a useful utility that will (A) connect to live sources of IntensityMap data, and (B) will utilise the WinUI-3 framework rather than UWP. 


### Viewer Component and App

The project will develop a reusable component, the 'IntensityMapViewer'. The Viewer component will be packaged as an Assembly (ie a DLL) that is referenced by the demo app.

The appearance of the Intensity Map Viewer is shown below in the Wireframes section. 

The app window has two sections : the Viewer itself, and a Navigation control that selects the source of the data that's being displayed by the Viewer.


## Wireframes

This is a screen shot of our existing display :

![](existing-software-snapshot.png)

We don't wish to replicate this entirely. The following wireframes indicate the aspects we're most interested in for the POC.

#### Standard view and camera settings panel
![Standard view](https://share.balsamiq.com/c/xn4A533GMav6RNe7CDFEja.png)

#### View with no profile graphs
![No profile view](https://share.balsamiq.com/c/pdh5f64LMTKwGK2HKLmRkt.png)

#### View when communication with the source is lost
![Loss of comms](https://share.balsamiq.com/c/Wy8aNRwXEXAyiAg8jrgSE.png)

## Domain Model

- Laser beam: a narrow beam of light that is manipulated by reflection off of mirrors. To change the direction of light so that it goes to the desired location, we adjust the angle of the mirror it reflect off. This process is known as laser alignment. 

- Sources: the laser beam is infra-red, and thus not visible by the naked eye (even if it was, it would be dangerous to look at it as it's too intense). To know where the laser beam is, we place a camera or other detector in the beam. We use the term source as a generalisation of camera, as for other applications of the Intensity Map viewer the source may be a different piece of technology that generates data of the same format. For the purposes of this POC source and camera are interchangable and equivalent.

- Intensity Map: a 2D array of either 8- or 16-bit values that encode the intensity of the light at a given point on the Camera detection area. 

- Data Set: _CDG:  can't see how this differs from Intensity Map_

- Image: A representation of the intensity map that can be rendered by the user interface. It combines the intensity values with a colour palette.

- Profile graphs ('cross sections'): 1D cross-sections through the intensity data that show the intensity value at each pixel.

- Laser Scientist: the person responsible for ensuring the laser beam is correctly setup, and who uses the intensity map on a camera to do this.

- Preference

The intensity data captured by a camera is always in 'grey-scale' format ; typically 640x480 pixels with 8 bit pixels that represent an intensity (brightness') from 0 (black) to 255 (bright white). Typically, the pixels in a captured dataset don't cover the entire range of values - in some cases we'll see values between zero and about 20, which makes for a very dim image if these are shown as greyscale pixels on the display. The visual representation can be improved by adjusting the camera settings (gain and/or exposure time) but as a convenience we provide a facility for stretching the data in the Viewer itself, by a process we call 'Normalisation'.

When an image is displayed on screen, a mapping formula is applied which typically (A) normalises the pixel values to a specified maximum value, and (B) maps the normalised values into false colours via a palette.

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
- Selecting a source of data
- Configuring the source settings
- Choosing how the image is displayed 
- Enabling/disabling the cross-section graphs 
- Choosing the X/Y coordinates for the cross-section graphs 
- Pan and zoom on the image
- Saving to a file
- Pausing and resuming acquisition
- Simulating errors such as loss-of-comms 
- Resizing the window 
- Closing and re-starting
- Selecting a different Theme
- Setting preferences for the app 

Here's a sketch to identify the regions we'll be talking about :

    +-----------------------------------------------+
    | Source-Selector                               |
    +-----------------------------------------------+

    Intensity-Map-Panel
    +---------------------------------------------------------------------+
    | +-----------------------------------------+ +---------------------+ |
    | |                                         | | Image               | |    
    | |  Intensity-Map-Display                  | | Presentation        | |
    | |                                         | | Settings            | |
    | |  ### +-------------------------------+  | |                     | |
    | |  ### |        |                      |  | +---------------------+ |
    | |  ### |        |                      |  | +---------------------+ |
    | |  ### | - - - -+- - - - - - - - - - - |  | | Profile             | |
    | |  ### |        |                      |  | | Graph               | |
    | |  ### |        |                      |  | | Settings            | |
    | |  ### |        |                      |  | |                     | |
    | |  ### |        |                      |  | +---------------------+ |
    | |  ### +-------------------------------+  | +---------------------+ |
    | |                                         | | Source Settings ... | |
    | |      #################################  | +---------------------+ |
    | |      #################################  | +---------------------+ |
    | |                                         | | Save Image ...      | |
    | +-----------------------------------------+ +---------------------+ |
    | +-----------------------------------------------------------------+ |
    | | Other Miscellaneous UI elements here                            | |
    | +-----------------------------------------------------------------+ |
    +---------------------------------------------------------------------+

The 'Source-Selector' is an independent UserControl that might contain a combo box or a tree view as discussed earlier. It is not part of the 'Intensity-Map-Panel', but it determines which 'Source' is displayed in that Panel.

The Intensity-Map-Panel is a UserControl that is composed of several other UserControls :
- An Intensity-Map-Display. This presents an image in either grey scale or false colours, and also (if enabled) graphs of the horizontal and vertical intensity profiles.
- An 'Image-Presentation-Settings' panel to configure how the underlying 8-bit pixel data is presented as an RGB image.
- A 'Profile-Graph-Settings' panel that configures the graph displays below and to the left of the image.
- A button to bring up a panel that configures the settings of the current Source, including the camera's gain and exposure time.
- A button to bring up a modal dialog that lets you save the current image as a disk file.
- Miscellaneous elements, such as a display of the current fps, whether we're in Pause/Play mode, maximum intensity value.

Generally speaking, when you hover over something a useful tooltip pops up.
- When you hover over the image, the tooltip shows the X-Y position (in terms of Source pixel indeces) and the un-normalised intensity value.
- When you hover over a button, the tooltip provides a brief explanation of what will happen if you click.
- When you hover over a UI element that is disabled (greyed-out) the tooltip explains why that feature is not available. For example if the Source Settings button is greyed out because that Panel is already active, the tooltip will mention that.

Keyboard shortuts will be provided for common actions, tbd. For example, zoom-out-completely ; play/pause.

We might also make use of the mouse wheel for making certain adjustments, possibly in conjunction with the Shift and Ctrl keys. Details tbd, but the scroll wheel will be very handly for zooming in and out of the image, and also might provide a very convenient way to tweak the 'max intensity value' for image normalisation that we'll describe later.

### The 'intensity map' display

This is the most interesting and important element in the UI. It shows a greyscale or false-colour image that represents the 2D map of the pixel value as acquired by the selected Source. 

Alongside the image are graphs that show cross-sections through the dataset at particular X and Y coordinates, which we refer to as the 'cross-section coordinates'. 
- The left hand graph shows the intensity values along a vertical slice passing through the specified point - that is, values which all have the same X coordinate. 
- The graph at the bottom shows values along a horizontal slice - that is, values which all have the same Y coordinate.

The X-Y coordinates of the horizontal and vertical cross-sections are indicated by coloured lines that are overlaid onto the image, as indicated in the ASCII-art above. The colour and thickness of the 'cross-section lines' can be configured in the Preferences panel.


The displayed coordinates and values always pertain to the underlying intensity map, not to the coordinates in the displayed image or the RGB values.

### Selecting a source of data

There's a choice of several sources. The available source is chosen from a combo box with a drop down list of textual summaries. Each line shows the name of the source, and indicate whether it's online or offline.

You are permitted to select a source that's currently offline, but the displayed image will be blank.

One of the source options is a 'null' source, which provides an 'empty' dataset of size 640x480 that results in an image that is completely black.

For the demo, all the sources will be simulated. They will differ in terms of the dimensions of the dataset, and the character of the intensity-map data : nice datasets with a good dynamic range, and poor ones with low intensity levels and a lot of noise.

The dataset dimensions will be either 640x480, 320x240, or 2048x2048 pixels.

There are various 'settings' associated with a source, such as the 'normalisation' parameters, the ColourMap choice, the Profile settings, and the pan/zoom parameters, which collectively govern how the Source dataset is displayed as an image. These settings are persisted, and are restored when a source is selected. The possible exception here is pan/zoom - persistence of these settings is set via preferences as noted above. 

Once the user has selected a source, the UI refreshes to show that source - not only the most recently captured dataset, but also the restored settings.

If the chosen source goes offline, the display will continue to show the most recently acquired image but the UI will provide a prominent indication of this, for example by turning the background red or purple (see wireframes for an example).

### Configuring the source settings

Clicking on the 'Source Settings' button brings up a modal dialog (see right-hand panel of first wireframe) that controls how the parameters of the active Source. Intensity map acquisition continues, and image updates continue to happen. The Settings button on the main panel is disabled while this dialog is open to prevent multiple dialogs being spawned. The source selector combobox is also disabled while this panel is open. The rest of the control on the main panel should function as normal. 

While the dialog is active you can adjust the Gain and the Exposure time. Changes in these values are reflected immediately in the image display ; increasing the Gain or Exposure time will make the intensity of the next acquired intensity map brighter.

Adjustments can be made either via sliders or by text entry. Text entries are validated, and changes are applied when focus leaves the text box or when you press 'ENTER'.

Gain values are constrained to be in the range 0 to 1000. Default is 50.

Exposure times can be be between 2uS and 100uS. The wireframe shows a text entry for this, but could also be a slider. Default is 20uS.

The 'Trigger' option offers two choices :

- Internal trigger, where the source provides images at full speed, 30fps.
- External trigger, where the source provides images at full speed, 10fps.

### Choosing how the intensity map data is presented 

This panel lets you adjust the Normalisation and select the Colour Map. These parameters determine the way that the intensity data (0-255) is translated into image pixels (RGB).

Normalisation can be selected as either Manual or Automatic. 

- In 'manual' mode you explicitly specify a 'maximum intensity value', say 100, and the pixel values in the intensity map will all be scaled such that those with a nominal value of 100 or above will get mapped to the highest intensity value, equivalent to 255, when the data is converted to visual pixels. 
- In 'auto' mode, the 'maximum intensity value' is determined by scanning the intensity map and finding the pixel with the highest value.

The maximum intensity value can be set via a sider or via text entry. In 'auto' mode, the slider and the text box are read-only, and they indicate the maximum value that has been found. When you transition from 'auto' to 'manual' mode, the value stays the same until you change it.

As an optional extra, it might be useful to be able to also adjust the 'manual' value using the mouse's scroll wheel. Perhaps while the mouse is hovering over the image with 'shift' held down, or while the mouse is hovering over the 'Presentation' panel.

The 'colour map' chooser is a combo box that offers a choice of three options :
- Greyscale
- The 'JET' colour scheme
- Binary (as implemented by National Instruments: https://zone.ni.com/reference/en-XX/help/370281AG-01/nivisionconcepts/palettes/)

Binary mode is a nice-to-have, not essential. It would be implemented entirely in the ViewModel layer.


### Panning and zooming

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

As you pan and zoom, the lines representing the 'cross-section coordinates' move along with the underlying image.

It is essential that the proper aspect ratio is preserved in the image display that is generated from the dataset. So depending on how the user has panned and zoomed, the area assigned for the display might need to show 'blank' areas that lie outside the bounds of the IntensityMap. 

When the Source is changed, the zoom settings should be reset to 'best fit' showing the full image. Optional improvement would be to have a preferences setting that would change this behaviour to remember pan/zoom levels for each Source, and restore those settings when a Source is changed.

When zoomed in to a subsection of the image, the cross-section graphs should only show data for the region visible in view.


Typical scenarios are illustrated below.

 Scenario 1

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

### Saving to a file

There is a button to let you save the current image to a file. A dialog comes up inviting you save the image in a file (in a 'standard' location, for the POC) whose name defaults to one based on the Source name and the intensity map's Time Stamp. You can accept this suggested name, or override it with a different one. The file formats supported are jpeg, png and tiff.

Saving is only enabled on when acquisition is paused.

You can save either just the main image (in which case the horizontal and vertical lines identifying the cross section coordinates are not included), or you can save the image and also the graphs.


### Pausing and resuming acquisition

The default is to refresh the displayed image and graphs as every new frame of data is acquired. However it will be useful to be able to 'freeze' the display to show the most recent dataset, in order to examine the image in detail without it changing.

A switch labelled 'Play/Pause' will be provided (exact label but an example is given in the first wireframe). The current mode will be indicated in the UI. 


### Simulating errors such as loss-of-comms

Tbd, maybe have a check box on the status bar ?

This lets us demonstrate how loss-of-comms is indicated in the UI.


### Resizing the window 

The image display changes size to make best use of the available space.


### Selecting a different theme

An optional extra would be to provide different colour themes settable by preferences.

The Viewer will be used in offices, and also in laboratories where safety considerations require that users wear special goggles to protect their eyes from the laser. The optimum colour for things such as warning and error indicators (eg 'loss of comms') is different in those two environments. For example in a control room 'red' is a good choice, but in the lab 'magenta' is strongly preferred as it shows up more distinctly through goggles.

In addition to the colour, the user could wish to set other visual attributes such as font size. This would be done via Preferences rather than via a Theme, although the 'Lab' theme might configure a larger font size as the default.

### Preference settings

There is a Preferences panel that you can bring up to change various settings.

Settings relating to the Viewer :

- Line thickness and colour of the X-Y profile position indicator.


Settings relating to the app generally :

- Theme, selectable as ControlRoomTheme or LabTheme.
- Font Size scaling. As the Viewer will be used with screens of various sizes and with the user at varying distances (particularly in the lab), it will be useful to be able to rescale the font sizes over a range of say 0.5x to 2x the nominal size, via a slider. The space taken up by the various panels will adapt appropriately.

Changes get applied instantly in the UI, there's no 'submit' or 'undo'.

Nice if the current settings are persisted (as a JSON file), and restored when you close and restart the Viewer.

### Closing / restarting the Viewer app

Either 
- (A) the most recently active Source is restored, along with the settings that were in force.
- or (B) no source is selected, and it's up to the user to choose one.

The choice here could be a Preferences setting.

The default is (A), we restore the Source when the Viewer is restarted.

If we run out of time for the POC, (B) will be fine.


----
----

## Technical notes

Ideas for the internals and the implementation, for discussion.

### Windows UI framework

If time allows, and if the WinUI-3 preview looks to be stable enough, we will consider migrating to WinUI as part of the POC ; however the primary target here is UWP, which is well proven and stable.

### Operations applied to the IntensityMap acquired from the Source

In the POC, a 'typical' intensity map for each Source will be loaded from a file, with values in the range 0-255. Gain/Exposure scaling will be applied to simulate the data being acquired from a camera, and random noise will be added so as to make successive datasets slightly different and give the illusion that we're actually capturing live images.

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
    | Falsely coloured    | 32 bit pixel values (ARGB)
    | IntensityMapImage   | Dimensions defined by the UI.
    +---------------------+

The final output from this pipeline is what gets displayed as an RGB image in the Viewer, within the available area that the user might have adjusted by resizing the window.

The pixel dimensions of the displayed image will almost certainly not match the pixel dimensions of the dataset, so some kind of resampling will need to be performed. Best if we do this via the view API's rather than in our code, as that will be more efficient. Ideally the stretching will be performed by duplicating pixel values rather than a technique such as bilinear interpolation, as this will make 'dead' pixels more clearly obvious.

The logic to present the appropriate segments of the dataset to the View layer, according to the requested Pan and Zoom parameters, will be implemented in the ViewModel layer and the associated helper functions. This will allow us to keep the View itself as straightforward and 'dumb' as possible ; the View it will just see an Image that needs to be painted in the assigned area.

### View Models

The UI for the Viewer will be backed by ViewModel classes coded in C#, for which CLF will provide an initial implementation. We'll use the MVVM library from the Community Toolkit.

Where we're simulating 'live' data such as streams of camera images, an internal timer will trigger the ViewModels to cycle through a baked-in sequence of snapshots loaded from resource files, at a rate of up to 30fps.

Rather than have the entire UI defined in a single 'flat' ViewModel, we'll structure things so that there are distinct ViewModel classes for distinct aspects of the overall Viewer control. This will make it easier for us to develop variants of the Viewer in future, for example one that displays several Intensity Map images side by side in a grid.

Provisionally, we'll define ViewModels for
- Viewer (top level VM is composed of other VM's plus some simple flags and Commands)
- ImageDisplay, with optional 'cross sectional profile' graphs and Pan/Zoom functionality
- SourceSettings (to control gain, exposure etc in the simulated camera)
- PresentationSettings (false-colour selection, enabling profile graphs etc)
- ViewerPreferences
- AppPreferences

Probably each of these VM's will be associated with a UserControl that provides the visuals. The 'main' UserControl for the Viewer will contain instances of the other UserControls, mirroring the composition structure of the ViewModel classes.

*Just ideas, we'll take advice on this !!*

ViewModel interactions - use the Messenger that comes with the Toolkit MVVM ?

### ViewModel sketches

Initially just 'interface' definitions, for discussion.

### Pan/Zoom

The pan/zoom settings would be coordinated via a helper class.

### Exception handling

Nothing should cause a crash, any uncaught exceptions should be caught at the top level and logged somewhere useful.

### Async

Nice if we can have some async commands with a proper bulletproof implementation behind them, illustrating best practice.

Also, good patterns for initialising class instances from async calls that are forbidden in the constructor.

### Image resampling

Several methods to choose from :
- Bilinear, as implemented in the Media API' ?
- Binning, NxN as required by the zoom factor
- Duplication, so we get crisp squares rather than fuzzily smoothed pixels
- Bresenhams, to select particular lines/columns and ignore the rest

### Cross section graphs

We would prefer that the graphs for the POC are implemented using custom code in SkiaSharp, as this will give us an excuse to see how to do graphics in Skia. Also our particular requirements don't align particularly well with the features in standard Graph packages.

The cross-section graphs will also be exposed to the View as Images.

The graphs will probably be created using the Skia pixel-oriented API's to write onto bitmaps, as part of the process that builds the coloured 'image' bitmap from the active area of the dataset.

https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/basics/pixels

The graphs could be created as separate Images, which would reside in a distinct area of the View, or the two graphs and the intensity-map itself could be drawn onto a single bitmap painted into a single area. Need to explore the tradeoffs. 

### Challenges

- The most complicated aspect is the support for panning and zooming, which is highly desirable but could conceivably be left out of the POC if it's troublesome.
- We'll be converting the 2-D datasets (typically 640x480 pixels but possibly up to 2048x2048) into false-coloured images presented in a viewing panel of a different size, together with cross-section graphs, while preserving the aspect ratio, and ideally supporting pan/zoom. An update rate of 30fps should be feasible provided we can use the highly optimised built-in 'media' transformations to map between the 'dataset' and 'image' coordinate spaces and perform the necessary resampling. If these operations have to be done in our own C# code, performance may be an issue with 2048x2048 datasets.


---
---

## Possible extensions

### A second appliation:  Test Harness for the Viewer and other UI controls

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

### Alternative zoom modes

There are two possible idioms here :
1. As described above

2. Unconstrained panning and zooming, with an 'infinite canvas' where the dataset is rendered with the top left corner at the canvas origin, and zooming/panning adjust the position and size of the viewport through which you look at the canvas. You can zoom in and out indefinitely, and pan to anywhere you want, with no constraints. Constraints might seem useful (eg limiting the zoom out factor to x10), but with constraints, strange and non-intuitive behaviours ensue when you encounter edge cases. In particular, it's not possible to keep the same part of the image 'under' the mouse pointer at all times.

The app should #1, the 'constrained' option. An extension would be to support mode #2, and switch to this using a choice available via a Preferences setting.

*Hmm, on UWP and WinUI, and Android, do we have any say in how this works, or is the behavior baked into the framework ? I'm basing this on how mouse based zooming and dragging is done in WPF.*

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


### Alternative ways to display the available sources

In the POC, this is done with a combobox. When deployed on our site that might not be optimal. Other options to be tested are:

- As a list presented at the left hand side, or possibly a tree (where sources that are related in some way would be presented as a group).
- As a preview panel that shows not only the source names and their statuses, but also a thumbnail image from the camera.

*All of these presentation formats for the Source selection are possible without the need to change the underlying ViewModel code. The ViewModel for a source provides all the information required, and how to present it in the UI is a separate choice.*


### Settings panel behaviour

By default, this should be modeless. That's what is used now, but it could be users would prefer modal. For testing, it could be helpful to have a preference setting to switch between the two.  


### Configuring the 'height' of the corss-section graphs

Nice if we could adjust the 'height' that is available to display the Intensity value, independently on the Horizontal and Vertical graphs. Using a Grid Splitter ?


### Image timestamp

When we're paused, it might be useful to display the timestamp of the image that's being displayed (not relevant for the POC as the images are synthetic, but we could generate one).

### Data export

In a future version that works with actual live images we'll provide an option to save the acquired dataset, eg as a '.pgm' file (Portable Greymap).


### Preferences

Long term we might want to distinguish between 'App Preferences' and 'IntensityMapViewer Preferences' and persist them in separate files. App preferences would pertain to all the various UI's we might build, and accommodate settings such as the Theme and Font Scaling. IntensityMapViewer preferences would pertain just to the Viewer, and represent the X-Y indicator settings.

