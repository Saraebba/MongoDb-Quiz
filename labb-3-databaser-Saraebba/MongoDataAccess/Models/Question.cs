using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace MongoDataAccess.Models;

public class Question
{
    [BsonId]
    public ObjectId Id { get; set; }
    [BsonElement]
    public string Statement { get; set; }
    [BsonElement]
    public string[] Answers { get; set; }
    [BsonElement]
    public int CorrectAnswer { get; set; }
    [BsonElement]
    public List<Category> Categories { get; set; } = new List<Category>();
}