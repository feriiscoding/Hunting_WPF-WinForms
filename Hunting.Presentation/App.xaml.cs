using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Hunting.Presentation.Model;
using Hunting.Presentation.ViewModel;
using Hunting.Presentation.Persistence;
using Microsoft.Win32;
using Hunting.Presentation.View;
using System;

namespace Hunting.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private HuntingGameModel _model = null!;
        private HuntingViewModel _viewModel = null!;
        private MainWindow _view = null!;
        private IHuntingDataAccess _persistence = null!;
        private OpenFileDialog? _openFileDialog;
        private SaveFileDialog? _saveFileDialog;
        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _persistence = new TextFilePersistence();

            _model = new HuntingGameModel(_persistence);
            _model.GameOver += new EventHandler<HuntingEventArgs>(Model_GameOver);
            _model.NewGame();

            _viewModel = new HuntingViewModel(_model);
            _viewModel.LoadGame += new EventHandler(ViewModel_LoadGame!);
            _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame!);
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame!);

            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Close!);
            _view.Show();
        }
        private void View_Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit the game?", "Hunting", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
        private async void ViewModel_LoadGame(object? sender, System.EventArgs e)
        {
            if (_openFileDialog == null)
            {
                _openFileDialog = new OpenFileDialog();
                _openFileDialog.Title = "Hunting - Load Game";
                _openFileDialog.Filter = "Szövegfájlok|*.txt";
            }
            if (_openFileDialog.ShowDialog() == true)
            {
                try
                {
                    await _model.LoadGameAsync(_openFileDialog.FileName);
                }
                catch (DataException)
                {
                    MessageBox.Show("An error occured during loading.", "Hunting", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ViewModel_SaveGame(object? sender, System.EventArgs e)
        {
            if (_saveFileDialog == null)
            {
                _saveFileDialog = new SaveFileDialog();
                _saveFileDialog.Title = "Hunting - Save Game";
                _saveFileDialog.Filter = "Szövegfájlok|*.txt";
            }
            if (_saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    await _model.SaveGameAsync(_saveFileDialog.FileName);
                }
                catch (DataException)
                {
                    MessageBox.Show("An error occured during saving.", "Hunting", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void ViewModel_ExitGame(object sender, EventArgs e)
        {
            Shutdown();
        }
        private void Model_GameOver(object? sender, HuntingEventArgs e)
        {
            if (e.HuntersWon)
            {
                MessageBox.Show("The cats won the game!", "Game over!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            if (e.PreyWon)
            {
                MessageBox.Show("The tuna won the game!", "Game over!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            _model.NewGame();
        }
    }

}
