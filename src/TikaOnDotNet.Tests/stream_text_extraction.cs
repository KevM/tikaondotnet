using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using org.apache.tika.io;
using TikaOnDotNet.TextExtraction;
using TikaOnDotNet.TextExtraction.Stream;

namespace TikaOnDotNet.Tests
{
    [TestFixture]
    public class stream_text_extraction
    {
        private StreamTextExtractor _cut;

        [SetUp]
        public virtual void SetUp()
        {
            _cut = new StreamTextExtractor();
        }

        [Test]
        public void should_throw_when_given_closed_stream()
        {
            var closedStream = new MemoryStream();
            closedStream.Dispose();

            var bytes = new byte[] {0, 1, 2, 3};
            Action act = () => _cut.Extract(metadata=> TikaInputStream.get(bytes, metadata), closedStream);

            act.ShouldThrow<TextExtractionException>().WithInnerException<ArgumentException>();
        }
    }
}