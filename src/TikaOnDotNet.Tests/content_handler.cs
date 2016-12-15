using System.IO;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using org.xml.sax.ext;
using TikaOnDotNet.TextExtraction;

namespace TikaOnDotNet.Tests
{
    [TestFixture]
    public class content_handler
    {
        private TextExtractorContentHandler _cut;
        private StringBuilder _stringBuilder;
        private char[] _expectedCharacters;

        [SetUp]
        public void SetUp()
        {
            _expectedCharacters = new[] { 'a', 'b', 'c' };
            _stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(_stringBuilder);

            _cut = new TextExtractorContentHandler(stringWriter);
        }

        [Test]
        public void should_capture_character_content()
        {
            _cut.characters(_expectedCharacters, 0, _expectedCharacters.Length);

            _stringBuilder.Length.Should().Be(_expectedCharacters.Length + 2);
        }

        [Test]
        public void should_ignore_empty_content()
        {
            _cut.characters(new char[0], 0, 0);

            _stringBuilder.Length.Should().Be(0);
        }

        [Test]
        public void should_ignore_non_character_content()
        {
            _cut.startDocument();
            _cut.startElement("wee","local","name", new Attributes2Impl());
            _cut.characters(_expectedCharacters, 0, _expectedCharacters.Length);
            _cut.endElement("wee","local","name");
            _cut.endDocument();

            _stringBuilder.Length.Should().Be(_expectedCharacters.Length + 2);
        }
    }
}