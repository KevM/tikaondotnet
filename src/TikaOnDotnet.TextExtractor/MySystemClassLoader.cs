using ikvm.runtime;
using java.lang;

namespace TikaOnDotNet.TextExtraction
{
    /// <summary>
    /// See http://weblog.ikvm.net/PermaLink.aspx?guid=375f1ff8-912a-4458-9120-f0a8cfb23b68
    /// </summary>
	public class MySystemClassLoader : ClassLoader
	{
		public MySystemClassLoader(ClassLoader parent) : base(new AppDomainAssemblyClassLoader(typeof(MySystemClassLoader).Assembly))
		{
		}
	}
}