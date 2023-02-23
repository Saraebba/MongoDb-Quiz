using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace labb_3_databaser_Saraebba.Managers;

public class NavigationManager
{
    private ObservableObject _currentViewModel;

    public ObservableObject CurrentViewModel
    {
        get { return _currentViewModel; }
        set
        {
            _currentViewModel = value;
            OnCurrentViewModelChanged();
        }
    }

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }

    public event Action CurrentViewModelChanged;
}