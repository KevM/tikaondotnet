using System;
using java.io;
using org.apache.tika.metadata;

namespace TikaOnDotNet.TextExtraction
{
    public interface ITextExtractor
    {
        /// <summary>
        ///     Extract the text from the given file and returns it as an <see cref="TextExtractionResult" /> object
        /// </summary>
        /// <param name="filePath">The file with its full path</param>
        /// <exception cref="TextExtractionException"></exception>
        TextExtractionResult Extract(string filePath);

        /// <summary>
        ///     Extract the text from the given byte array and returns it as an <see cref="TextExtractionResult" /> object
        /// </summary>
        /// <param name="data">Bytes of content from which you'd like Tika to extract text.</param>
        /// <exception cref="TextExtractionException"></exception>
        TextExtractionResult Extract(byte[] data);

        /// <summary>
        ///     Extract from the give uri and returns it as an <see cref="TextExtractionResult" /> object
        /// </summary>
        /// <param name="uri">Url to download and </param>
        /// <exception cref="TextExtractionException"></exception>
        TextExtractionResult Extract(Uri uri);

        /// <summary>
        ///     Extract the text from the given inputstreams and returns it as an <see cref="TextExtractionResult" /> object. 
        ///     This is the lowest level overload which all the other overloads use under the hood.
        /// </summary>
        /// <param name="streamFactory">Function taking a <see cref="Metadata"/> and returning an <see cref="InputStream"/> of content you with extracted.</param>
        /// <exception cref="TextExtractionException"></exception>
        TextExtractionResult Extract(Func<Metadata, InputStream> streamFactory);
    }
}
