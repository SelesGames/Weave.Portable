using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace SelesGames.HttpClient.SerializerModules
{
    public class JsonSerializer : ContentSerializer
    {
        public Encoding Encoding { get; set; }
        public JsonSerializerSettings SerializerSettings { get; set; }

        public JsonSerializer()
        {
            ContentEncoderSettings = ContentEncoderSettings.Json;

            Encoding = new UTF8Encoding(false, false);
            SerializerSettings = new JsonSerializerSettings();
        }

        public override T ReadObject<T>(Stream readStream)
        {
            var serializer = Newtonsoft.Json.JsonSerializer.Create(SerializerSettings);
            using (var streamReader = new StreamReader(readStream, Encoding))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return serializer.Deserialize<T>(jsonTextReader);
            }
        }

        public override void WriteObject<T>(Stream writeStream, T obj)
        {
            var serializer = Newtonsoft.Json.JsonSerializer.Create(SerializerSettings);

            using (var ms = new MemoryStream())
            using (var streamWriter = new StreamWriter(ms, Encoding))
            using (var jsonTextWriter = new JsonTextWriter(streamWriter))
            {
                serializer.Serialize(jsonTextWriter, obj);
                jsonTextWriter.Flush();

                ms.Position = 0;
                ms.CopyTo(writeStream);

                jsonTextWriter.Close();
            }
        }
    }
}