using System.Collections.Generic;
using System.Text;

namespace TikaOnDotNet.TextExtraction
{
	public class TextExtractionResult
	{
		/// <summary>
		/// Text extracted from the source data
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// MIME Content-Type of the data from which the text was extracted.
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// Dictionary of meta data (e.g. titie, size, dimensions) about the data source of the extraction.
		/// </summary>
		public IDictionary<string, string> Metadata { get; set; }

		/// <summary>
		/// Pretty print the output of the extraction
		/// </summary>
		/// <returns>Human readable version of the text extraction result</returns>
		public override string ToString()
		{
			var builder = new StringBuilder("Text:\n" + Text + "MetaData:\n");

			foreach (var keypair in Metadata)
				builder.AppendFormat("{0} - {1}\n", keypair.Key, keypair.Value);

			return builder.ToString();
		}
    }
}