using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using org.xml.sax.ext;
using TikaOnDotNet.TextExtraction.Stream;

namespace TikaOnDotNet.Tests
{
    [TestFixture]
    public class stream_content_handler
    {
        private StreamContentHandler _cut;
        private char[] _expectedCharacters;
        private MemoryStream _outputStream;

        [SetUp]
        public void SetUp()
        {
            _expectedCharacters = new[] { 'a', 'b', 'c' };
            _outputStream = new MemoryStream();
            _cut = new StreamContentHandler(_outputStream);
        }

        private string GetResult()
        {
            using (var reader = new StreamReader(_outputStream))
            {
                return reader.ReadToEnd();
            }
        }

        [Test]
        public void should_capture_character_content()
        {
            _cut.characters(_expectedCharacters, 0, _expectedCharacters.Length);
            _cut.endDocument();

            var result = GetResult();
            result.Length.Should().Be(_expectedCharacters.Length + 2);
            result.Take(_expectedCharacters.Length).Should().BeEquivalentTo(_expectedCharacters);
        }

        [Test]
        public void should_ignore_empty_content()
        {
            _cut.characters(new char[0], 0, 0);
            _cut.endDocument();

            GetResult().Length.Should().Be(0);
        }

        [Test]
        public void should_ignore_non_character_content()
        {
            _cut.startDocument();
            _cut.startElement("wee", "local", "name", new Attributes2Impl());
            _cut.characters(_expectedCharacters, 0, _expectedCharacters.Length);
            _cut.endElement("wee", "local", "name");
            _cut.endDocument();

            GetResult().Length.Should().Be(_expectedCharacters.Length + 2);
        }
    }
}