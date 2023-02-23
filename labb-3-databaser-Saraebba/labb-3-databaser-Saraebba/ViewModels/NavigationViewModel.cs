using System.Runtime.InteropServices.ComTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using labb_3_databaser_Saraebba.Managers;

namespace labb_3_databaser_Saraebba.ViewModels;

public class NavigationViewModel : ObservableObject
{
    private readonly NavigationManager _navigationManager;

    public ObservableObject CurrentViewModel => _navigationManager.CurrentViewModel;

    public NavigationViewModel(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;

        _navigationManager.CurrentViewModelChanged += CurrentViewModelChanged;
    }

    private void CurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }
}