using Checkers.Models;
using Checkers.Views;
using Checkers.Services;
using System;
using System.Windows.Forms;

namespace CheckersGame.Controllers
{
    public class GameController
    {
        private readonly MainForm _view;
        private Game _game;
        private readonly GameSaver _saver;
        private readonly AIService _ai;

        public GameController(MainForm view)
        {
            _view = view;
            _saver = new GameSaver();
            _ai = new AIService();
            NewGame();
        }

        public void NewGame()
        {
            _game = new Game();
            _view.UpdateGameState();
        }

        public void HandleClick(int row, int col)
        {
            if (_game.IsGameOver) return;

            _game.SelectPiece(row, col);
            _view.UpdateGameState();

            if (_game.CurrentPlayer == PlayerType.Black && !_game.IsGameOver)
            {
                var move = _ai.GetBestMove(_game);
                if (move != null)
                {
                    _game.MakeMove(move);
                    _view.UpdateGameState();
                }
            }

            if (_game.IsGameOver)
            {
                _view.ShowGameOver(_game.Winner);
            }
        }

        public void SaveGame()
        {
            var dialog = new SaveFileDialog { Filter = "Файли шашок (*.chk)|*.chk" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _saver.Save(_game, dialog.FileName);
            }
        }

        public void LoadGame()
        {
            var dialog = new OpenFileDialog { Filter = "Файли шашок (*.chk)|*.chk" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _game = _saver.Load(dialog.FileName) ?? new Game();
                _view.UpdateGameState();
            }
        }

        public Game GetGame() => _game;
    }
}