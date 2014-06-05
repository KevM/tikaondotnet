using System;
using System.Collections.Generic;
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
using StringBuilder = System.Text.StringBuilder;

namespace TikaOnDotNet
{
	public class TextExtractionResult
	{
		public string Text { get; set; }
		public string ContentType { get; set; }
		public IDictionary<string, string> Metadata { get; set; }

		public override string ToString()
		{
			var builder = new StringBuilder("Text:\n" + Text + "MetaData:\n");

			foreach (var keypair in Metadata)
			{
				builder.AppendFormat("{0} - {1}\n", keypair.Key, keypair.Value);
			}

			return builder.ToString();
		}
	}

	public class TextExtractor
	{
		private StringWriter _outputWriter;

		public TextExtractionResult Extract(string filePath)
		{
			try
			{
				var file = new File(filePath);
				return Extract(metadata => TikaInputStream.get(file, metadata));
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Extraction of text from the file '{0}' failed.".ToFormat(filePath), ex);
			}
		}

		public TextExtractionResult Extract(byte[] data)
		{
			return Extract(metadata => TikaInputStream.get(data, metadata));
		}

		public TextExtractionResult Extract(Uri uri)
		{
			var jUri = new java.net.URI(uri.ToString());
			return Extract(metadata => TikaInputStream.get(jUri, metadata));
		}

		public TextExtractionResult Extract(Func<Metadata, InputStream> streamFactory)
		{
			try
			{
				var parser = new AutoDetectParser();
				var metadata = new Metadata();
				var parseContext = new ParseContext();
				Class parserClass = parser.GetType();
				parseContext.set(parserClass, parser);

				using (var inputStream = streamFactory(metadata))
				{
					parser.parse(inputStream, getTransformerHandler(), metadata, parseContext);
					inputStream.close();
				}

				return assembleExtractionResult(_outputWriter.ToString(), metadata);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Extraction failed.", ex);
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

		private TransformerHandler getTransformerHandler()
		{
			var factory = (SAXTransformerFactory) TransformerFactory.newInstance();
			var transformerHandler = factory.newTransformerHandler();
			
			transformerHandler.getTransformer().setOutputProperty(OutputKeys.METHOD, "text");
			transformerHandler.getTransformer().setOutputProperty(OutputKeys.INDENT, "yes");

			_outputWriter = new StringWriter();
			transformerHandler.setResult(new StreamResult(_outputWriter));
			return transformerHandler;
		}
	}
}