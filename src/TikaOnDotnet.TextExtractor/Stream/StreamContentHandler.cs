using System.IO;
using org.xml.sax;

namespace TikaOnDotNet.TextExtraction.Stream
{
    /// <summary>
    /// Write Tika output to a string builder while squelching the dreadful empty lines.
    /// NOTE: This type is only public for testing.
    /// </summary>
    public class StreamContentHandler : ContentHandler
    {
        private readonly StreamWriter _writer;
        private readonly System.IO.Stream _outputStream;

        public StreamContentHandler(System.IO.Stream outputStream)
        {
            _outputStream = outputStream;
            _writer = new StreamWriter(_outputStream);
        }

        public void setDocumentLocator(Locator locator)
        {
        }

        public void startDocument()
        {
        }

        public void endDocument()
        {
            _writer.Flush();
            _outputStream.Position = 0;
        }

        public void startPrefixMapping(string prefix, string uri)
        {
        }

        public void endPrefixMapping(string prefix)
        {
        }

        public void startElement(string uri, string localName, string qName, Attributes atts)
        {
        }

        public void endElement(string uri, string localName, string qName)
        {
        }

        public void characters(char[] ch, int start, int length)
        {
            if (length < 1) return;

            _writer.WriteLine(ch);
        }

        public void ignorableWhitespace(char[] ch, int start, int length)
        {
            // Ignore all the dreadful blank lines
        }

        public void processingInstruction(string target, string data)
        {
        }

        public void skippedEntity(string name)
        {
        }
    }
}