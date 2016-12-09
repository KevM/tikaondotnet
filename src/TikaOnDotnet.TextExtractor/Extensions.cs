namespace TikaOnDotNet.TextExtraction
{
    /// <summary>
    /// Contains extension methods to standard .NET objects
    /// </summary>
	public static class Extensions
	{
        #region ToFormat
        public static string ToFormat(this string formatMe, params object[] args)
	    {
	        return string.Format(formatMe, args);
	    }
	    #endregion
    }
}