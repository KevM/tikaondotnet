using java.io;

namespace TikaOnDotNet.TextExtraction.Stream
{
    internal class DotNetOutputStreamAdapter : OutputStream
    {
        private readonly System.IO.Stream _stream;

        public DotNetOutputStreamAdapter(System.IO.Stream stream)
        {
            _stream = stream;
        }

        public override void write(int b)
        {
            _stream.WriteByte((byte)b);
        }

        public override void write(byte[] b, int off, int len)
        {
            _stream.Write(b, off, len);
        }

        public override void write(byte[] b)
        {
            _stream.Write(b, 0, b.Length);
        }
    }
}
