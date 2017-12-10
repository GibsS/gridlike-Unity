# Gridlike - A Unity3D library 

Gridlike is a library for creating grid based games similar to Terraria and King Arthur's Gold.

The library allows you to define your blocks information in a "tile atlas" and create grids in the scene editor or programmatically.

This library offers an alternative to Unity's 2017 tile map system that is centered more around programmatically definable and modifiable infinite grids:
It comes with serialization functionality and helpers to create procedurally generated grids.

The gridlike package also comes with a [character controller](https://github.com/cjddmut/Unity-2D-Platformer-Controller) originally made 
by [cjddmut](https://github.com/cjddmut). It is modified and improved to better work with Gridlike. It is an amazing character controller 
and well worth checking out. You are free to use any other 2D collider based character controller with Gridlike.

## Testing it out

I made a small (see very very small and unpolished) game using Gridlike called Gridship. You can test it out by following the link below.
It is not much to look at but it will give you a sense of what you can accomplish with the library. It is important to note
that despite the tiles (or blocks if you prefer) being a single color, they are actually sprite and could be replaced by actual 
non monochromatic sprites.

You might need to wait for a few seconds before the game starts:
[A small demo](https://gibss.github.io/test/gridlike-unity/Gridship3/)

## How to install

You can find the latest release here: https://github.com/GibsS/gridlike-Unity/releases

Download link: [gridlike.unitypackage](https://github.com/GibsS/gridlike-Unity/releases/download/1.0.0/gridlike.unitypackage)

## How to get started

The code is commented and the package is organized well enough (I hope) but there is no manual yet. To wrap your head around the library
your best hope is to check out the scenes associated to Gridship at Gridlike/Samples/Gridship/Scenes. Does few scenes pretty much cover 
everything you can do with the library.

Note: Gridlike was created and tested with a project gravity of -30 instead of the default -9.8. Might be worse changing for less floaty
movement.
