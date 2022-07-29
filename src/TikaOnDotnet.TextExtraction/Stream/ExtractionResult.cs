using org.apache.tika.metadata;

namespace TikaOnDotNet.TextExtraction.Stream
{
    public class ExtractionResult
    {
        /// <summary>
        /// MIME Content-Type of the data from which the text was extracted.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Dictionary of meta data (e.g. titie, size, dimensions) about the data source of the extraction.
        /// </summary>
        public Metadata Metadata { get; set; }
    }
}
