using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Compression
{
    public class CompressionHandlerCollection : List<CompressionHandler>
    {
        public IEnumerable<string> GetSupportedEncodings()
        {
            return this.SelectMany(o => o.SupportedEncodings);
        }

        public CompressionHandler Find(string encoding)
        {
            return this
                .FirstOrDefault(o => o.SupportedEncodings
                    .Any(e => e.Equals(encoding, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
