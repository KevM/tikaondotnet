using System.Linq;
using java.io;
using javax.xml.transform;
using javax.xml.transform.sax;
using javax.xml.transform.stream;
using org.apache.tika.metadata;

namespace TikaOnDotNet.TextExtraction
{
    static class Helpers
    {
        #region AssembleExtractionResult
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

        #region GetTransformerHandler
        internal static TransformerHandler GetTransformerHandler(Writer output)
        {
            var factory = (SAXTransformerFactory)TransformerFactory.newInstance();
            var transformerHandler = factory.newTransformerHandler();

            transformerHandler.getTransformer().setOutputProperty(OutputKeys.METHOD, "text");
            transformerHandler.getTransformer().setOutputProperty(OutputKeys.INDENT, "yes");

            transformerHandler.setResult(new StreamResult(output));
            return transformerHandler;
        }
        #endregion
    }
}
