# Loonim Engine

Depends on: IO/BinaryIO, IO/Values, Blaze/*

Loonim is an image generator which is fine tuned for operating at runtime. In here you'll find the core engine only, featuring both GPU and CPU modes.

## Using Loonim Standalone

Make sure you've pulled all the dependencies into your project too, then compile with the NO_BLADE_RUNTIME flag. This flag makes sure that none of the Unity specific GPU code is included.

## Using Loonim inside a Unity project

This is the most common form that Loonim takes; using it this way allows you to also make use of the GPU mode. See the Loonim/UnityProject repo for an example of a sample Unity project containing Loonim.

## The file format

The Loonim Engine has its own compact binary file format (.lim files) which is described and developed over in the Loonim/FileFormat repo.