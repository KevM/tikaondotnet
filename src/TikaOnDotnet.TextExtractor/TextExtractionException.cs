using System;

namespace TikaOnDotNet.TextExtraction
{
    /// <summary>
    /// Raised when an exception occurs with extracting text with Tika
    /// </summary>
	public class TextExtractionException : Exception
	{
		internal TextExtractionException(string message) : base(message)
		{
			
		}

		internal TextExtractionException(string message, Exception exception) : base(message, exception)
		{

		}
	}
}