using MongoDataAccess.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using MongoDB.Bson;
using System.Windows.Controls;


namespace MongoDataAccess.Managers;

public class QuizManager
{
    private Quiz _currentQuiz;

    public Quiz CurrentQuiz
    {
        get { return _currentQuiz; }
        set
        {
            _currentQuiz = value;
            CurrentQuizChanged?.Invoke();
        }
    }

    public event Action CurrentQuizChanged;

    private const string hostname = "localhost";
    private const string databaseName = "SarasQuizApp";
    private const string connectionString = $"mongodb://{hostname}:27017";
    private const string QuizCollection = "Quiz";
    private const string QuestionCollection = "Question";
    private const string CategoryCollection = "Category";


    private IMongoCollection<T> ConnectToMongo<T>(in string collection)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        return database.GetCollection<T>(collection);

    }

    public void AddQuiz(Quiz quiz)
    {
        var quizCollection = ConnectToMongo<Quiz>(QuizCollection);
        quizCollection.InsertOne(quiz);
    }

    public IEnumerable<Quiz> GetAllQuizzes()
    {
        var quizCollection = ConnectToMongo<Quiz>(QuizCollection);
        return quizCollection.Find(_ => true).ToEnumerable();
    }

    public IEnumerable<Question> QuizQuestions(Quiz quiz)
    {
        var quizCollection = ConnectToMongo<Quiz>(QuizCollection);
       return quizCollection.Find(q => q.Id == quiz.Id).FirstOrDefault().Questions.ToList();
    }


    public void UpdateQuiz(object id, string title)
    {
        var quizCollection = ConnectToMongo<Quiz>(QuizCollection);

        var filter = Builders<Quiz>.Filter.Eq("Id", id);
        var update = Builders<Quiz>
            .Update
            .Set("Title", title);

        quizCollection
            .FindOneAndUpdate(
                filter,
                update,
                new FindOneAndUpdateOptions<Quiz, Quiz>
                {
                    IsUpsert = true
                }
            );
    }

    public void DeleteQuiz(object id)
    {
        var quizCollection = ConnectToMongo<Quiz>(QuizCollection);

        var filter = Builders<Quiz>.Filter.Eq("Id", id);
        quizCollection.FindOneAndDelete(filter);
    }

    public void PullQuestion(object id, Question question)
    {
        var quizCollection = ConnectToMongo<Quiz>(QuizCollection);

        var filter = Builders<Quiz>.Filter.Eq("Id", id);
        var pull = Builders<Quiz>.Update.Pull(q => q.Questions, question);
        quizCollection.FindOneAndUpdate(filter, pull, new FindOneAndUpdateOptions<Quiz, Quiz>
        {
            IsUpsert = true
        });
    }

    public void PushQuestion(object id, Question question)
    {
        var quizCollection = ConnectToMongo<Quiz>(QuizCollection);
        var quiz = quizCollection.Find(q => q.Id == (ObjectId)id).FirstOrDefault();

        if (quiz.Questions.Any(q => q.Statement == question.Statement))
        {
            return;
        }

        var filter = Builders<Quiz>.Filter.Eq("Id", id);
        var push = Builders<Quiz>.Update.Push(q => q.Questions, question);
        quizCollection.FindOneAndUpdate(filter, push, new FindOneAndUpdateOptions<Quiz, Quiz>
        {
            IsUpsert = true
        });
    }


    public void AddQuestion(Question question)
    {
        var questionCollection = ConnectToMongo<Question>(QuestionCollection);

        questionCollection.InsertOne(question);
    }

    public IEnumerable<Question> GetAllQuestions()
    {
        var questionCollection = ConnectToMongo<Question>(QuestionCollection);
        return questionCollection.Find(_ => true).ToEnumerable();
    }

    public void UpdateQuestion(object id, string statement, string[] answers, int correctAnswer)
    {
        var questionCollection = ConnectToMongo<Question>(QuestionCollection);

        var filter = Builders<Question>.Filter.Eq("Id", id);
        var update = Builders<Question>
            .Update
            .Set("Statement", statement)
            .Set("Answers", answers)
            .Set("CorrectAnswer", correctAnswer);
        questionCollection
            .FindOneAndUpdate(
                filter,
                update,
                new FindOneAndUpdateOptions<Question, Question>
                {
                    IsUpsert = true
                }
            );
    }

    public void DeleteQuestion(object id)
    {
        var questionCollection = ConnectToMongo<Question>(QuestionCollection);

        var filter = Builders<Question>.Filter.Eq("Id", id);
        questionCollection.FindOneAndDelete(filter);
    }

    public IEnumerable<Category> GetAllCategories()
    {
        var categoryCollection = ConnectToMongo<Category>(CategoryCollection);
        return categoryCollection.Find(_ => true).ToEnumerable();
    }

    public void AddCategory(Category category)
    {
        var categoryCollection = ConnectToMongo<Category>(CategoryCollection);

        categoryCollection.InsertOne(category);
    }

    public void DeleteCategory(object id)
    {
        var categoryCollection = ConnectToMongo<Category>(CategoryCollection);

        var filter = Builders<Category>.Filter.Eq("Id", id);
        categoryCollection.FindOneAndDelete(filter);
    }

    public void PushCategory(object id, Category category)
    {
        var questionCollection = ConnectToMongo<Question>(QuestionCollection);
        var question = questionCollection.Find(q => q.Id == (ObjectId)id).FirstOrDefault();

        if (question.Categories.Any(q => q.CategoryName == category.CategoryName))
        {
            return;
        }

        var filter = Builders<Question>.Filter.Eq("Id", id);
        var push = Builders<Question>.Update.Push(q => q.Categories, category);
        questionCollection.FindOneAndUpdate(filter, push, new FindOneAndUpdateOptions<Question, Question>
        {
            IsUpsert = true
        });
    }

    public IEnumerable<Category> QuestionCategories(Question question)
    {
        var questionCollection = ConnectToMongo<Question>(QuestionCollection);
        return questionCollection.Find(q => q.Id == question.Id).FirstOrDefault().Categories.ToList();
    }

    public IEnumerable<Question> CategoryQuestions (Category category)
    {
        var questionCollection = ConnectToMongo<Question>(QuestionCollection);
        var filter = Builders<Question>.Filter.ElemMatch(q  => q.Categories, c => c.CategoryName == category.CategoryName);
        var result = questionCollection.Find(filter);
        return result.ToEnumerable();
    }

    public IEnumerable<Question> SearchQuestions(string search)
    {
        var questionCollection = ConnectToMongo<Question>(QuestionCollection);
        search = "/.*" + search + ".*/i";
        var filter = Builders<Question>.Filter.Regex(q => q.Statement, search);
        var result = questionCollection.Find(filter);
        return result.ToEnumerable();
    }

    public void PullCategory(object id, Category category)
    {
        var questionCollection = ConnectToMongo<Question>(QuestionCollection);

        var filter = Builders<Question>.Filter.Eq("Id", id);
        var pull = Builders<Question>.Update.Pull(q => q.Categories, category);
        questionCollection.FindOneAndUpdate(filter, pull, new FindOneAndUpdateOptions<Question, Question>
        {
            IsUpsert = true
        });
    }

}