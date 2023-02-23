using CommunityToolkit.Mvvm.ComponentModel;
using labb_3_databaser_Saraebba.Managers;
using MongoDataAccess.Managers;
using MongoDataAccess.Models;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;

namespace labb_3_databaser_Saraebba.ViewModels;

public class CreateQuizViewModel : ObservableObject
{
    private readonly NavigationManager _navigationManager;
    private readonly QuizManager _quizManager = new QuizManager();


    private ObservableCollection<Quiz> _quizCollection;
    public ObservableCollection<Quiz> QuizCollection
    {
        get { return _quizCollection; }
        set { SetProperty(ref _quizCollection, value); }
    }

    private ObservableCollection<Question> _questionCollection;
    public ObservableCollection<Question> QuestionCollection
    {
        get { return _questionCollection; }
        set
        {
            SetProperty(_questionCollection, value, (v) => _questionCollection = v);
            OnPropertyChanged(nameof(QuestionCollection));
        }
    }

    private ObservableCollection<Question>? _selectedQuizQuestions;
    public ObservableCollection<Question>? SelectedQuizQuestions
    {
        get { return _selectedQuizQuestions; }
        set
        {
            SetProperty(ref _selectedQuizQuestions, value);
            OnPropertyChanged(nameof(SelectedQuizQuestions));
        }
    }


    private ObservableCollection<Category> _categoryCollection;
    public ObservableCollection<Category> CategoryCollection
    {
        get { return _categoryCollection; }
        set { SetProperty(ref _categoryCollection, value); }
    }


    private Category _selectedCategory;
    public Category SelectedCategory
    {
        get { return _selectedCategory; }
        set
        {
            SetProperty(_selectedCategory, value, (v) => _selectedCategory = v);
            QuestionCollection = new ObservableCollection<Question>(_quizManager.CategoryQuestions(_selectedCategory));
        }
    }

    private string _nameSearch;
    public string NameSearch
    {
        get { return _nameSearch; }
        set
        {
            SetProperty(_nameSearch, value, (v) => _nameSearch = v);
            QuestionCollection = new ObservableCollection<Question>(_quizManager.SearchQuestions(_nameSearch));
        }
    }


    private Question _selectedQuizQuestion;
    public Question SelectedQuizQuestion
    {
        get { return _selectedQuizQuestion; }
        set
        {
            SetProperty(ref _selectedQuizQuestion, value);
        }
    }

    private Quiz _selectedQuiz;
    public Quiz SelectedQuiz
    {
        get { return _selectedQuiz; }
        set
        {
            if (value is null) return;
            if (SetProperty(_selectedQuiz, value, (v) => _selectedQuiz = v))
            {
                Title = _selectedQuiz.Title;
            }
            QuizQuestions();
        }
    }

    private Question? _selectedQuestion;
    public Question? SelectedQuestion
    {
        get { return _selectedQuestion; }
        set { SetProperty(ref _selectedQuestion, value); }
    }


    private string _title;
    public string Title
    {
        get { return _title; }
        set { SetProperty(ref _title, value); }
    }

    private string _statement;
    public string Statement
    {
        get { return _statement; }
        set { SetProperty(ref _statement, value); }
    }

    public IRelayCommand AddQuizCommand { get; }
    public IRelayCommand DeleteQuizCommand { get; }
    public IRelayCommand UpdateQuizCommand { get; }
    public IRelayCommand NavigateToStartCommand { get; }
    public IRelayCommand AddQuestionCommand { get; }
    public IRelayCommand DeleteQuestionCommand { get; }
    public IRelayCommand AllQuestionsCommand { get; }


    public CreateQuizViewModel(NavigationManager navigationManager, QuizManager quizManager)
    {
        _navigationManager = navigationManager;
        _quizManager = quizManager;


        QuizCollection = new ObservableCollection<Quiz>(_quizManager.GetAllQuizzes());
        QuestionCollection = new ObservableCollection<Question>(_quizManager.GetAllQuestions());
        CategoryCollection = new ObservableCollection<Category>(_quizManager.GetAllCategories());

        AddQuizCommand = new RelayCommand(AddQuiz);
        DeleteQuizCommand = new RelayCommand(DeleteQuiz);
        UpdateQuizCommand = new RelayCommand(UpdateQuiz);
        NavigateToStartCommand = new RelayCommand(() =>
            _navigationManager.CurrentViewModel = new StartMenuViewModel(_navigationManager));
        AddQuestionCommand = new RelayCommand(AddQuestion);
        DeleteQuestionCommand = new RelayCommand(DeleteQuestion);
        AllQuestionsCommand =
            new RelayCommand(() => QuestionCollection = new ObservableCollection<Question>(_quizManager.GetAllQuestions()));
    }

    private void AddQuiz()
    {
        if (string.IsNullOrEmpty(Title)) return;
        var quiz = new Quiz()
        {
            Title = _title
        };
        _quizManager.AddQuiz(quiz);
        UpdateQuizList();
    }
    private void DeleteQuiz()
    {
        if (_selectedQuiz is Quiz quiz)
        {
            _quizManager.DeleteQuiz(quiz.Id);
        }
        UpdateQuizList();
    }

    private void UpdateQuiz()
    {
        if (SelectedQuiz is Quiz quiz)
        {
            if (!string.IsNullOrEmpty(_title))
            {
                _quizManager.UpdateQuiz(quiz.Id, _title);
            }
            UpdateQuizList();
        }
    }

    private void UpdateQuizList()
    {
        var quiz = _quizManager.GetAllQuizzes();
        QuizCollection.Clear();
        foreach (var title in quiz)
        {
            QuizCollection.Add(title);
        }
    }

    private ObservableCollection<Question> QuizQuestions()
    {
        QuestionCollection.Clear();
        SelectedQuizQuestions = new ObservableCollection<Question>();

        foreach (var Q in SelectedQuiz.Questions)
        {
            SelectedQuizQuestions.Add(Q);
        }
        return SelectedQuizQuestions;
    }

    private void AddQuestion()
    {
        if (_selectedQuiz is null) return;
        if (_selectedQuiz is Quiz quiz && _selectedQuestion is Question question)
        {
            _quizManager.PushQuestion(_selectedQuiz.Id, question);
        }
        UpdateQuizQuestions();
    }

    private void DeleteQuestion()
    {
        if (_selectedQuizQuestion is null) return;
        if (_selectedQuiz is Quiz quiz && _selectedQuizQuestion is Question question)
        {
            _quizManager.PullQuestion(_selectedQuiz.Id, question);
        }
        UpdateQuizQuestions();
    }

    private void UpdateQuizQuestions()
    {
        _selectedQuizQuestions.Clear();
        _quizManager.QuizQuestions(_selectedQuiz).ToList().ForEach(q =>
        {
            _selectedQuizQuestions.Add(q);
        });
    }

}