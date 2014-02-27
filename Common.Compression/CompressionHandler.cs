using System.Collections.ObjectModel;
using System.IO;

namespace Common.Compression
{
    public abstract class CompressionHandler
    {
        readonly Collection<string> supportedEncodings = new Collection<string>();

        public Collection<string> SupportedEncodings { get { return supportedEncodings; } }

        public abstract Stream Compress(Stream inputStream);
        public abstract Stream Decompress(Stream inputStream);
    }
}
