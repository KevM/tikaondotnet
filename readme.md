Developers Guid to Tika on .NET
===============================

This project is a simple wrapper around the very excellent and robust [Tika](http://tika.apache.org/) text extraction Java library. 

##Building TikaOnDotNet##

This project uses [rake](http://rake.rubyforge.org/) for build automation. 

1.[Install Ruby](http://rubyinstaller.org/)
2. Install Rake ```gem install rake```
3. ```rake```

If successful this should have built and run the Tika text extraction integration tests.

To ensure you have all the required gems installed [Bundler](http://bundler.io/) is used and should be automatically installed and setup the first time you rake the project. To manage our Nuget dependencies we are using a tool called [Ripple}(http://darthfubumvc.github.io/ripple/ripple/gettingstarted/overview/) but you should hopefully not have to worry about that. 

##Building the Tika-App .NET Assembly##

To accomplish this the **tika-app-{version}.jar** is transpiled into a .Net assembly using the [IKVM](http://www.ikvm.net/) compiler. 

```
ikvmc.exe -target:library -assembly:tika-app tika-app-{version}.jar
```

The result of this process is a .NET assembly ```tika-app.dll``` which is stored in this repo's [lib directory](https://github.com/KevM/tikaondotnet/tree/master/lib).

The Tika app .jar file can be downloaded from the [Tika Download page](http://tika.apache.org/download.html).

##Updating the IKVM Nuget dependency##

```
ripple update -n IKVM -p TikaOnDotNet -v {version}
```

##Releasing TikaOnDotNet##

There is a handy ```release.bat``` which will create a release build and package the nuget. The resulting nuget package will be in the **artifacts** directory.

