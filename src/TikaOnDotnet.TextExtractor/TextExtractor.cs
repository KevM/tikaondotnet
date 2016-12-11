using System;
using System.Linq;
using java.io;
using java.lang;
using java.net;
using javax.xml.transform;
using javax.xml.transform.sax;
using javax.xml.transform.stream;
using org.apache.tika;
using org.apache.tika.config;
using org.apache.tika.detect;
using org.apache.tika.io;
using org.apache.tika.metadata;
using org.apache.tika.mime;
using org.apache.tika.parser;
using org.apache.tika.parser.ocr;
using org.apache.tika.parser.pdf;
using org.apache.tika.sax;
using Exception = System.Exception;
using Object = java.lang.Object;
using String = System.String;
using StringBuilder = System.Text.StringBuilder;
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
        #endregion

        #region Properties
        /// <summary>
        /// Set the <see cref="TesseractOCRConfig"/> and enables extraction of text from
        /// images with the use of Tesseract
        /// </summary>
        /// <remarks>
        /// See https://tika.apache.org/1.14/api/org/apache/tika/parser/ocr/TesseractOCRParser.html
        /// </remarks>
        public TesseractOCRConfig TesseractOCRConfig
        {
            get { return _tesseractOcrConfig ?? (_tesseractOcrConfig = new TesseractOCRConfig()); }
            set { _tesseractOcrConfig = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to extract text from scanned PDF's with the use of Tesseract.
        /// When set to <c>false</c> (default) then images in PDF's are ignored
        /// </summary>
        /// <remarks>
        /// Tesseract has to be setup trough the <see cref="TesseractOCRConfig"/> property to
        /// make this work. When not set then this property is ignored
        /// </remarks>
        public bool ExtractFromScannedPdfs { get; set; }
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

                if (_tesseractOcrConfig != null)
                {
                    parseContext.set(typeof (TesseractOCRConfig), _tesseractOcrConfig);

                    if (ExtractFromScannedPdfs)
                    {
                        var pdfParserConfig = new PDFParserConfig();
                        pdfParserConfig.setExtractInlineImages(true);
                        // Set to false if pdf contains multiple images
                        pdfParserConfig.setExtractUniqueInlineImagesOnly(false);
                        parseContext.set(typeof(PDFParserConfig), pdfParserConfig);
                    }
                }

                //use the base class type for the key or parts of Tika won't find a usable parser
                parseContext.set(typeof (Parser), parser);

                using (var inputStream = streamFactory(metadata))
                {
                    try
                    {
                        parser.parse(inputStream, GetTransformerHandler(outputWriter), metadata, parseContext);
                    }
                    finally
                    {
                        inputStream.close();
                    }
                }

                return AssembleExtractionResult(outputWriter.ToString(), metadata);
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

        #region AssembleExtractionResult
        /// <summary>
        /// Takes the extracted <see cref="text"/> and <see cref="metadata"/> and returns it as
        /// an <see cref="TextExtractionResult"/> object
        /// </summary>
        /// <param name="text"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private static TextExtractionResult AssembleExtractionResult(string text, Metadata metadata)
        {
            var metaDataResult = metadata.names()
                .ToDictionary(name => name, name => string.Join(", ", metadata.getValues(name)));

            var contentType = metaDataResult["Content-Type"];

            return new TextExtractionResult
            {
                Text = text,
                ContentType = contentType,
                Metadata = metaDataResult
            };
        }
        #endregion

        #region GetTransformerHandler
        /// <summary>
        /// Sets the way the output of Tika is converted
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        internal static TransformerHandler GetTransformerHandler(Writer output)
        {
            var factory = (SAXTransformerFactory)TransformerFactory.newInstance();
            var transformerHandler = factory.newTransformerHandler();
            var transformer = transformerHandler.getTransformer();

            transformer.setOutputProperty(OutputKeys.METHOD, "text");
            transformer.setOutputProperty(OutputKeys.INDENT, "yes");
            transformer.setOutputProperty(OutputKeys.ENCODING, "UTF-8");

            transformerHandler.setResult(new StreamResult(output));
            return transformerHandler;
        }
        #endregion
    }
}