using System;

namespace TikaOnDotNet
{
	public static class StringExtrensions
	{
		public static string ToFormat(this string formatMe, params object[] args)
		{
			return String.Format(formatMe, args);
		}
	}
}