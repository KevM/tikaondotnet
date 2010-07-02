using NUnit.Framework;
using SpecUnit;

namespace TikaOnDotNet
{
	[TestFixture]
	public class tikadriver_examples
	{
		private TextExtractor _cut;

		[SetUp]
		public virtual void SetUp()
		{
			_cut = new TextExtractor();
		}

		[Test]
		public void should_extract_from_pdf()
		{
			var textExtractionResult = _cut.Extract("Tika.pdf");

			textExtractionResult.Text.ShouldContain("pack of pickled almonds");
		}

		[Test]
		public void should_extract_from_docx()
		{
			var textExtractionResult = _cut.Extract("Tika.docx");

			textExtractionResult.Text.ShouldContain("formatted in interesting ways");
		}

		[Test]
		public void should_extract_from_pptx()
		{
			var textExtractionResult = _cut.Extract("Tika.pptx");

			textExtractionResult.Text.ShouldContain("Tika Test Presentation");
		}

		[Test]
		public void should_extract_from_xlsx()
		{
			var textExtractionResult = _cut.Extract("Tika.xlsx");

			textExtractionResult.Text.ShouldContain("Use the force duke");
		}

		[Test]
		public void should_extract_from_doc()
		{
			var textExtractionResult = _cut.Extract("Tika.doc");

			textExtractionResult.Text.ShouldContain("formatted in interesting ways");
		}

		[Test]
		public void should_extract_from_ppt()
		{
			var textExtractionResult = _cut.Extract("Tika.ppt");

			textExtractionResult.Text.ShouldContain("This document is used for testing");
		}

		[Test]
		public void should_extract_from_xls()
		{
			var textExtractionResult = _cut.Extract("Tika.xls");

			textExtractionResult.Text.ShouldContain("Use the force duke");
		}
	}
}