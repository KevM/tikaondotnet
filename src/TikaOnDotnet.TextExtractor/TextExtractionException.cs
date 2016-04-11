using System;

namespace TikaOnDotNet.TextExtraction
{
	public class TextExtractionException : Exception
	{
		public TextExtractionException(string message) : base(message)
		{
			
		}

		public TextExtractionException(string message, Exception exception)
			: base(message, exception)
		{

		}
	}
}