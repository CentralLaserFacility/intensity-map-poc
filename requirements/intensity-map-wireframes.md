## Default view



![Default view](https://share.balsamiq.com/c/xn4A533GMav6RNe7CDFEja.png)





Intensity map of the data. 

Dropdown menu at the top to allow choice of different sources for intensity map data. Intensity maps do not in general have the same x,y pixel counts. 



**Max display value** section: 

The colour map dropdown allows a choice of colour palettes for the image display. 

The slider and text box are bound to the same value. When **Automatic** is not selected, the colour map is remapped to pixel values between zero and this value.

When **Automatic** is selected, the colour map is remapped to pixels values between zero and the maximum pixel intensity in the data. Turning this switch on disables the slide and text entry. When this switch is off, the slider and text entry are enabled. 

The max values for the slider and text entry should be updated according to the maximum allowable value from the source of the intensity map. The text entry should perform entry validation so only values > 0 and < _max_ are accepted. 



**Profiles** section:

One-dimensional cross section through the data are displayed below (for the horizontal cross section) and to the left (for the vertical cross section) of the image. 

The **X** and **Y** slider and text entries set the position in the intensity map at which these cross sections are taken.  The maximum allowable values for the text entry and the slider should update according to the intensity map dimensions. The text entry should perform entry validation so only values > 0 and < _size_ are accepted. 

When a new source is chosen from the dropdown menu, the profile positions should reset to the central pixel position for that source.

The **Visible** switch turns these graphs on and off (see **No profile view** below). The intensity map should rescale to fill the available space. 

The profile graphs should display tic marks and numeric values. The y-axis for the graphs, which is the intensity value for each pixel in the cross section, should have a maximum value set by the value determined in the **Max display value** section. The x-axis for the profiles, which is the pixel position, should have a range that covers the the current view. That is to say, if the user zooms in to a smaller region the x-axis range should update accordingly. 



**Settings** button:

Launch a new window as shown on the right. These settings are source dependent, and so change values only for the currently selected source. The title bar of the settings windows should reflect this. In this model, most of the controls here are dummy controls that do nothing and need no implementation - just display for aesthetic purposes. The two exceptions are:

**Gain** - a value between 0 and 1000, with entry validation to constrain it. On change, after a loss of focus event or pressing Enter, this will apply change the values in the intensity map. Higher values of Gain will result in higher intensities in the intensity map, with a simple liner conversion. 

**Trigger** - this simulates switching from streaming images at full speed (**Internal** setting), to synchronising data capture with an electrical event received by the source (**External** setting). These events come at 10Hz in our system, and changing the drop down will thus change the image rate from maximum to 10 Hz. 

When this window is displayed, the user must still be able to see the main display window, to see how changes are affecting the intensity map data in real time. If this window is open, and the user clicks again on the **Settings** button, it should not launch a new window, but bring this one to the front. 



**Save image** button:

Launches a file save dialog with the option to export the current intensity map as an image. A choice of standard image formats should be provided (e.g. png, bmp, tiff). 





## No profile view

![No profile view](https://share.balsamiq.com/c/pdh5f64LMTKwGK2HKLmRkt.png)

In this case the **Visible** switch in the **Profiles** section has been set to off, and the profiles are hidden. The image rescales accordingly. 

Also, the **Automatic** switch in the **Max display value** section has been set to off, so the slide and text box are enabled. 

The user had expanded the dropdown box at the top of the screen, and it shows a list of available sources. Clicking on one of these switches to that source.





## Loss of communication with the source

![Loss of comms](https://share.balsamiq.com/c/Wy8aNRwXEXAyiAg8jrgSE.png)

When the system loses communication with one or more sources, the user is informed by:

- the source name shows (disconnected) in the drop down menu
- any of the controls that are bound to properties of that source show a coloured border. By default this is magenta (our users wear goggles when working with lasers. They cut out a lot of the green and red spectrum so we need to choose something they see clearly). For this demo it can be any colour. 