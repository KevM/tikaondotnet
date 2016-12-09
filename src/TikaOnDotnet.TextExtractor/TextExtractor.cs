using System;
using java.io;
using java.net;
using org.apache.tika.io;
using org.apache.tika.metadata;
using org.apache.tika.parser;

namespace TikaOnDotNet.TextExtraction
{
    /// <summary>
    ///     With this class text can be extracted from all diferent kind of documents with
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
                var outputWriter = new StringWriter();
                var parseContext = new ParseContext();

                //use the base class type for the key or parts of Tika won't find a usable parser
                parseContext.set(typeof (Parser), parser);

                using (var inputStream = streamFactory(metadata))
                {
                    try
                    {
                        parser.parse(inputStream, Helpers.GetTransformerHandler(outputWriter), metadata, parseContext);
                    }
                    finally
                    {
                        inputStream.close();
                    }
                }

                return Helpers.AssembleExtractionResult(outputWriter.ToString(), metadata);
            }
            catch (Exception ex)
            {
                throw new TextExtractionException("Extraction failed.", ex);
            }
        }
        #endregion
    }
}