using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TikaOnDotNet.TextExtraction;

namespace TikaOnDotNet.Tests
{
    [TestFixture]
    public class text_extraction
    {
        #region Consts
        /// <summary>
        /// The path to Tesseract
        /// </summary>
        const string TesseractPath = @"C:\Program Files (x86)\Tesseract-OCR";

        /// <summary>
        /// The path to ImageMagick
        /// </summary>
        private const string ImageMagickPath = @"C:\Program Files\ImageMagick-7.0.3-Q16\";
        #endregion

        #region Fields
        /// <summary>
        /// The <see cref="TextExtractor"/>
        /// </summary>
        private TextExtractor _textExtractor;

        /// <summary>
        /// The <see cref="TextExtractorOCR"/>
        /// </summary>
        private TextExtractorOCR _textExtractorOCR;
        #endregion

        #region Setup
        /// <summary>
        /// Setup default values for our unit tests
        /// </summary>
        [SetUp]
        public virtual void SetUp()
        {
            _textExtractor = new TextExtractor();

            _textExtractorOCR = new TextExtractorOCR();
            // Setting up tesseract
            _textExtractorOCR.TesseractOCRConfig.setTesseractPath(TesseractPath);

            // The path to ImageMagick
            _textExtractorOCR.TesseractOCRConfig.setImageMagickPath(ImageMagickPath);

            // The language to use
            _textExtractorOCR.TesseractOCRConfig.setLanguage("ENG");

            // Use ImageMagick to cleanup en enhace images for better OCRing
            _textExtractorOCR.TesseractOCRConfig.setEnableImageProcessing(1);

            // Max amount of time to OCR before it times out (default 120 seconds)
            _textExtractorOCR.TesseractOCRConfig.setTimeout(60);

            // Extract text from scanned pdf's (default false)
            _textExtractorOCR.ExtractFromScannedPdfs = true;
        }
        #endregion

        [Test]
        public void extract_by_filepath_should_add_filepath_to_metadata()
        {
            const string filePath = "files/apache.jpg";

            var textExtractionResult = _textExtractor.Extract(filePath);

            textExtractionResult.Metadata["FilePath"].Should().Be(filePath);
        }

        [Test]
        public void non_existing_files_should_fail_with_exception()
        {
            const string fileName = "files/doesnotexist.mp3";
            
            Action act = () => _textExtractor.Extract(fileName);

            act.ShouldThrow<TextExtractionException>()
                .Which.Message.Should().Contain(fileName);
        }

        [Test]
        public void non_existing_uri_should_fail_with_exception()
        {
            const string uri = "http://example.com/does/not/really/exist/mp3/repo/zzzarble.mp3";

            Action act = () => _textExtractor.Extract(new Uri(uri));

            act.ShouldThrow<TextExtractionException>();
        }

