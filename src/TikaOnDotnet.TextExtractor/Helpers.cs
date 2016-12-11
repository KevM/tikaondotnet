using System.Linq;
using org.apache.tika.metadata;

namespace TikaOnDotNet.TextExtraction
{
    static class Helpers
    {
        #region AssembleExtractionResult
        /// <summary>
        /// Takes the extracted <see cref="text"/> and <see cref="metadata"/> and returns it as
        /// an <see cref="TextExtractionResult"/> object
        /// </summary>
        /// <param name="text"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        internal static TextExtractionResult AssembleExtractionResult(string text, Metadata metadata)
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
        #endregion
    }
}
