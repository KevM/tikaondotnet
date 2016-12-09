using System;

namespace TikaOnDotNet.TextExtraction
{
	public static class Extensions
	{
		public static string ToFormat(this string formatMe, params object[] args)
		{
			return String.Format(formatMe, args);
		}
	}
}