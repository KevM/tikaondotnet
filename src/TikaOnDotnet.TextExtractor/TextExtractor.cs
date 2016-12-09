using System;
using System.IO;
using System.Text;
using java.io;
using java.net;
using org.apache.tika;
using org.apache.tika.config;
using org.apache.tika.detect;
using org.apache.tika.io;
using org.apache.tika.metadata;
using org.apache.tika.mime;
using org.apache.tika.parser;
using org.apache.tika.parser.ocr;
using Object = java.lang.Object;
using StringWriter = java.io.StringWriter;

namespace TikaOnDotNet.TextExtraction
{
    /// <summary>
    ///     With this class text can be extracted from all diferent kind of documents with
    ///     the use of Tika
    /// </summary>
    public class TextExtractor : ITextExtractor
    {
        #region Fields
        private static readonly TikaConfig Config = TikaConfig.getDefaultConfig();
        private TesseractOCRConfig _tesseractOcrConfig;
        private static string _tesseractPath = string.Empty;
        #endregion

        #region Properties
        public string TesseractPath
        {
            get { return _tesseractPath; }
            set
            {
                _tesseractPath = value;
                _tesseractOcrConfig = new TesseractOCRConfig();
                //todo: validate directory and tesseract.exe at location
                _tesseractOcrConfig.setTesseractPath(_tesseractPath);
            }
        }

        public bool IsOcrPathEnabled
        {
            get { return _tesseractOcrConfig != null; }
            set
            {
                if (value)
                {
                    _tesseractOcrConfig = new TesseractOCRConfig();
                    _tesseractOcrConfig.setTesseractPath(_tesseractPath);
                }
                else
                {
                    _tesseractOcrConfig = null;
                }
            }
        }
        #endregion

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

        public TextExtractionResult Extract(byte[] data, string filePath, string contentType)
        {
            var result = Extract
                (
                    metadata =>
                    {
                        metadata.add(TikaMetadataKeys.__Fields.RESOURCE_NAME_KEY, Path.GetFileName(filePath));
                        metadata.add(TikaMimeKeys.__Fields.TIKA_MIME_FILE, filePath);

                        if (!contentType.Equals(MimeTypes.OCTET_STREAM, StringComparison.CurrentCultureIgnoreCase))
                        {
                            metadata.add(HttpHeaders.__Fields.CONTENT_TYPE, contentType);
                        }
                        else
                        {
                            var detector = Config.getDetector();
                            using (var inputStream = TikaInputStream.@get(data, metadata))
                            {
                                var foundType = detector.detect(inputStream, metadata);
                                if (
                                    !foundType.toString()
                                        .Equals(MimeTypes.OCTET_STREAM, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    metadata.add(HttpHeaders.__Fields.CONTENT_TYPE, foundType.toString());
                                }
                            }
                        }

                        return TikaInputStream.get(data, metadata);
                    }
                );

            return result;
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

                if (IsOcrPathEnabled)
                    parseContext.set(typeof(TesseractOCRConfig), _tesseractOcrConfig);

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

        #region TikaConfigDump
        /// <summary>
        /// Dumps the Tika configuration and retuns it as an string
        /// </summary>
        /// <returns></returns>
        public static string TikaConfigDump()
        {
            var retVal = new StringBuilder();

            retVal.AppendFormat("{0}\t{1}\n", "Version", (new Tika(Config)).toString());

            retVal.AppendLine("\nDetectors");

            var configDetector = (CompositeDetector)Config.getDetector();
            var detectors = configDetector.getDetectors().toArray();
            foreach (Detector detector in detectors)
            {
                retVal.AppendFormat("\t{0}\n", ((Object)detector).getClass().getName());

                if (detector.GetType() == typeof(CompositeDetector))
                {
                    var subDetectors = configDetector.getDetectors().toArray();
                    foreach (Detector subDetector in subDetectors)
                    {
                        retVal.AppendFormat("\t\t{0}\n", ((Object)subDetector).getClass().getName());
                    }
                }
            }

            retVal.AppendLine("\nParsers");

            var configParser = (CompositeParser)Config.getParser();
            var parsers = configParser.getAllComponentParsers().toArray();
            foreach (Parser parser in parsers)
            {
                retVal.AppendFormat("\t{0}\n", ((Object)parser).getClass().getName());

                var parserTypes = parser.getSupportedTypes(new ParseContext()).toArray();
                foreach (MediaType mediaType in parserTypes)
                {
                    retVal.AppendFormat("\t\t{0}\n", mediaType.toString());
                }
            }

            var translator = Config.getTranslator();
            if (translator.isAvailable())
            {
                retVal.AppendFormat("Translator {0}\n", ((Object)translator).getClass().getName());
            }

            retVal.AppendFormat("\nFallback Parser: {0}\n", configParser.getFallback());

            return retVal.ToString();
        }
        #endregion
    }
}