        [Test]
        public void should_be_able_to_delete_the_mp4_after_extraction()
        {
            var original = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), @"files\badgers.mp4"));
            var mp4 = original.CopyTo(Path.Combine(Directory.GetCurrentDirectory(), @"files\badgers.bak.mp4"));

            _textExtractor.Extract("files/badgers.bak.mp4");

            mp4.Delete();
            mp4.Exists.Should().BeFalse();
        }

        [Test]
        public void should_extract_contained_filenames_and_text_from_zips()
        {
            var textExtractionResult = _textExtractor.Extract("files/tika.zip");

            var fileNames = new List<string>(new[] {"Tika.docx", "Tika.pptx", "tika.xlsx"});

            // Verify all expected files are there
            fileNames.ForEach(name => textExtractionResult.Text.Should().Contain(name));

            // We should find the same string once for every file in the zip. if we split the string on file names
            // this should break out the content into different strings to confirm the phrase is found in each extracted text content,
            // not just multiple times in one file.
            var splits = textExtractionResult.Text.Split(fileNames.ToArray(), StringSplitOptions.None);
            var foundCount = splits.Count(s => s.Contains("Use the force duke"));
            foundCount.Should().Be(fileNames.Count);
        }

        [Test]
        public void should_extract_contained_filenames_from_zips()
        {
            var textExtractionResult = _textExtractor.Extract("files/tika.zip");

            textExtractionResult.Text.Should().Contain("Tika.docx");
            textExtractionResult.Text.Should().Contain("Tika.pptx");
            textExtractionResult.Text.Should().Contain("tika.xlsx");
        }

        [Test]
        public void should_extract_from_doc()
        {
            var textExtractionResult = _textExtractor.Extract("files/Tika.doc");

            textExtractionResult.Text.Should().Contain("formatted in interesting ways");
        }

        [Test]
        public void should_extract_from_docx()
        {
            var textExtractionResult = _textExtractor.Extract("files/Tika.docx");

            textExtractionResult.Text.Should().Contain("formatted in interesting ways");
        }

        [Test]
        public void should_extract_from_jpg()
        {
            var textExtractionResult = _textExtractor.Extract("files/apache.jpg");

            textExtractionResult.Text.Trim().Should().BeEmpty();

            textExtractionResult.Metadata["Software"].Should().Contain("Paint.NET");
        }

        [Test]
        public void should_extract_from_pdf()
        {
            var textExtractionResult = _textExtractor.Extract("files/Tika.pdf");

            textExtractionResult.Text.Should().Contain("pack of pickled almonds");
        }

        [Test]
        public void should_extract_from_ppt()
        {
            var textExtractionResult = _textExtractor.Extract("files/Tika.ppt");

            textExtractionResult.Text.Should().Contain("This document is used for testing");
        }

        [Test]
        public void should_extract_from_pptx()
        {
            var textExtractionResult = _textExtractor.Extract("files/Tika.pptx");

            textExtractionResult.Text.Should().Contain("Tika Test Presentation");
        }

        [Test]
        public void should_extract_from_rtf()
        {
            var textExtractionResult = _textExtractor.Extract("files/Tika.rtf");

            textExtractionResult.Text.Should().Contain("pack of pickled almonds");
        }

        [Test]
        public void should_extract_from_uri()
        {
            const string url = "http://google.com/";
            var textExtractionResult = _textExtractor.Extract(new Uri(url));

            textExtractionResult.Text.Should().Contain("Google");
            textExtractionResult.Metadata["Uri"].Should().Be(url);
        }

        [Test]
        public void should_extract_from_xls()
        {
            var textExtractionResult = _textExtractor.Extract("files/Tika.xls");

            textExtractionResult.Text.Should().Contain("Use the force duke");
        }

        [Test]
        public void should_extract_from_xls_with_byte()
        {
            var data = File.ReadAllBytes("files/Tika.xls");
            var textExtractionResult = _textExtractor.Extract(data);

            textExtractionResult.Text.Should().Contain("Use the force duke");
        }

        [Test]
        public void should_extract_from_xlsx()
        {
            var textExtractionResult = _textExtractor.Extract("files/Tika.xlsx");

            textExtractionResult.Text.Should().Contain("Use the force duke");
        }

        [Test]
        public void should_extract_mp4()
        {
            var textExtractionResult = _textExtractor.Extract("files/badgers.mp4");

            textExtractionResult.ContentType.Should().Be("video/mp4");
        }

        [Test]
        public void should_extract_msg()
        {
            var textExtractionResult = _textExtractor.Extract("files/test.msg");
            
            textExtractionResult.Text.Should().Contain("This is my test file");
        }

        #region OCR test
        [Test]
        public void should_ocr_tif_with_tesseract()
        {
            if (!File.Exists(Path.Combine(TesseractPath, "tesseract.exe")))
                Assert.Ignore("Tesseract not found in the path '" + TesseractPath + "'");
            else
            {
                var textExtractionResult = _textExtractor.Extract("files/testfile.tif");
                textExtractionResult.Text.Should().Contain("This is some example text that should be ocred by tesseract");
            }
        }

        [Test]
        public void should_ocr_jpg_with_tesseract()
        {
            if (!File.Exists(Path.Combine(TesseractPath, "tesseract.exe")))
                Assert.Ignore("Tesseract not found in the path '" + TesseractPath + "'");
            else
            {
                var textExtractionResult = _textExtractor.Extract("files/testfile.png");
                textExtractionResult.Text.Should().Contain("This is some example text that should be ocred by tesseract");
            }
        }

        [Test]
        public void should_ocr_png_with_tesseract()
        {
            if (!File.Exists(Path.Combine(TesseractPath, "tesseract.exe")))
                Assert.Ignore("Tesseract not found in the path '" + TesseractPath + "'");
            else
            {
                var textExtractionResult = _textExtractor.Extract("files/testfile.png");
                textExtractionResult.Text.Should().Contain("This is some example text that should be ocred by tesseract");
            }
        }

        [Test]
        public void should_ocr_msg_with_tif_with_tesseract()
        {
            if (!File.Exists(Path.Combine(TesseractPath, "tesseract.exe")))
                Assert.Ignore("Tesseract not found in the path '" + TesseractPath + "'");
            else
            {
                var textExtractionResult = _textExtractor.Extract("files/withtifattachment.msg");
                textExtractionResult.Text.Should().Contain("This is some example text that should be ocred by tesseract");
            }
        }


        [Test]
        public void should_extract_from_scanned_pdf()
        {
            if (!File.Exists(Path.Combine(TesseractPath, "tesseract.exe")))
                Assert.Ignore("Tesseract not found in the path '" + TesseractPath + "'");
            else
            {
                _textExtractorOCR.ExtractFromScannedPdfs = true;
                var textExtractionResult = _textExtractor.Extract("files/scanned.pdf");
                textExtractionResult.Text.Should().Contain("This is some example text that should be ocred by tesseract");
            }
        }
        #endregion
    }
}