using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using labb_3_databaser_Saraebba.Managers;
using MongoDataAccess.Managers;
using MongoDataAccess.Models;

namespace labb_3_databaser_Saraebba.ViewModels;
public class CreateQuestionViewModel : ObservableObject
{
    private readonly NavigationManager _navigationManager;
    private readonly QuizManager _quizManager = new QuizManager();


    private ObservableCollection<Question> _questionCollection;
    public ObservableCollection<Question> QuestionCollection
    {
        get { return _questionCollection; }
        set
        {
            SetProperty(ref _questionCollection, value);
        }
    }

    private ObservableCollection<Category> _categoryCollection;
    public ObservableCollection<Category> CategoryCollection
    {
        get { return _categoryCollection; }
        set { SetProperty(ref _categoryCollection, value); }
    }

    private ObservableCollection<Category> _selectedQuestionCategories;
    public ObservableCollection<Category> SelectedQuestionCategories
    {
        get { return _selectedQuestionCategories; }
        set
        {
            SetProperty(ref _selectedQuestionCategories, value);
            OnPropertyChanged(nameof(SelectedQuestionCategories));
        }
    }

    private Category _selectedQestionCategory;
    public Category SelectedQuestionCategory
    {
        get { return _selectedQestionCategory; }
        set
        {
            SetProperty(ref _selectedQestionCategory, value);
        }
    }

    private Category _selectedCategory;
    public Category SelectedCategory
    {
        get { return _selectedCategory; }
        set
        {
            if (value is null) return;
            if (SetProperty(_selectedCategory, value, (v) => _selectedCategory = v))
            {
              CategoryName   = _selectedCategory.CategoryName;
            }
        }
    }

    private Question _selectedQuestion;
    public Question SelectedQuestion
    {
        get { return _selectedQuestion; }
        set
        {
            if (value is null) return;
            if (SetProperty(_selectedQuestion, value, (v) => _selectedQuestion = v))
            {
                Statement = _selectedQuestion.Statement;
                AnswerOne = _selectedQuestion.Answers[0];
                AnswerTwo = _selectedQuestion.Answers[1];
                AnswerThree = _selectedQuestion.Answers[2];
                CorrectAnswer = _selectedQuestion.CorrectAnswer;
            }
            QuestionCategories();
        }
    }

