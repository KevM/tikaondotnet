using System;
using System.IO;
using java.io;
using org.apache.tika.io;
using org.apache.tika.metadata;
using TikaOnDotNet.TextExtraction.Stream;
using Exception = System.Exception;

namespace TikaOnDotNet.TextExtraction
{
    public class TextExtractor : ITextExtractor
	{
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
				throw new TextExtractionException("Extraction of text from the file '{0}' failed.".ToFormat(filePath), ex);
			}
		}

		public TextExtractionResult Extract(byte[] data)
		{
			return Extract(metadata => TikaInputStream.get(data, metadata));
		}

		public TextExtractionResult Extract(Uri uri)
		{
			var jUri = new java.net.URI(uri.ToString());
			return Extract(metadata =>
			{
				var result = TikaInputStream.get(jUri, metadata);
				metadata.add("Uri", uri.ToString());
				return result;
			});
		}

		public TextExtractionResult Extract(Func<Metadata, InputStream> streamFactory)
		{
            var streamExtractor = new StreamTextExtractor();
		    using (var outputStream = new MemoryStream())
		    {
                var streamResult = streamExtractor.Extract(streamFactory, outputStream);

		        using (var reader = new StreamReader(outputStream))
		        {
                    return new TextExtractionResult
                    {
                        Text = reader.ReadToEnd(),
                        Metadata = streamResult.Metadata,
                        ContentType = streamResult.ContentType
                    };
                }
            }
        }
	}
}