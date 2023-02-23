using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace MongoDataAccess.Models;

public class Category
{

    [BsonId]
    public ObjectId Id { get; set; }
    [BsonElement]
    public string CategoryName { get; set; } = string.Empty;

}