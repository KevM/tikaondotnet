using System;
using java.io;
using org.apache.tika.io;
using org.apache.tika.metadata;
using org.apache.tika.parser;
using Exception = System.Exception;
using Parser = org.apache.tika.parser.Parser;
using URI = java.net.URI;

namespace TikaOnDotNet.TextExtraction
{
    /// <summary>
    ///     With this class text can be extracted from all different kind of documents with
    ///     the use of Tika
    /// </summary>
    public class TextExtractor : ITextExtractor
    {
        #region Extract
        /// <summary>
        ///     Extract the text from the given file and returns it as an <see cref="TextExtractionResult" /> object
        /// </summary>
        /// <param name="filePath">The file with its full path</param>
        /// <returns></returns>
        /// <exception cref="TextExtractionException"></exception>
        public TextExtractionResult Extract(string filePath)
        {
            try
            {
                var inputStream = new FileInputStream(filePath);
                return Extract(metadata =>
                {
                    var result = TikaInputStream.get(inputStream);
                    metadata.add("FilePath", filePath);
                    return result;
                });
            }
            catch (Exception ex)
            {
                throw new TextExtractionException("Extraction of text from the file '{0}' failed.".ToFormat(filePath),
                    ex);
            }
        }

        /// <summary>
        ///     Extract the text from the given byte array and returns it as an <see cref="TextExtractionResult" /> object
        /// </summary>
        /// <param name="data">The byte array</param>
        /// <returns></returns>
        /// <exception cref="TextExtractionException"></exception>
        public TextExtractionResult Extract(byte[] data)
        {
            return Extract(metadata => TikaInputStream.get(data, metadata));
        }

        /// <summary>
        ///     Extract from the give uri and returns it as an <see cref="TextExtractionResult" /> object
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="TextExtractionException"></exception>
        public TextExtractionResult Extract(Uri uri)
        {
            var jUri = new URI(uri.ToString());
            return Extract(metadata =>
            {
                var result = TikaInputStream.get(jUri, metadata);
                metadata.add("Uri", uri.ToString());
                return result;
            });
        }

        /// <summary>
        ///     Extract the text from the given inputstreams and returns it as an <see cref="TextExtractionResult" /> object
        /// </summary>
        /// <param name="streamFactory"></param>
        /// <returns></returns>
        /// <exception cref="TextExtractionException"></exception>
        public TextExtractionResult Extract(Func<Metadata, InputStream> streamFactory)
        {
            try
            {
                var parser = new AutoDetectParser();
                var metadata = new Metadata();
                var parseContext = new ParseContext();

                // Use the base class type for the key or parts of Tika won't find a usable parser
                parseContext.set(typeof (Parser), parser);

                var content = new System.IO.StringWriter();
                var customTextContentHandler = new CustomTextContentHandler(content);

                using (var inputStream = streamFactory(metadata))
                {
                    try
                    {
                        parser.parse(inputStream, customTextContentHandler, metadata, parseContext);
                    }
                    finally
                    {
                        inputStream.close();
                    }
                }

                return Helpers.AssembleExtractionResult(content.ToString(), metadata);
            }
            catch (Exception ex)
            {
                throw new TextExtractionException("Extraction failed.", ex);
            }
        }
        #endregion
    }
}