using CommunityToolkit.Mvvm.Input;
using labb_3_databaser_Saraebba.Managers;
using MongoDataAccess.Managers;
using MongoDataAccess.Models;
using System.Collections.Generic;
using System.Windows;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq;

namespace labb_3_databaser_Saraebba.ViewModels;

public class PlayViewModel : ObservableObject
{
    private readonly NavigationManager _navigationManager;
    private readonly QuizManager _quizManager = new QuizManager();

    private List<int> index = new List<int>();

    private List<Question> _questions;
    public List<Question> Questions
    {
        get { return _questions; }
        set { SetProperty(ref _questions, value); }
    }

    private int _scoreCount;
    public int ScoreCount
    {
        get { return _scoreCount; }
        set { SetProperty(ref _scoreCount, value); }
    }

    private string _statement;
    public string Statement
    {
        get { return _statement; }
        set { SetProperty(ref _statement, value); }
    }

    private string _answerOne;
    public string AnswerOne
    {
        get { return _answerOne; }
        set { SetProperty(ref _answerOne, value); }
    }

    private string _answerTwo;
    public string AnswerTwo
    {
        get { return _answerTwo; }
        set { SetProperty(ref _answerTwo, value); }
    }

    private string _answerThree;
    public string AnswerThree
    {
        get { return _answerThree; }
        set { SetProperty(ref _answerThree, value); }
    }

    private int _correctAnswer;
    public int CorrectAnswer
    {
        get { return _correctAnswer; }
        set { SetProperty(ref _correctAnswer, value); }
    }


    private int _totatQuizLength;
    public int TotalQuizLength
    {
        get { return _totatQuizLength; }
        set { _totatQuizLength = value; }
    }


    private Question _currentQuestion;
    public Question CurrentQuestion
    {
        get { return _currentQuestion; }
        set
        {
            SetProperty(ref _currentQuestion, value);
            Statement = _currentQuestion.Statement;
            AnswerOne = _currentQuestion.Answers[0];
            AnswerTwo = _currentQuestion.Answers[1];
            AnswerThree = _currentQuestion.Answers[2];
            CorrectAnswer = _currentQuestion.CorrectAnswer;
        }
    }

    private int _quizLength;
    public int QuizLength
    {
        get { return _quizLength; }
        set { SetProperty(ref _quizLength, value); }
    }

    public IRelayCommand AltOneCommand { get; }
    public IRelayCommand AltTwoCommand { get; }
    public IRelayCommand AltThreeCommand { get; }
    public IRelayCommand NavigateToStartCommand { get; }

    public PlayViewModel(NavigationManager navigationManager, QuizManager quizManager)
    {
        _navigationManager = navigationManager;
        _quizManager = quizManager;
        QuizLength = 1;
        TotalQuizLength = _quizManager.CurrentQuiz.Questions.Count;

        QuizManagerOnCurrentQuizChanged();

        AltOneCommand = new RelayCommand(CheckAnswerOne);
        AltTwoCommand = new RelayCommand(CheckAnswerTwo);
        AltThreeCommand = new RelayCommand(CheckAnswerThree);
        NavigateToStartCommand = new RelayCommand(() =>
            _navigationManager.CurrentViewModel = new StartMenuViewModel(_navigationManager));
    }

    private void CheckAnswerOne()
    {
        if (CorrectAnswer == 0)
        {
            ScoreCount++;
        }
        QuizLength++;
        GetNextQuestion();
    }
    private void CheckAnswerTwo()
    {
        if (CorrectAnswer == 1)
        {
            ScoreCount++;
        }
        QuizLength++;
        GetNextQuestion();
    }
    private void CheckAnswerThree()
    {
        if (CorrectAnswer == 2)
        {
            ScoreCount++;
        }
        QuizLength++;
        GetNextQuestion();
    }

    public Question GetRandomQuestion()
    {
        var rnd = new Random();
        int index = rnd.Next(0, _questions.Count());
        Question question = _questions.ElementAt(index);
        return question;
    }

    private void GetNextQuestion()
    {
        var question = GetRandomQuestion();
        if (index.Count == _quizManager.CurrentQuiz.Questions.Count)
        {
            QuizLength += -1;
            ShowResultBox();
            return;
        }
        while (index.Contains(Questions.IndexOf(question)))
        {
            question = GetRandomQuestion();
        }
        index.Add(Questions.IndexOf(question));
        CurrentQuestion = question;
    }

    private void QuizManagerOnCurrentQuizChanged()
    {
        Questions = new List<Question>(_quizManager.CurrentQuiz.Questions);
        var question = GetRandomQuestion();
        index.Add(Questions.IndexOf(question));
        CurrentQuestion = question;
    }

    private void ShowResultBox()
    {
        double showPercentage = (double)(ScoreCount * 100) / TotalQuizLength;
        string message = $"Quiz Finished!\n";
        message += $" \n";
        message += $"You got {Math.Round(showPercentage)}% right.\n";
        message += $"Your total score is is {ScoreCount}/{TotalQuizLength}";

        if (MessageBox.Show(message, "Result",
              MessageBoxButton.OK) == MessageBoxResult.OK)
        {
            _navigationManager.CurrentViewModel = new StartMenuViewModel(_navigationManager);
        }
    }
}