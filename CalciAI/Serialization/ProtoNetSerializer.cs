using System;
using System.IO;

namespace CalciAI.Serialization
{
    public sealed class ProtoNetSerializer
    {
        private ProtoNetSerializer()
        {

        }

        public static byte[] Serialize(object record)
        {
            if (null == record)
            {
                return Array.Empty<byte>();
            }

            using var stream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(stream, record);
            return stream.ToArray();
        }

        public static T Deserialize<T>(byte[] data) where T : class
        {
            if (null == data) return null;

            using var stream = new MemoryStream(data);
            return ProtoBuf.Serializer.Deserialize<T>(stream);
        }
    }
}
