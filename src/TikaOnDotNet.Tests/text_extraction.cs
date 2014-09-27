using System;
using System.IO;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

namespace TikaOnDotNet.Tests
{
	[TestFixture]
	public class text_extraction
	{
		private TextExtractor _cut;

		[SetUp]
		public virtual void SetUp()
		{
			_cut = new TextExtractor();
		}

		[Test]
		public void non_existing_files_should_fail_with_exception()
		{
			const string fileName = "files/doesnotexist.mp3";

			typeof(TextExtractionException).ShouldBeThrownBy(() => _cut.Extract(fileName))
				.Message.ShouldContain(fileName);
		}

		[Test]
		public void non_existing_uri_should_fail_with_exception()
		{
			const string uri = "http://example.com/does/not/really/exist/mp3/repo/zzzarble.mp3";

			typeof(TextExtractionException).ShouldBeThrownBy(() => _cut.Extract(new Uri(uri)));
		}

		[Test]
		public void should_extract_mp4()
		{
			var textExtractionResult = _cut.Extract("files/badgers.mp4");

			textExtractionResult.ContentType.ShouldEqual("video/mp4");
		}

		[Test, Explicit("Issue #11")]
		public void should_be_able_to_delete_the_mp4_after_extraction()
		{
			_cut.Extract("files/badgers.mp4");

			var fileInfo = new FileInfo(@"C:\projects\tikaondotnet\src\TikaOnDotNet.Tests\bin\Debug\files\badgers.mp4");
			fileInfo.Delete();
			fileInfo.Exists.ShouldBeFalse();
		}


		[Test]
		public void extract_by_filepath_should_add_filepath_to_metadata()
		{
			const string filePath = "files/apache.jpg";

			var textExtractionResult = _cut.Extract(filePath);

			textExtractionResult.Metadata["FilePath"].ShouldEqual(filePath);
		}

		[Test]
		public void should_extract_contained_filenames_from_zips()
		{
			var textExtractionResult = _cut.Extract("files/tika.zip");

			textExtractionResult.Text.ShouldContain("Tika.docx");
			textExtractionResult.Text.ShouldContain("Tika.pptx");
			textExtractionResult.Text.ShouldContain("tika.xlsx");
		}

        [Test]
        public void should_extract_contained_filenames_and_text_from_zips()
        {
            var textExtractionResult = _cut.Extract("files/tika.zip");

            List<string> fileNames = new List<string>(new string [] { "Tika.docx", "Tika.pptx", "tika.xlsx" });

            //verify all expected files are there
            fileNames.ForEach(name => textExtractionResult.Text.ShouldContain(name));

            //we should find the same string once for every file in the zip. if we split the string on file names
            // this should break out the content into different strings to confirm the phrase is found in each extracted text content,
            // not just multiple times in one file.
            string[] splits = textExtractionResult.Text.Split(fileNames.ToArray(), StringSplitOptions.None);   
            var foundCount = splits.Where(s => s.Contains("Use the force duke")).Count();
            foundCount.ShouldEqual(fileNames.Count());
        }

		[Test]
		public void should_extract_from_jpg()
		{
			var textExtractionResult = _cut.Extract("files/apache.jpg");

			textExtractionResult.Text.Trim().ShouldBeEmpty();

			textExtractionResult.Metadata["Software"].ShouldContain("Paint.NET");
		}

		[Test]
		public void should_extract_from_rtf()
		{
			var textExtractionResult = _cut.Extract("files/Tika.rtf");

			textExtractionResult.Text.ShouldContain("pack of pickled almonds");
		}

		[Test]
		public void should_extract_from_pdf()
		{
			var textExtractionResult = _cut.Extract("files/Tika.pdf");

			textExtractionResult.Text.ShouldContain("pack of pickled almonds");
		}
		
		[Test]
		public void should_extract_from_docx()
		{
			var textExtractionResult = _cut.Extract("files/Tika.docx");

			textExtractionResult.Text.ShouldContain("formatted in interesting ways");
		}

		[Test]
		public void should_extract_from_pptx()
		{
			var textExtractionResult = _cut.Extract("files/Tika.pptx");

			textExtractionResult.Text.ShouldContain("Tika Test Presentation");
		}

		[Test]
		public void should_extract_from_xlsx()
		{
			var textExtractionResult = _cut.Extract("files/Tika.xlsx");

			textExtractionResult.Text.ShouldContain("Use the force duke");
		}

		[Test]
		public void should_extract_from_doc()
		{
			var textExtractionResult = _cut.Extract("files/Tika.doc");

			textExtractionResult.Text.ShouldContain("formatted in interesting ways");
		}

		[Test]
		public void should_extract_from_ppt()
		{
			var textExtractionResult = _cut.Extract("files/Tika.ppt");

			textExtractionResult.Text.ShouldContain("This document is used for testing");
		}

		[Test]
		public void should_extract_from_xls()
		{
			var textExtractionResult = _cut.Extract("files/Tika.xls");

			textExtractionResult.Text.ShouldContain("Use the force duke");
		}
		
		[Test]
		public void should_extract_from_xls_with_byte()
		{
			var data = File.ReadAllBytes("files/Tika.xls");
			var textExtractionResult = _cut.Extract(data);

			textExtractionResult.Text.ShouldContain("Use the force duke");
		}

		[Test]
		public void should_extract_from_uri()
		{
			const string url = "http://google.com/";
			var textExtractionResult = _cut.Extract(new Uri(url));
			
			textExtractionResult.Text.ShouldContain("Google");
			textExtractionResult.Metadata["Uri"].ShouldEqual(url);
		}
	}
}