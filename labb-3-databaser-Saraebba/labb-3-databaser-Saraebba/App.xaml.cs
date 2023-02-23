using labb_3_databaser_Saraebba.Managers;
using labb_3_databaser_Saraebba.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MongoDataAccess.Managers;
using MongoDataAccess.Models;

namespace labb_3_databaser_Saraebba
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly NavigationManager _navigationManager;
        private readonly QuizManager _quizManager;

        public App()
        {
            _navigationManager = new NavigationManager();
            _quizManager = new QuizManager();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            _navigationManager.CurrentViewModel = new StartMenuViewModel(_navigationManager);
            MainWindow = new MainWindow
            {
                DataContext = new NavigationViewModel(_navigationManager)
            };
            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}

