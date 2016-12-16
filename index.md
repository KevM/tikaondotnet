### Tika On DotNet

> The Apache [Tika™](http://tika.apache.org/)  toolkit detects and extracts metadata and structured text content from various documents using existing parser libraries.

#### Usage 

It is best to take a dependency on the Nugets we produce: 

- [TikaOnDotNet](https://www.nuget.org/packages/TikaOnDotnet./)
- [TikaOnDotNet.TextExtractor](https://www.nuget.org/packages/TikaOnDotnet.TextExtractor/)

#### What is this?

This project contains all the .Net [assemblies](https://github.com/KevM/tikaondotnet/tree/master/lib) necessary to use the wonderful [Tika](http://tika.apache.org/) library in your .Net applications.

Tika is a Apache Foundation open source project written in Java. It may sound scary but it is possible to leverage Java libraries from .Net applications without any TCP sockets or web services getting caught in the crossfire using [IKVM](http://www.ikvm.net/). I've done the hard work for you and built the .Net version of Tika for you and bundled the supporting IKVM runtime libraies. 

#### Tests

A basic set of unit tests are present in this project to verify that Tika is working. These tests extract text from test documents. The following rich document types are tested:

* Adobe PDF - .pdf
* Microsoft Word - .doc and .docx
* Microsoft Excel - .xls and .xlsx
* Microsoft PowerPoint - .ppt and .pptx
* Rich Text Format - .rtf
* Zip files - .zip (only a listing of the filenames in the .zip file are extracted)
* JPEG - .jpg (image metadata)

For more details on how this is accomplished checkout [this blog post](http://www.dovetailsoftware.com/blogs/kmiller/archive/2010/07/02/using-the-tika-java-library-in-your-net-application-with-ikvm) from @KevM

### Cloning 

You can use your favorite git client to clone this repository. Please do!

```
$ git clone git@github.com:KevM/tikaondotnet.git
$ cd tikadotnet
```

### Authors and Contributors
This project was created by @KevM to support a project created by @DovetailSoftware. 

### Support or Contact
If you have any problems. Create an issue and we can talk about it.