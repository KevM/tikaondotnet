using System;
using java.io;
using org.apache.tika;
using org.apache.tika.config;
using org.apache.tika.detect;
using org.apache.tika.metadata;
using org.apache.tika.mime;
using org.apache.tika.parser;
using org.apache.tika.parser.ocr;
using org.apache.tika.parser.pdf;
using Exception = System.Exception;
using Object = java.lang.Object;
using Parser = org.apache.tika.parser.Parser;
using StringBuilder = System.Text.StringBuilder;

namespace TikaOnDotNet.TextExtraction
{
    /// <summary>
    ///     With this class text can be extracted from all different kind of documents with
    ///     the use of Tika and Tesseract
    /// </summary>
    public class TextExtractorOCR : TextExtractor
    {
        #region Fields
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
        ///     Extract the text from the given inputstreams and returns it as an <see cref="TextExtractionResult" /> object
        /// </summary>
        /// <param name="streamFactory"></param>
        /// <returns></returns>
        /// <exception cref="TextExtractionException"></exception>
        public new TextExtractionResult Extract(Func<Metadata, InputStream> streamFactory)
        {
            try
            {
                var parser = new AutoDetectParser();
                var metadata = new Metadata();
                //var outputWriter = new StringWriter();
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
                        parseContext.set(typeof (PDFParserConfig), pdfParserConfig);
                    }
                }

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

        #region TikaConfigDump
        /// <summary>
        /// Dumps the Tika configuration and retuns it as an string
        /// </summary>
        /// <returns></returns>
        public static string TikaConfigDump()
        {
            var retVal = new StringBuilder();
            var tikaConfig = TikaConfig.getDefaultConfig();

            retVal.AppendFormat("{0}\t{1}\n", "Version", (new Tika(tikaConfig)).toString());

            retVal.AppendLine("\nDetectors");

            var configDetector = (CompositeDetector)tikaConfig.getDetector();
            var detectors = configDetector.getDetectors().toArray();
            foreach (Detector detector in detectors)
            {
                retVal.AppendFormat("\t{0}\n", ((Object) detector).getClass().getName());

                if (detector.GetType() == typeof (CompositeDetector))
                {
                    var subDetectors = configDetector.getDetectors().toArray();
                    foreach (Detector subDetector in subDetectors)
                    {
                        retVal.AppendFormat("\t\t{0}\n", ((Object) subDetector).getClass().getName());
                    }
                }
            }

            retVal.AppendLine("\nParsers");

            var configParser = (CompositeParser)tikaConfig.getParser();
            var parsers = configParser.getAllComponentParsers().toArray();
            foreach (Parser parser in parsers)
            {
                retVal.AppendFormat("\t{0}\n", ((Object) parser).getClass().getName());

                var parserTypes = parser.getSupportedTypes(new ParseContext()).toArray();
                foreach (MediaType mediaType in parserTypes)
                {
                    retVal.AppendFormat("\t\t{0}\n", mediaType.toString());
                }
            }

            var translator = tikaConfig.getTranslator();
            if (translator.isAvailable())
            {
                retVal.AppendFormat("Translator {0}\n", ((Object) translator).getClass().getName());
            }

            retVal.AppendFormat("\nFallback Parser: {0}\n", configParser.getFallback());

            return retVal.ToString();
        }
        #endregion
    }
}