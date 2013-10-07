
namespace SelesGames.HttpClient.SerializerModules
{
    public class ProtobufSerializer : ContentSerializer
    {
        public ProtobufSerializer()
        {
            ContentEncoderSettings = ContentEncoderSettings.Protobuf;
        }

        public override T ReadObject<T>(System.IO.Stream readStream)
        {
            return ProtoBuf.Serializer.Deserialize<T>(readStream);
        }

        public override void WriteObject<T>(System.IO.Stream writeStream, T obj)
        {
            ProtoBuf.Serializer.Serialize(writeStream, obj);
        }
    }
}