    private string _categoryName;
    public string CategoryName
    {
        get { return _categoryName; }
        set {SetProperty(ref _categoryName, value); }
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

    private bool _radioButton1;
    public bool RadioButton1
    {
        get { return _radioButton1; }
        set { SetProperty(ref _radioButton1, value); }
    }

    private bool _radioButton2;
    public bool RadioButton2
    {
        get { return _radioButton2; }
        set { SetProperty(ref _radioButton2, value); }
    }

    private bool _radioButton3;
    public bool RadioButton3
    {
        get { return _radioButton3; }
        set { SetProperty(ref _radioButton3, value); }
    }

    public int CorrectAnswer
    {
        get
        {
            if (RadioButton2) return 1;
            if (RadioButton3) return 2;
            return 0;
        }
        set
        {
            switch (value)
            {
                case 0:
                    RadioButton1 = true;
                    break;
                case 1:
                    RadioButton2 = true;
                    break;
                case 2:
                    RadioButton3 = true;
                    break;
            }
        }
    }

    public IRelayCommand AddQuestionCommand { get; }
    public IRelayCommand DeleteQuestionCommand { get; }
    public IRelayCommand UpdateQuestionCommand { get; }
    public IRelayCommand NavigateToStartCommand { get; }
    public IRelayCommand AddCategoryCommand { get; }
    public IRelayCommand DeleteCategoryCommand { get; }
    public IRelayCommand PushCategoryCommand { get; }
    public IRelayCommand PullCategoryCommand { get; }


    public CreateQuestionViewModel(NavigationManager navigationManager, QuizManager quizManager)
    {
        _navigationManager = navigationManager;
        _quizManager = quizManager;

        QuestionCollection = new ObservableCollection<Question>(_quizManager.GetAllQuestions());
        CategoryCollection = new ObservableCollection<Category>(_quizManager.GetAllCategories());
        AddQuestionCommand = new RelayCommand(AddQuestion);
        DeleteQuestionCommand = new RelayCommand(DeleteQuestion);
        UpdateQuestionCommand = new RelayCommand(UpdateQuestion);
        AddCategoryCommand = new RelayCommand(AddCategory);
        DeleteCategoryCommand = new RelayCommand(DeleteCategory);
        PushCategoryCommand = new RelayCommand(PushCategory);
        PullCategoryCommand = new RelayCommand(PullCategory);
        NavigateToStartCommand = new RelayCommand(() =>
            _navigationManager.CurrentViewModel = new StartMenuViewModel(_navigationManager));
    }


    private void AddQuestion()
    {
        if (string.IsNullOrEmpty(Statement) && string.IsNullOrEmpty(AnswerOne) && string.IsNullOrEmpty(AnswerTwo) && string.IsNullOrEmpty(AnswerThree)) return;
        var question = new Question()
        {
            Statement = _statement,
            Answers = new[] { AnswerOne, AnswerTwo, AnswerThree },
            CorrectAnswer = CorrectAnswer
        };
        _quizManager.AddQuestion(question);
        UpdateQuestionList();
    }

    private void DeleteQuestion()
    {
        if (_selectedQuestion is Question question)
        {
            _quizManager.DeleteQuestion(question.Id);
        }
        UpdateQuestionList();
    }

    private void UpdateQuestion()
    {
        if (SelectedQuestion is Question question)
        {
            if (!string.IsNullOrEmpty(_statement) && !string.IsNullOrEmpty(_answerOne) && !string.IsNullOrEmpty(_answerTwo) && !string.IsNullOrEmpty(_answerThree))
            {
                _quizManager.UpdateQuestion(question.Id, _statement, new []{_answerOne, _answerTwo, _answerThree}, CorrectAnswer );
            }
            UpdateQuestionList();
        }
    }

    private void UpdateQuestionList()
    {
        var question = _quizManager.GetAllQuestions();
            QuestionCollection.Clear();
        foreach (var statment in question)
        {
            QuestionCollection.Add(statment);
        }
    }

    private ObservableCollection<Category> QuestionCategories()
    {
        SelectedQuestionCategories = new ObservableCollection<Category>();

        foreach (var C in SelectedQuestion.Categories)
        {
            SelectedQuestionCategories.Add(C);
        }
        return SelectedQuestionCategories;
    }

    private void AddCategory()
    {
        if (string.IsNullOrEmpty(CategoryName)) return;
        var category = new Category()
        {
            CategoryName = _categoryName
        };
        _quizManager.AddCategory(category);
        UpdateCategoryList();
    }

    private void DeleteCategory()
    {
        if (_selectedCategory is Category category)
        {
            _quizManager.DeleteCategory(category.Id);
        }
        UpdateCategoryList();
    }

    private void PushCategory()
    {
        if ( _selectedQuestion is null) return;
        if (_selectedQuestion is Question question && _selectedCategory is Category category)
        {
            _quizManager.PushCategory(_selectedQuestion.Id, category);
        }
        UpdateQuestionCategories();
    }

    private void PullCategory()
    {
        if (_selectedQuestion is null) return;
        if (_selectedQuestion is Question question && _selectedQestionCategory is Category category)
        {
            _quizManager.PullCategory(_selectedQuestion.Id, category);
        }
        UpdateQuestionCategories();
    }

    private void UpdateCategoryList()
    {
        var categories = _quizManager.GetAllCategories();
        CategoryCollection.Clear();
        foreach (var category in categories)
        {
            CategoryCollection.Add(category);
        }
    }

    private void UpdateQuestionCategories()
    {
        _selectedQuestionCategories.Clear();
        _quizManager.QuestionCategories(_selectedQuestion).ToList().ForEach(c =>
        {
            _selectedQuestionCategories.Add(c);
        });
    }
}
