using CalciAI.Models;
using MongoDB.Bson.Serialization.Serializers;

namespace CalciAI.Persistance.Serializers
{
    public class OperatorUserIdSerializer : SerializerBase<OperatorUserId>
    {
        public override void Serialize(MongoDB.Bson.Serialization.BsonSerializationContext context, MongoDB.Bson.Serialization.BsonSerializationArgs args, OperatorUserId value)
        {
            context.Writer.WriteString(value.ToString());
        }

        public override OperatorUserId Deserialize(MongoDB.Bson.Serialization.BsonDeserializationContext context, MongoDB.Bson.Serialization.BsonDeserializationArgs args)
        {
            return new OperatorUserId(context.Reader.ReadString());
        }
    }
}