using System;
using java.io;
using org.apache.tika.metadata;

namespace TikaOnDotNet.TextExtraction
{
    public interface ITextExtractor
    {
        /// <summary>
        /// Extract text from a given filepath.
        /// </summary>
        /// <param name="filePath">File path to be extracted.</param>
        TextExtractionResult Extract(string filePath);
		
        /// <summary>
        /// Extract text from a byte[]. This is a good way to get data from arbitrary sources.
        /// </summary>
        /// <param name="data">A byte array of data which will have its text extracted.</param>
        TextExtractionResult Extract(byte[] data);

        /// <summary>
        /// Extract text from a URI. Time to create your very of web spider.
        /// </summary>
        /// <param name="uri">URL which will have its text extracted.</param>
        TextExtractionResult Extract(Uri uri);

        /// <summary>
        /// Under the hood we are using Tika which is a Java project. Tika wants an java.io.InputStream. The other overloads eventually call this Extract giving this method a Func.
        /// </summary>
        /// <param name="streamFactory">A Func which takes a Metadata object and returns an InputStream.</param>
        /// <returns></returns>
        TextExtractionResult Extract(Func<Metadata, InputStream> streamFactory);
    }
}