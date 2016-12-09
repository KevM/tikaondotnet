using System;
using org.apache.tika.config;
using org.apache.tika.detect;
using org.apache.tika.io;
using org.apache.tika.mime;

namespace TikaOnDotNet.TextExtraction
{
	public static class Extensions
	{
		public static string ToFormat(this string formatMe, params object[] args)
		{
			return string.Format(formatMe, args);
		}

        private static readonly TikaConfig Config = TikaConfig.getDefaultConfig();
        public static TextExtractionResult Extract(this TextExtractor te, byte[] data, string filePath, string contentType)
        {
            var result = te.Extract
                (
                    metadata =>
                    {
                        metadata.add("resourceName", System.IO.Path.GetFileName(filePath));
                        metadata.add("FilePath", filePath);
                        if (!contentType.Equals("application/octet-stream", StringComparison.CurrentCultureIgnoreCase))
                            metadata.add("Content-Type", contentType);
                        else
                        {
                            var detector = Config.getDetector();
                            using (var inputStream = TikaInputStream.@get(data, metadata))
                            {
                                var foundType = detector.detect(inputStream, metadata);
                                if (
                                    !foundType.toString()
                                        .Equals("application/octet-stream", StringComparison.CurrentCultureIgnoreCase))
                                    metadata.add("Content-Type", foundType.toString());
                            }
                        }

                        return TikaInputStream.get(data, metadata);
                    }
                );

            return result;
        }

        public static TextExtractionResult Extract(this TextExtractor te, byte[] data, string filePath)
        {
            return te.Extract(data, filePath, "application/octet-stream");
        }
    }
}