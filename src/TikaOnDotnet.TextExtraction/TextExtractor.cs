using System;
using System.IO;
using System.Linq;
using System.Net;
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
            return Extract(filePath, LegacyResultAssembler);
        }

        public TExtractionResult Extract<TExtractionResult>(
            string filePath,
            Func<string, Metadata, TExtractionResult> extractionResultAssembler
            )
        {
            try
            {
                return Extract(FileStreamFactory, extractionResultAssembler);
            }
            catch (Exception ex)
            {
                throw new TextExtractionException("Extraction of text from the file '{0}' failed.".ToFormat(filePath), ex);
            }

            InputStream FileStreamFactory(Metadata metadata)
            {
                var inputFile = new java.io.File(filePath);
                var result = TikaInputStream.get(inputFile.toPath());
                metadata.add("FilePath", filePath);
                return result;
            }
        }

        public TextExtractionResult Extract(byte[] data)
        {
            return Extract(data, LegacyResultAssembler);
        }

        public TExtractionResult Extract<TExtractionResult>(byte[] data, Func<string, Metadata, TExtractionResult> extractionResultAssembler)
        {
            return Extract(metadata => TikaInputStream.get(data, metadata), extractionResultAssembler);
        }

        public TextExtractionResult Extract(Uri uri)
        {
            return Extract(uri, LegacyResultAssembler);
        }

        public TExtractionResult Extract<TExtractionResult>(
            Uri uri,
            Func<string, Metadata, TExtractionResult> extractionResultAssembler
        )
        {
            return Extract(UrlStreamFactory, extractionResultAssembler);

            InputStream UrlStreamFactory(Metadata metadata)
            {
                metadata.add("Uri", uri.ToString());

                var pageBytes = new WebClient().DownloadData(uri);

                return TikaInputStream.get(pageBytes, metadata);
            }
        }

        public TextExtractionResult Extract(Func<Metadata, InputStream> streamFactory)
        {
            return Extract(streamFactory, LegacyResultAssembler);
        }

        public TExtractionResult Extract<TExtractionResult>(Func<Metadata, InputStream> streamFactory, Func<string, Metadata, TExtractionResult> extractionResultAssembler)
        {
            var streamExtractor = new StreamTextExtractor();
            using (var outputStream = new MemoryStream())
            {
                var metadata = streamExtractor.Extract(streamFactory, outputStream);
                outputStream.Position = 0;

                using (var reader = new StreamReader(outputStream))
                {
                    var text = reader.ReadToEnd();
                    return extractionResultAssembler(text, metadata);
                }
            }
        }

        private static TextExtractionResult LegacyResultAssembler(string text, Metadata metadata)
        {
            var metaDataDictionary = metadata.names()
                .ToDictionary(name => name, name => string.Join(", ", metadata.getValues(name)));
            var contentType = metaDataDictionary["Content-Type"];

            return new TextExtractionResult
            {
                Metadata = metaDataDictionary,
                Text = text,
                ContentType = contentType
            };
        }
    }
}
