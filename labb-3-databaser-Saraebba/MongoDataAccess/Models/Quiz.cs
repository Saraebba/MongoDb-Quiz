using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace MongoDataAccess.Models;

public class Quiz
{
    [BsonId]
    public ObjectId Id { get; set; }
    [BsonElement]
    public string Title { get; set; } = string.Empty;

    [BsonElement] 
    public List<Question> Questions { get; set; }  = new List<Question>();
}  