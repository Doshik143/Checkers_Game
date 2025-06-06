using Checkers.Commands;
using Checkers.Models;
using Checkers.Services;
using Checkers.Strategies;
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
        private readonly GameCommandInvoker _commandInvoker;
        private IGameSessionStrategy _sessionStrategy;

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
            _sessionStrategy.OnGameOver(_game.Winner, _currentStyle);
            GameOver?.Invoke(_game.Winner, _currentStyle);
            StateChanged?.Invoke();
        }

        public void SaveGame()
        {
            var command = new SaveGameCommand(_game, _saver);
            _commandInvoker.ExecuteCommand(command);
        }

        public void LoadGame()
        {
            var command = new LoadGameCommand(_saver, (loadedGame) => {
                _game = loadedGame;
                StateChanged?.Invoke();
            });
            _commandInvoker.ExecuteCommand(command);
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

        public void SetSessionStrategy(IGameSessionStrategy strategy)
        {
            _sessionStrategy = strategy;
        }

        public Game GetGame() => _game;
    }
}
