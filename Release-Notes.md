## 1.12.0

Introducing **TikaOneDotNet.App** Nuget.

TikaOneDotNet has been separated into two Nugets.  
Updated build automation to build `tika-app-{version}.dll` and package it as a
new standalone nuget **TikaOneDotNet.App**. This Nuget is the minimum you need
to work with Tika.

**TODO** add more details here.

## 1.7.0

[Updated to Tika 1.7](http://clarify.dovetailsoftware.com/kmiller/2015/02/06/tikaondotnet-now-supports-tika-1-7/).

## 1.6.3

We accidentally got the release numbers out of sync with the version of Tika. TikaOnDotnet version **1.6.3** moves our Tika support to version **1.6**.

**Breaking Change**

When using the TextExtractor on ``.zip` files the contents of the files compressed inside the .zip are now extracted. Previously only their filenames were extracted.

## 1.4.0

[Tika 1.4 release notes](http://tika.apache.org/1.4/)

There have been many versions of Tika since our last release and I tried each of them out but there were failing tests when extracting older PowerPoint files. In this release all tests are green!

This release is also [available in the Nuget gallery](https://nuget.org/packages/TikaOnDotNet/) - [Blog post](http://blogs.dovetailsoftware.com/blogs/kmiller/archive/2013/07/12/tikaondotnet-14-released-as-a-nuget).
