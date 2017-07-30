using System; 
using FluentAssertions;
using NUnit.Framework;
using TikaOnDotNet.TextExtraction;

namespace TikaOnDotNet.Tests
{
    [TestFixture]
    public class sad_text_extraction
    {
        private TextExtractor _cut;

        [SetUp]
        public virtual void SetUp()
        {
            _cut = new TextExtractor();
        }

        [Test]
        public void issue_81_file_1()
        {
            var filePath = "sad-files/EI-73-1018-2_5632837.doc";

            Action act = () => _cut.Extract(filePath);

            act.ShouldThrow<TextExtractionException>();
        }

        [Test]
        public void issue_81_file_2()
        {
            var filePath = "sad-files/EI-73-1027-3_5632849.doc";

            Action act = () => _cut.Extract(filePath);

            act.ShouldThrow<TextExtractionException>();
        }
    }
}
