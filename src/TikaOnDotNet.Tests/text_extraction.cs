using System;
using FubuTestingSupport;
using NUnit.Framework;

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
		public void should_extract_contained_filenames_from_zips()
		{
			var textExtractionResult = _cut.Extract("files/tika.zip");

			textExtractionResult.Text.ShouldContain("Tika.docx");
			textExtractionResult.Text.ShouldContain("Tika.pptx");
			textExtractionResult.Text.ShouldContain("tika.xlsx");
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
	}
}