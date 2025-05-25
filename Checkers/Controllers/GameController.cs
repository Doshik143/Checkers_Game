using Checkers.Models;
using Checkers.Services;
using Checkers.Views;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Checkers.Controllers
{
    public class GameController
    {
        private Game _game;
        private readonly GameSaver _saver;
        private readonly AIService _ai;
        private readonly GameStatistics _stats;
        private readonly Stopwatch _gameTimer;
        private bool _playAgainstAI;
        private PlayerType _humanPlayerColor;
        private string _currentStyle;

        public bool IsTournamentMode { get; set; } = false;

        // Події для представлення
        public event Action StateChanged;
        public event Action<PlayerType, string> GameOver;
        public event Action<string> StyleChanged;
        public event Action<PlayerType> HumanColorChanged;

        /// <summary>
        /// Доступ до статистики для діалогів.
        /// </summary>
        public GameStatistics GetStatistics() => _stats;

        public GameController(
            GameSaver saver,
            GameStatistics stats,
            AIService ai)
        {
            _saver = saver ?? throw new ArgumentNullException(nameof(saver));
            _stats = stats ?? throw new ArgumentNullException(nameof(stats));
            _ai = ai ?? throw new ArgumentNullException(nameof(ai));
            _gameTimer = new Stopwatch();

            _playAgainstAI = false;
            _humanPlayerColor = PlayerType.White;
            _currentStyle = "Шашки";

            NewGame(autoStart: true);
        }

        public void NewGame(bool autoStart = false)
        {
            if (!autoStart)
            {
                using (var settingsForm = new GameSettingsForm())
                {
                    if (settingsForm.ShowDialog() != DialogResult.OK)
                        return;

                    _currentStyle = settingsForm.SelectedStyle;
                    StyleChanged?.Invoke(_currentStyle);

                    _playAgainstAI = settingsForm.PlayAgainstAI;
                    _humanPlayerColor = settingsForm.PlayerColor;
                    HumanColorChanged?.Invoke(_humanPlayerColor);

                    if (_playAgainstAI)
                        _ai.SetDifficulty(settingsForm.SelectedDifficulty);
                }
            }

            _game = new Game();
            _gameTimer.Restart();
            StateChanged?.Invoke();

            // Якщо AI ходить першим
            if (_playAgainstAI && _humanPlayerColor == PlayerType.Black)
                MakeAIMove();
        }

        private void MakeAIMove()
        {
            var timer = new Timer { Interval = 500 };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                var move = _ai.GetBestMove(_game);
                if (move == null) return;

                _game.MakeMove(move);
                StateChanged?.Invoke();

                if (_game.IsGameOver)
                {
                    FinalizeGameOver();
                }
                else if (_game.CurrentPlayer != _humanPlayerColor &&
                         _game.ValidMoves.Any(m => m.CapturedPiece != null))
                {
                    MakeAIMove();
                }
            };
            timer.Start();
        }

        public void HandleClick(int row, int col)
        {
            if (_game.IsGameOver) return;
            if (_playAgainstAI && _game.CurrentPlayer != _humanPlayerColor) return;

            _game.SelectPiece(row, col);
            StateChanged?.Invoke();

            if (_game.IsGameOver)
                FinalizeGameOver();
            else if (_playAgainstAI && _game.CurrentPlayer != _humanPlayerColor)
                MakeAIMove();
        }

        private void FinalizeGameOver()
        {
            _gameTimer.Stop();
            _stats.RecordGame(_game.Winner, _gameTimer.Elapsed);
            _stats.SaveToFile("stats.dat");

            
             GameOver?.Invoke(_game.Winner, _currentStyle);

            StateChanged?.Invoke();
        }

        public void SaveGame()
        {
            using (var saveDialog = new SaveFileDialog
            {
                Filter = "JSON файли (*.json)|*.json",
                Title = "Зберегти гру",
                DefaultExt = "json",
                AddExtension = true
            })
            {
                if (saveDialog.ShowDialog() == DialogResult.OK &&
                    _saver.Save(_game, saveDialog.FileName))
                {
                    MessageBox.Show("Гра успішно збережена!", "Успіх",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public void LoadGame()
        {
            using (var openDialog = new OpenFileDialog
            {
                Filter = "JSON файли (*.json)|*.json",
                Title = "Завантажити гру",
                CheckFileExists = true
            })
            {
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    var loaded = _saver.Load(openDialog.FileName);
                    if (loaded != null)
                    {
                        _game = loaded;
                        StateChanged?.Invoke();
                        MessageBox.Show("Гра успішно завантажена!", "Успіх",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        public void UndoLastMove()
        {
            if (_game == null || _game.IsGameOver) return;

            if (_playAgainstAI)
            {
                _game.Undo(); // AI
                _game.Undo(); // Player
            }
            else
            {
                _game.Undo();
            }

            StateChanged?.Invoke();
        }

        public Game GetGame() => _game;
    }
}
