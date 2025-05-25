using Checkers.Models;
using Checkers.Views;
using System;
using System.Windows.Forms;

namespace Checkers.Controllers
{
    public class TournamentManager
    {
        private readonly GameController _controller;
        private readonly MainForm _view;
        private int _totalGames;
        private int _currentGame;
        private int _whiteWins;
        private int _blackWins;
        private TournamentStatusForm _statusForm;

        public TournamentManager(GameController controller, MainForm view)
        {
            _controller = controller;
            _view = view;
        }

        public void StartTournament(int gamesCount)
        {
            _totalGames = gamesCount;
            _currentGame = 0;
            _whiteWins = 0;
            _blackWins = 0;

            _controller.IsTournamentMode = true;

            _statusForm = new TournamentStatusForm();
            _statusForm.Show();

            PlayNextGame();
        }

        private void PlayNextGame()
        {
            // Підписуємося на подію завершення гри
            _controller.GameOver += OnControllerGameOver;

            // Запускаємо нову гру без інтерфейсу налаштувань
            _controller.NewGame(autoStart: true);

            // Оновлюємо статус
            _statusForm?.UpdateStatus(_currentGame, _totalGames, _whiteWins, _blackWins);
        }

        private void OnControllerGameOver(PlayerType winner, string style)
        {
            // Відписуємося, щоб не отримувати дублі
            _controller.GameOver -= OnControllerGameOver;

            // Лічимо переможців
            if (winner == PlayerType.White) _whiteWins++;
            else if (winner == PlayerType.Black) _blackWins++;

            _currentGame++;

            if (_currentGame < _totalGames)
            {
                PlayNextGame();
            }
            else
            {
                ShowFinalResults();
            }
        }

        private void ShowFinalResults()
        {
            _statusForm?.Close();
            _controller.IsTournamentMode = false;

            using (var resultsForm = new TournamentResultsForm(_totalGames, _whiteWins, _blackWins))
            {
                var result = resultsForm.ShowDialog();
                if (result == DialogResult.Retry)
                {
                    StartTournament(_totalGames);
                }
                else if (result == DialogResult.Abort)
                {
                    Application.Exit();
                }
            }
        }
    }
}
