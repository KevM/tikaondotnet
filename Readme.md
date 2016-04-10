Tika on .NET
============

This project is a simple wrapper around the very excellent and robust
[Tika](http://tika.apache.org/) text extraction Java library. This project produces two nugets:
- TikaOnDotNet - A straight [IKVM](http://www.ikvm.net/userguide/ikvmc.html) hosted port of Java Tika project.
- TikaOnDotNet.TextExtractor - Use Tika to extract text from rich documents.

## Building

This project uses [FAKE](http://fsharp.github.io/FAKE/) for build automation and
[Paket](https://fsprojects.github.io/Paket/) for managing dependencies.

**Note:** Your first build should be from the command line to get the assembly version file created.

`./build.cmd`

The default build will run our Tika text extraction integration tests.

### Building Nugets

It's easy to produce updated `.nupkg` packages.

`./build.cmd PackageNugets`

Look in `./artifacts` for the resulting `.nupkg` files.

## Updating Tika

When a new Tika release comes out you can follow the instructions below to get on the newest version.

1. Edit the `paket.dependencies` file to point to the new release of the Tika Jar file.
2. `./build.cmd PackageNugets`

Follow this quick procedure to find the latest Tika release Jar archive:

1. Visit the [Tika download page](https://tika.apache.org/download.html)
2. Click on the **Mirrors for tika-app-<version>.jar** link.
3. Find the Jar hosted on **www-us.apache.org**.
4. Copy this url into `paket.dependencies`.

Note: The automation looks for the Tika Jar file under `paket-files/<hostname>/*.jar`. If you do not use the **www-us.apache.org** url you'll need to update `build.fsx`.

## Updating IKVM

When a new release of IKVM comes out you can follow the instructions below to get on the newest version.

1. Edit the `paket.dependencies` file
  - Point the IKVM tools binary to the [new release](http://weblog.ikvm.net).
  - Point the IKVM nuget to the matching version of the Nuget.
2. `./build.cmd PackageNugets`

Note: The automation looks for the IKVM compiler in `./bin/ikvmc.exe` of the expanded archive in `paket-files`.

You should make sure that `paket.depdendencies`linked to the same version for the Nuget of IKVM and the build tools

```
//IKVM dependencies - the nuget and tool versions need to be in sync.
nuget IKVM <version>
http http://www.frijters.net/ikvmbin-<version>.zip
```

Looking for updated versions of IKVM? [Check out their blog](http://weblog.ikvm.net).
