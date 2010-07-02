using System;
using System.Collections.Generic;
using System.Linq;
using java.io;
using javax.xml.transform;
using javax.xml.transform.sax;
using javax.xml.transform.stream;
using org.apache.tika.metadata;
using org.apache.tika.parser;

namespace TikaOnDotNet
	{
		public class TextExtractionResult
		{
			public string Text { get; set; }
			public string ContentType { get; set; }
			public IDictionary<string, string> Metadata { get; set; }
		}

		public class TextExtractor 
		{
			private StringWriter _outputWriter;

			public TextExtractionResult Extract(string filePath)
			{
				var parser = new AutoDetectParser();
				var metadata = new Metadata();
				var parseContext = new ParseContext();
				java.lang.Class parserClass = parser.GetType();
				parseContext.set(parserClass, parser);

				try
				{
					var file = new File(filePath);
					var url = file.toURI().toURL();
					using (var inputStream = MetadataHelper.getInputStream(url, metadata))
					{
						parser.parse(inputStream, getTransformerHandler(), metadata, parseContext);
						inputStream.close();
					}

					return assembleExtractionResult(_outputWriter.toString(), metadata);
				}
				catch (Exception ex)
				{
					throw new ApplicationException("Extraction of text from the file '{0}' failed.".ToFormat(filePath), ex);
				}
			}

			private static TextExtractionResult assembleExtractionResult(string text, Metadata metadata)
			{
				var metaDataResult = metadata.names().ToDictionary(name => name, name => String.Join(", ", metadata.getValues(name)));

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
				var factory = TransformerFactory.newInstance() as SAXTransformerFactory;
				var handler = factory.newTransformerHandler();
				handler.getTransformer().setOutputProperty(OutputKeys.METHOD, "text");
				handler.getTransformer().setOutputProperty(OutputKeys.INDENT, "yes");

				_outputWriter = new StringWriter();
				handler.setResult(new StreamResult(_outputWriter));
				return handler;
			}
		}
	}

