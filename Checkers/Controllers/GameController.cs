using Checkers.Models;
using Checkers.Views;
using Checkers.Services;
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;

namespace Checkers.Controllers
{
    public class GameController
    {
        private readonly MainForm _view;
        private Game _game;
        private readonly GameSaver _saver;
        private readonly AIService _ai;
        private readonly GameStatistics _stats;
        private Stopwatch _gameTimer;
        private readonly GameSaver _gameSaver = new GameSaver();
        public GameStatistics GetStatistics() => _stats;
        private bool _playAgainstAI = false;
        private PlayerType _humanPlayerColor = PlayerType.White;

        public GameController(MainForm view)
        {
            _view = view;
            _saver = new GameSaver();
            _ai = new AIService();
            _stats = GameStatistics.LoadFromFile("stats.dat");
            _gameTimer = new Stopwatch();
            NewGame();
        }

        public void NewGame()
        {
            using (var settingsForm = new GameSettingsForm())
            {
                if (settingsForm.ShowDialog() != DialogResult.OK)
                    return;

                _playAgainstAI = settingsForm.PlayAgainstAI;
                _humanPlayerColor = settingsForm.PlayerColor;

                if (_playAgainstAI)
                {
                    _ai.SetDifficulty(settingsForm.SelectedDifficulty);
                }
            }

            _view.SetHumanPlayerColor(_humanPlayerColor);

            _gameTimer?.Stop();
            _game = new Game();
            _gameTimer = Stopwatch.StartNew();
            _view.UpdateGameState();

            if (_playAgainstAI && _humanPlayerColor == PlayerType.Black)
            {
                MakeAIMove();
            }
        }

        private void MakeAIMove()
        {
            var timer = new Timer { Interval = 500 };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                var move = _ai.GetBestMove(_game);
                if (move != null)
                {
                    _game.MakeMove(move);
                    _view.UpdateGameState();
                    FinalizeGameIfOver();

                    if (_game.CurrentPlayer != _humanPlayerColor &&
                        _game.ValidMoves.Any(m => m.CapturedPiece != null))
                    {
                        MakeAIMove();
                    }
                }
            };
            timer.Start();
        }

        public void HandleClick(int row, int col)
        {
            if (_game.IsGameOver)
                return;

            if (_playAgainstAI && _game.CurrentPlayer != _humanPlayerColor)
                return;

            _game.SelectPiece(row, col);
            _view.UpdateGameState();
            FinalizeGameIfOver();

            if (_playAgainstAI && _game.CurrentPlayer != _humanPlayerColor && !_game.IsGameOver)
            {
                MakeAIMove();
            }
        }

        private void FinalizeGameIfOver()
        {
            if (_game.IsGameOver)
            {
                _gameTimer.Stop();
                _stats.RecordGame(_game.Winner, _gameTimer.Elapsed);
                _stats.SaveToFile("stats.dat");
                _view.ShowGameOver(_game.Winner);
            }
        }

        public void SaveGame()
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "JSON файли (*.json)|*.json",
                Title = "Зберегти гру",
                DefaultExt = "json",
                AddExtension = true
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                if (_gameSaver.Save(_game, saveDialog.FileName))
                {
                    MessageBox.Show("Гра успішно збережена!", "Успіх",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public void LoadGame()
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "JSON файли (*.json)|*.json",
                Title = "Завантажити гру",
                CheckFileExists = true
            };

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                var loadedGame = _gameSaver.Load(openDialog.FileName);
                if (loadedGame != null)
                {
                    _game = loadedGame;
                    _view.UpdateGameState();
                    MessageBox.Show("Гра успішно завантажена!", "Успіх",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public void UndoLastMove()
        {
            if (_game == null || _game.IsGameOver) return;

            if (_playAgainstAI)
            {
                _game.Undo(); //AI
                _game.Undo(); //Player
            }
            else
            {
                _game.Undo(); //OnlyPlayer
            }

            _view.UpdateGameState();
        }

        public Game GetGame() => _game;
    }
}