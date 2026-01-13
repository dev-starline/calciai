using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CalciAI.Persistance
{
    public interface IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        ObjectId Id { get; set; }

        DateTime CreatedAt { get; }

    }

    public abstract class Document : IDocument
    {
        public ObjectId Id { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt => Id.CreationTime;
    }

    public abstract class OpDoc : Document
    {
        [BsonElement("operator_id")]
        public string OperatorId { get; set; }
    }

    public abstract class OuDoc : OpDoc
    {
        /// <summary>
        /// Username without operator id
        /// </summary>
        [BsonElement("username")]
        public string Username { get; set; }

        /// <summary>
        /// This is combined username of operator_id and username
        /// </summary>
        [BsonElement("user_id")]
        public string UserId => $"{OperatorId}:{Username}";

    }

    public abstract class OuWithParentsDoc : OuDoc
    {
        /// <summary>
        /// List of OperatorUserIds
        /// </summary>
        [BsonElement("parent_sharing")]
        public List<ParentSharingDoc> ParentSharing { get; set; }

        [BsonElement("parents")]
        public string[] Parents { get; set; }
    }

    public class ParentSharingDoc
    {
        [BsonElement("dealer_id")]
        public string DealerId { get; set; }

        [BsonElement("sharing")]
        public double Sharing { get; set; }
    }

}
