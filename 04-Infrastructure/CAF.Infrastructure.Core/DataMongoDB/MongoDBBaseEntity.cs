using CAF.Infrastructure.Core.Auditing;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace CAF.Infrastructure.Core
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    // [DataContract]
    public abstract partial class MongoDBBaseEntity : BaseEntity<int>
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>                
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public string _id { get; set; }        

    }
}
