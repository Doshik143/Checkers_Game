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
            var allMoves = GetAllPossibleMoves(game.Board, game.CurrentPlayer);

            if (!allMoves.Any()) return null;

            var captureMoves = allMoves.Where(m => m.CapturedPiece != null).ToList();
            if (captureMoves.Any())
            {
                allMoves = captureMoves;
            }

            var moveSequences = new List<List<Move>>();
            foreach (var move in allMoves)
            {
                var sequence = new List<Move> { move };
                ExploreMoveSequences(game.Board.Clone(), move, sequence, moveSequences);
            }

            if (moveSequences.Any())
            {
                var bestSequence = moveSequences
                    .OrderByDescending(seq => seq.Count)
                    .ThenByDescending(seq => seq.Sum(m => m.CapturedPiece?.Type == PieceType.King ? 2 : 1))
                    .First();

                return bestSequence.First();
            }

            switch (_difficulty)
            {
                case Difficulty.Easy:
                    return GetRandomMove(allMoves);
                case Difficulty.Medium:
                    return GetMediumMove(game.Board, allMoves);
                case Difficulty.Hard:
                    return GetHardMove(game.Board, allMoves, 3);
                default:
                    return GetRandomMove(allMoves);
            }
        }

        private void ExploreMoveSequences(Board board, Move move, List<Move> currentSequence, List<List<Move>> allSequences)
        {
            var newBoard = board.Clone();
            var piece = newBoard.GetPiece(move.Piece.Row, move.Piece.Col);
            newBoard.MovePiece(piece, move.ToRow, move.ToCol);

            if (move.CapturedPiece != null)
            {
                newBoard.RemovePiece(move.CapturedPiece.Row, move.CapturedPiece.Col);
            }

            var nextMoves = newBoard.GetValidMoves(newBoard.GetPiece(move.ToRow, move.ToCol))
                .Where(m => m.CapturedPiece != null)
                .ToList();

            if (nextMoves.Any())
            {
                foreach (var nextMove in nextMoves)
                {
                    var newSequence = new List<Move>(currentSequence) { nextMove };
                    ExploreMoveSequences(newBoard, nextMove, newSequence, allSequences);
                }
            }
            else
            {
                allSequences.Add(currentSequence);
            }
        }

        private List<Move> GetAllPossibleMoves(Board board, PlayerType player)
        {
            var allMoves = new List<Move>();
            for (int row = 0; row < Board.Size; row++)
            {
                for (int col = 0; col < Board.Size; col++)
                {
                    var piece = board.GetPiece(row, col);
                    if (piece != null && piece.Player == player)
                    {
                        allMoves.AddRange(board.GetValidMoves(piece));
                    }
                }
            }
            return allMoves;
        }

        private Move GetRandomMove(List<Move> moves)
        {
            return moves.Count > 0 ? moves[_random.Next(moves.Count)] : null;
        }

        private Move GetMediumMove(Board board, List<Move> moves)
        {
            return moves.OrderByDescending(m =>
            {
                var piece = m.Piece;
                if (piece.Type == PieceType.King) return 0;

                bool willBecomeKing = (piece.Player == PlayerType.White && m.ToRow == 0) ||
                                     (piece.Player == PlayerType.Black && m.ToRow == Board.Size - 1);

                return willBecomeKing ? 2 : (m.CapturedPiece != null ? 1 : 0);
            }).FirstOrDefault();
        }

        private Move GetHardMove(Board board, List<Move> moves, int depth)
        {
            Move bestMove = null;
            int bestScore = int.MinValue;

            foreach (var move in moves)
            {
                var newBoard = board.Clone();
                var piece = newBoard.GetPiece(move.Piece.Row, move.Piece.Col);
                newBoard.MovePiece(piece, move.ToRow, move.ToCol);

                if (move.CapturedPiece != null)
                {
                    newBoard.RemovePiece(move.CapturedPiece.Row, move.CapturedPiece.Col);
                }

                int score = Minimax(newBoard, depth - 1, int.MinValue, int.MaxValue, false);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            return bestMove ?? GetMediumMove(board, moves);
        }

        private int Minimax(Board board, int depth, int alpha, int beta, bool maximizingPlayer)
        {
            if (depth == 0)
                return EvaluateBoard(board);

            var currentPlayer = maximizingPlayer ? PlayerType.Black : PlayerType.White;
            var moves = GetAllPossibleMoves(board, currentPlayer);

            var captureMoves = moves.Where(m => m.CapturedPiece != null).ToList();
            if (captureMoves.Any()) moves = captureMoves;

            if (maximizingPlayer)
            {
                int maxEval = int.MinValue;
                foreach (var move in moves)
                {
                    var newBoard = board.Clone();
                    var piece = newBoard.GetPiece(move.Piece.Row, move.Piece.Col);
                    newBoard.MovePiece(piece, move.ToRow, move.ToCol);

                    if (move.CapturedPiece != null)
                    {
                        newBoard.RemovePiece(move.CapturedPiece.Row, move.CapturedPiece.Col);
                    }

                    int eval = Minimax(newBoard, depth - 1, alpha, beta, false);
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
                    var newBoard = board.Clone();
                    var piece = newBoard.GetPiece(move.Piece.Row, move.Piece.Col);
                    newBoard.MovePiece(piece, move.ToRow, move.ToCol);

                    if (move.CapturedPiece != null)
                    {
                        newBoard.RemovePiece(move.CapturedPiece.Row, move.CapturedPiece.Col);
                    }

                    int eval = Minimax(newBoard, depth - 1, alpha, beta, true);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha) break;
                }
                return minEval;
            }
        }

        private int EvaluateBoard(Board board)
        {
            int score = 0;
            for (int row = 0; row < Board.Size; row++)
            {
                for (int col = 0; col < Board.Size; col++)
                {
                    var piece = board.GetPiece(row, col);
                    if (piece != null)
                    {
                        int pieceValue = piece.Type == PieceType.King ? 5 : 1;
                        int positionValue = GetPositionValue(row, col, piece.Player, piece.Type);

                        if (piece.Player == PlayerType.Black)
                            score += pieceValue + positionValue;
                        else
                            score -= pieceValue + positionValue;
                    }
                }
            }
            return score;
        }

        private int GetPositionValue(int row, int col, PlayerType player, PieceType type)
        {
            if (type == PieceType.King) return 0;

            return player == PlayerType.White
                ? (Board.Size - 1 - row) * 2
                : row * 2;
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