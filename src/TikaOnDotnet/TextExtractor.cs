using System;
using System.Linq;
using java.io;
using java.lang;
using javax.xml.transform;
using javax.xml.transform.sax;
using javax.xml.transform.stream;
using org.apache.tika.io;
using org.apache.tika.metadata;
using org.apache.tika.parser;
using Exception = System.Exception;
using String = System.String;

namespace TikaOnDotNet
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

	public class TextExtractor : ITextExtractor
	{
		public TextExtractionResult Extract(string filePath)
		{
			try
			{
				var file = new File(filePath);
				return Extract(metadata =>
				{
					var result = TikaInputStream.get(file, metadata);
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
				Class parserClass = parser.GetType();
				parseContext.set(parserClass, parser);
				
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

				return assembleExtractionResult(outputWriter.ToString(), metadata);
			}
			catch (Exception ex)
			{
				throw new TextExtractionException("Extraction failed.", ex);
			}
		}

		private static TextExtractionResult assembleExtractionResult(string text, Metadata metadata)
		{
			var metaDataResult = metadata.names()
				.ToDictionary(name => name, name => String.Join(", ", metadata.getValues(name)));

			var contentType = metaDataResult["Content-Type"];

			return new TextExtractionResult
			{
				Text = text,
				ContentType = contentType,
				Metadata = metaDataResult
			};
		}

		private TransformerHandler getTransformerHandler(StringWriter output)
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