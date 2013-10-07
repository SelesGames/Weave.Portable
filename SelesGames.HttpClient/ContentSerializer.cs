using System.IO;
using System.Net.Http.Headers;

namespace SelesGames.HttpClient
{
    public abstract class ContentSerializer
    {
        public ContentEncoderSettings ContentEncoderSettings { get; set; }

        public bool CanRead(MediaTypeHeaderValue contentType)
        {
            return contentType.MediaType.Equals(ContentEncoderSettings.ContentType);
        }

        public bool CanWrite(MediaTypeHeaderValue contentType)
        {
            return contentType.MediaType.Equals(ContentEncoderSettings.ContentType);
        }

        public abstract T ReadObject<T>(Stream readStream);
        public abstract void WriteObject<T>(Stream writeStream, T obj);
    }
}
