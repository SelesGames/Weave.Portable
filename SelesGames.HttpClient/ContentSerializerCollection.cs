using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace SelesGames.HttpClient
{
    public class ContentSerializerCollection : List<ContentSerializer>
    {
        public ContentSerializer FindReader(MediaTypeHeaderValue contentType)
        {
            return this.FirstOrDefault(o => o.CanRead(contentType));
        }

        public ContentSerializer FindWriter(MediaTypeHeaderValue contentType)
        {
            return this.FirstOrDefault(o => o.CanWrite(contentType));
        }
    }
}
