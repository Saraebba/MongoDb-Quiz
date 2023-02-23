using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using labb_3_databaser_Saraebba.Managers;
using MongoDataAccess.Managers;
using MongoDataAccess.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace labb_3_databaser_Saraebba.ViewModels;

public class StartMenuViewModel : ObservableObject
{
    private readonly NavigationManager _navigationManager;
    private readonly QuizManager _quizManager = new QuizManager();


    private ObservableCollection<Quiz> _quizCollection;
    public ObservableCollection<Quiz> QuizCollection
    {
        get { return _quizCollection; }
        set { SetProperty(ref _quizCollection, value); }
    }

    private Quiz _selectedQuiz;
    public Quiz SelectedQuiz
    {
        get { return _selectedQuiz; }
        set { SetProperty(ref _selectedQuiz, value); }
    }


    public IRelayCommand EditCommand { get; }
    public IRelayCommand ModifyQuizCommand { get; }
    public IRelayCommand PlayCommand {get;}

    public StartMenuViewModel(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;

        QuizCollection = new ObservableCollection<Quiz>(_quizManager.GetAllQuizzes());

        EditCommand = new RelayCommand(() => _navigationManager.CurrentViewModel = new CreateQuestionViewModel(_navigationManager, _quizManager)); 
        _navigationManager = navigationManager;
        ModifyQuizCommand = new RelayCommand(() => _navigationManager.CurrentViewModel = new CreateQuizViewModel(_navigationManager, _quizManager));
        PlayCommand = new RelayCommand(NavigateToPlay);
    }

    private void NavigateToPlay()
    {
        _quizManager.CurrentQuiz = SelectedQuiz;

        if (_quizManager.CurrentQuiz != null)
        {
            _navigationManager.CurrentViewModel = new PlayViewModel(_navigationManager, _quizManager);
        }
        return;
    }
}