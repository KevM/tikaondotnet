using System;
using System.Linq;
using java.io;
using javax.xml.transform;
using javax.xml.transform.sax;
using javax.xml.transform.stream;
using org.apache.tika.io;
using org.apache.tika.metadata;
using org.apache.tika.parser;
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
			try
			{
				var parser = new AutoDetectParser();
				var metadata = new Metadata();
				var outputWriter = new StringWriter();
				var parseContext = new ParseContext();

                //use the base class type for the key or parts of Tika won't find a usable parser
				parseContext.set(typeof(Parser), parser);
				
				using (var inputStream = streamFactory(metadata))
				{
					try
					{
						parser.parse(inputStream, getTransformerHandler(outputWriter), metadata, parseContext);
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

		private static TransformerHandler getTransformerHandler(Writer output)
		{
			var factory = (SAXTransformerFactory) TransformerFactory.newInstance();
			var transformerHandler = factory.newTransformerHandler();
			
			transformerHandler.getTransformer().setOutputProperty(OutputKeys.METHOD, "text");
			transformerHandler.getTransformer().setOutputProperty(OutputKeys.INDENT, "yes");

			transformerHandler.setResult(new StreamResult(output));
			return transformerHandler;
		}
	}
}