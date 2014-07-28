XNA-Zoom-Scope
==============

XNA Code for scope zooming in an unzoomed environment.

File contains a background image and a crosshair image.
The intent of the class is to show the background image normally and have the crosshairs zoom in on the section behind the scope.


NOTES
=====
If you make your own crosshair, be warned that the pixels in the "lens" must have a low but non-zero alpha value.
There is code in the class to remove pixels with alpha values of 0 and display the normal background there.
This code is present to allow non-rectangular crosshair images to be used without creating graphic issues in the corners of the crosshairs.
