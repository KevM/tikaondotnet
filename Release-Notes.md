## 1.14

- Extract all headers from MSG/RFC822 (TIKA-2122).

- Upgrade metadata-extractor to 2.9.1 (TIKA-2113).

- Extract PDF DocInfo metadata into separate keys to prevent
    overwriting by XMP metadata (TIKA-2057).

- Re-enable fileUrl for tika-server (TIKA-2081).  If you choose,
    to use this feature, beware of the security vulnerabilities!
    See: https://cve.mitre.org/cgi-bin/cvename.cgi?name=CVE-2015-3271

- Add Tesseract's hOCR output format as an option, via Eric Pugh
    (TIKA-2093)

- Extract macros from MSOffice files (TIKA-2069).

- Maintain passed-in mime in TXTParser (TIKA-2047).

- Upgrade to POI.3-15 (TIKA-2013).

- Upgrade to PDFBox 2.0.3 (TIKA-2051).

- Fix hyperlinks with formatting in DOC and DOCX (TIKA-1255
    and TIKA-2078)

- Tika now is integrated with the Tensorflow library from Google 
    and it can use its Inception v3 image classification model to 
    identify objects in images (TIKA-1993).

- Parser configuration is now type-safe and parameters for parsers
    can have assigned types (TIKA-1508, TIKA-1986).

- Prevent OOM/permanent hang on some corrupt CHM files (TIKA-2040).

- Upgrade ICU4J charset detection components to fix multithreading
    bug (TIKA-2041).

- Upgrade to Jackcess 2.1.4 (TIKA-2039).

- Maintain more significant digits in cells of "General" format
    in XLS and XLSX (TIKA-2025).

- Avoid mark/reset issues when extracting or detecting embedded resources
    in RFC822 emails (TIKA-2037).

- Improving accuracy of Tesseract for better extraction of numeric 
    and alphanumeric text from images (TIKA-2021, TIKA-2031).

- Improve extraction of embedded documents from PPT, PPTX and XLSX
    (TIKA-2026).

- Add parser for applefile (AppleSingle) (TIKA-2022).

- Add mime types, mime magic and/or globs for:
   - Endnote Import File (TIKA-2011)
   - DJVU files (TIKA-2009)
   - MS Owner File (TIKA-2008)
   - Windows Media Metafile (TIKA-2004)
   - iCal and vCalendar (TIKA-2006)
   - MBOX (TIKA-2042)
   - Stata DTA (TIKA-2064)

- Add configurable maximum threshold for number of events extracted
    from the XMP Media Management Schema in JempboxExtractor (TIKA-1999).

- Integrate TesseractOCR with full page image rendering for PDFs (TIKA-1994).

- Add mime detection via Nick C and parser for DBF files (TIKA-1513).
  
- Add mime detection and parsers for MSOffice 2003 XML Word
    and Excel formats (TIKA-1958).

- Extract hyperlinks from PPT, PPTX, XSLX (TIKA-1454).

- Upgrade to Commons Compress 1.12 (supports progress on TIKA-1358)


## 1.13.1

- Update TextExtractor dependency metadata to properly sync with TikaOnDotNet

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
