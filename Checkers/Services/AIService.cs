using Checkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Checkers.Services
{
    public class AIService
    {
        public enum Difficulty { Easy, Medium, Hard }

        private readonly Random _random = new Random();
        private Difficulty _difficulty = Difficulty.Medium;

        public void SetDifficulty(Difficulty difficulty) => _difficulty = difficulty;

        public Move GetBestMove(Game game)
        {
            switch (_difficulty)
            {
                case Difficulty.Easy:
                    return GetRandomMove(game);
                case Difficulty.Medium:
                    return GetMediumMove(game);
                case Difficulty.Hard:
                    return GetHardMove(game, 3);
                default:
                    return GetRandomMove(game);
            }
        }

        private Move GetRandomMove(Game game)
        {
            var moves = GetAllValidMoves(game.Board, game.CurrentPlayer);
            return moves.Count > 0 ? moves[_random.Next(moves.Count)] : null;
        }

        private Move GetMediumMove(Game game)
        {
            var moves = GetAllValidMoves(game.Board, game.CurrentPlayer);
            var captures = moves.Where(m => m.CapturedPiece != null).ToList();

            if (captures.Any())
            {
                return captures.OrderByDescending(m =>
                    m.CapturedPiece.Type == PieceType.King ? 3 : 1)
                    .First();
            }
            return GetRandomMove(game);
        }

        private Move GetHardMove(Game game, int depth)
        {
            var moves = GetAllValidMoves(game.Board, game.CurrentPlayer);
            Move bestMove = null;
            int bestScore = int.MinValue;

            foreach (var move in moves)
            {
                var newBoard = game.Board.Clone();
                var newGame = new Game { Board = newBoard, CurrentPlayer = game.CurrentPlayer };
                newGame.MakeMove(move);

                int score = Minimax(newGame, depth - 1, int.MinValue, int.MaxValue, false);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            return bestMove ?? GetMediumMove(game);
        }

        private int Minimax(Game game, int depth, int alpha, int beta, bool maximizingPlayer)
        {
            if (depth == 0 || game.IsGameOver)
                return EvaluateBoard(game.Board, maximizingPlayer ? PlayerType.Black : PlayerType.White);

            var moves = GetAllValidMoves(game.Board, game.CurrentPlayer);

            if (maximizingPlayer)
            {
                int maxEval = int.MinValue;
                foreach (var move in moves)
                {
                    var newBoard = game.Board.Clone();
                    var newGame = new Game { Board = newBoard, CurrentPlayer = game.CurrentPlayer };
                    newGame.MakeMove(move);

                    int eval = Minimax(newGame, depth - 1, alpha, beta, false);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha) break;
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (var move in moves)
                {
                    var newBoard = game.Board.Clone();
                    var newGame = new Game { Board = newBoard, CurrentPlayer = game.CurrentPlayer };
                    newGame.MakeMove(move);

                    int eval = Minimax(newGame, depth - 1, alpha, beta, true);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha) break;
                }
                return minEval;
            }
        }

        private int EvaluateBoard(Board board, PlayerType player)
        {
            int score = 0;
            for (int row = 0; row < Board.Size; row++)
            {
                for (int col = 0; col < Board.Size; col++)
                {
                    var piece = board.GetPiece(row, col);
                    if (piece != null)
                    {
                        int pieceValue = piece.Type == PieceType.King ? 3 : 1;
                        int positionValue = piece.Player == PlayerType.Black ? row : (Board.Size - 1 - row);

                        if (piece.Player == player)
                            score += pieceValue + positionValue / 2;
                        else
                            score -= pieceValue + positionValue / 2;
                    }
                }
            }
            return score;
        }

        private List<Move> GetAllValidMoves(Board board, PlayerType player)
        {
            var moves = new List<Move>();
            for (int row = 0; row < Board.Size; row++)
            {
                for (int col = 0; col < Board.Size; col++)
                {
                    var piece = board.GetPiece(row, col);
                    if (piece?.Player == player)
                    {
                        moves.AddRange(board.GetValidMoves(piece));
                    }
                }
            }
            return moves;
        }
    }
}