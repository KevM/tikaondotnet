using System.IO;
using org.xml.sax;

namespace TikaOnDotNet.TextExtraction
{
    /// <summary>
    /// A custom text handler that writes it output to a string builder and gets rid of all the dreadful
    /// empty lines that are added with the standard tike transformers
    /// </summary>
    internal class CustomTextContentHandler : ContentHandler
    {
        #region Fields
        private readonly StringWriter _contentWriter;
        #endregion

        /// <summary>
        /// Makes this object and sets all it's properties
        /// </summary>
        /// <param name="content">All the extracted text will be written to this StringWriter</param>
        public CustomTextContentHandler(StringWriter content)
        {
            _contentWriter = content;
        }

        #region Implementation of ContentHandler
        public void setDocumentLocator(Locator locator)
        {
            
        }

        public void startDocument()
        {
        }

        public void endDocument()
        {
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
            _contentWriter.WriteLine(ch);
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
        #endregion
    }
}