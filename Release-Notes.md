## 1.13

- Updated Tika dependency to 1.13.

## 1.12.2

- Breaking Change: Renamed the namespace and assembly name of TikaOnDotNet to match the Nuget id (was `tika-app`). This should only affect the resulting filename of the assembly. All Tika code is namespaced with a Java style (com.apache.{yadda yadda}).
- Fix TextExtractor dependency so that it is using a "working" version of TikaOnDotNet (1.12.2)

## 1.12.1

- This release fixes the minimum version for child dependencies. #41
- Fix project urls in nugets.

## 1.12.0

**Breaking Change**: This release splits **TikaOneDotNet** into two Nugets.
- TikaOnDotNet
- TikaOnDotNet.TextExtractor

Existing Nuget users who have problems are encouraged to take a dependency on [TikaOnDotNet.TextExtractor](https://www.nuget.org/packages/TikaOnDotNet.TextExtractor/).

Note: Even though this is a breaking change we are not updating our major version as we are trying to stay in sync with Tika.

Build automation has been revamped and entirely automated. It is now super easy to upgrade to newer versions of [Tika](http://tika.apache.org/) or [IKVM](http://www.ikvm.net).

Added [AppVeyor Continuous Integration](https://ci.appveyor.com/project/KevM/tikaondotnet).

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
