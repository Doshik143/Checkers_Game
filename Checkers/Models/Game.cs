using System;
using System.Collections.Generic;

namespace Checkers.Models
{
    public class Game
    {
        public Board Board { get; set; }
        public PlayerType CurrentPlayer { get; set; }
        public Piece SelectedPiece { get; set; }
        public List<Move> ValidMoves { get; set; }
        public bool IsGameOver { get; set; }
        public PlayerType Winner { get; set; }
        public DateTime StartTime { get; private set; }
        private Stack<GameState> _history;

        public Game()
        {
            StartTime = DateTime.Now;
            InitializeGame();
        }

        private void InitializeGame()
        {
            Board = new Board();
            CurrentPlayer = PlayerType.White;
            SelectedPiece = null;
            ValidMoves = new List<Move>();
            IsGameOver = false;
            Winner = PlayerType.None;
            StartTime = DateTime.Now;
            _history = new Stack<GameState>();
            SaveState();
        }

        public void SelectPiece(int row, int col)
        {
            if (IsGameOver) return;

            var piece = Board.GetPiece(row, col);

            if (piece != null && piece.Player == CurrentPlayer)
            {
                SelectedPiece = piece;
                ValidMoves = Board.GetValidMoves(piece);
                return;
            }

            if (SelectedPiece != null)
            {
                foreach (var move in ValidMoves)
                {
                    if (move.ToRow == row && move.ToCol == col)
                    {
                        MakeMove(move);
                        break;
                    }
                }
            }
        }

        public void MakeMove(Move move)
        {
            if (IsGameOver) return;

            Board.MovePiece(move.Piece, move.ToRow, move.ToCol);

            if (move.CapturedPiece != null)
            {
                Board.RemovePiece(move.CapturedPiece.Row, move.CapturedPiece.Col);
            }

            bool canCaptureAgain = false;
            if (move.CapturedPiece != null)
            {
                var nextCaptures = Board.GetValidMoves(move.Piece)
                    .FindAll(m => m.CapturedPiece != null);
                canCaptureAgain = nextCaptures.Count > 0;
            }

            if (!canCaptureAgain)
            {
                CurrentPlayer = CurrentPlayer == PlayerType.White ? PlayerType.Black : PlayerType.White;
                SelectedPiece = null;
                ValidMoves.Clear();
            }
            else
            {
                SelectedPiece = move.Piece;
                ValidMoves = Board.GetValidMoves(SelectedPiece)
                    .FindAll(m => m.CapturedPiece != null);
            }

            CheckGameOver();
            SaveState();
        }

        public void Undo()
        {
            if (_history.Count <= 1) return;

            _history.Pop();
            LoadState(_history.Peek());
        }

        private void SaveState()
        {
            _history.Push(new GameState
            {
                Board = Board.Clone(),
                CurrentPlayer = CurrentPlayer,
                SelectedPiece = SelectedPiece?.Clone(),
                ValidMoves = new List<Move>(ValidMoves),
                IsGameOver = IsGameOver,
                Winner = Winner
            });
        }

        private void LoadState(GameState state)
        {
            Board = state.Board.Clone();
            CurrentPlayer = state.CurrentPlayer;
            SelectedPiece = state.SelectedPiece;
            ValidMoves = new List<Move>();
            foreach (var move in state.ValidMoves)
            {
                var piece = Board.GetPiece(move.Piece.Row, move.Piece.Col);
                var captured = move.CapturedPiece != null
                    ? Board.GetPiece(move.CapturedPiece.Row, move.CapturedPiece.Col)
                    : null;

                ValidMoves.Add(new Move(piece, move.ToRow, move.ToCol, captured));
            }
            IsGameOver = state.IsGameOver;
            Winner = state.Winner;
        }

        private void CheckGameOver()
        {
            bool whiteHasMoves = PlayerHasValidMoves(PlayerType.White);
            bool blackHasMoves = PlayerHasValidMoves(PlayerType.Black);

            if (!whiteHasMoves || CountPieces(PlayerType.White) == 0)
            {
                IsGameOver = true;
                Winner = PlayerType.Black;
            }
            else if (!blackHasMoves || CountPieces(PlayerType.Black) == 0)
            {
                IsGameOver = true;
                Winner = PlayerType.White;
            }
        }

        private bool PlayerHasValidMoves(PlayerType player)
        {
            for (int row = 0; row < Board.Size; row++)
            {
                for (int col = 0; col < Board.Size; col++)
                {
                    var piece = Board.GetPiece(row, col);
                    if (piece?.Player == player && Board.GetValidMoves(piece).Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private int CountPieces(PlayerType player)
        {
            int count = 0;
            for (int row = 0; row < Board.Size; row++)
            {
                for (int col = 0; col < Board.Size; col++)
                {
                    var piece = Board.GetPiece(row, col);
                    if (piece?.Player == player) count++;
                }
            }
            return count;
        }
    }
}