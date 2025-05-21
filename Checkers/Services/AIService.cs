using Checkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Checkers.Services
{
    public class AIService
    {
        public enum Difficulty { Easy, Medium, Hard, Pro }

        private readonly Random _random = new Random();
        private Difficulty _difficulty = Difficulty.Medium;

        public void SetDifficulty(Difficulty difficulty) => _difficulty = difficulty;

        public Move GetBestMove(Game game)
        {
            var clone = game.Board.Clone();
            var allMoves = GetAllPossibleMoves(clone, game.CurrentPlayer);

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
                case Difficulty.Pro:
                    return GetHardMove(game.Board, allMoves, 6);
                default:
                    return GetRandomMove(allMoves);
            }
        }

        private void ExploreMoveSequences(Board board, Move move, List<Move> currentSequence,
                        List<List<Move>> allSequences, int depth = 0)
        {
            const int maxDepth = 10;
            if (depth > maxDepth) return;

            if (board == null || move == null || currentSequence == null || allSequences == null)
                return;

            var newBoard = board.Clone();

            var piece = newBoard.GetPiece(move.Piece.Row, move.Piece.Col);
            if (piece == null) return;

            var captured = move.CapturedPiece != null
                ? newBoard.GetPiece(move.CapturedPiece.Row, move.CapturedPiece.Col)
                : null;

            var newMove = new Move(piece, move.ToRow, move.ToCol, captured);

            newBoard.MovePiece(piece, newMove.ToRow, newMove.ToCol);
            if (captured != null)
                newBoard.RemovePiece(captured.Row, captured.Col);

            var nextMoves = newBoard.GetValidMoves(piece)
                .Where(m => m?.CapturedPiece != null)
                .ToList();

            var updatedSequence = new List<Move>(currentSequence) { newMove };

            if (nextMoves.Any())
            {
                foreach (var nextMove in nextMoves)
                {
                    ExploreMoveSequences(newBoard, nextMove, updatedSequence, allSequences, depth + 1);
                }
            }
            else
            {
                allSequences.Add(updatedSequence);
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
                var tempBoard = board.Clone();
                var piece = tempBoard.GetPiece(m.Piece.Row, m.Piece.Col);
                tempBoard.MovePiece(piece, m.ToRow, m.ToCol);

                if (m.CapturedPiece != null)
                {
                    tempBoard.RemovePiece(m.CapturedPiece.Row, m.CapturedPiece.Col);
                }

                bool becomesKing = (piece.Player == PlayerType.White && m.ToRow == 0) ||
                                   (piece.Player == PlayerType.Black && m.ToRow == Board.Size - 1);

                bool canBeCaptured = GetAllPossibleMoves(tempBoard, Opponent(piece.Player))
                    .Any(enemyMove => enemyMove.CapturedPiece != null &&
                                      enemyMove.ToRow == piece.Row && enemyMove.ToCol == piece.Col);

                int score = 0;
                if (becomesKing) score += 3;
                if (m.CapturedPiece != null) score += 2;
                if (canBeCaptured) score -= 4;

                return score;
            }).FirstOrDefault();
        }

        private PlayerType Opponent(PlayerType player) =>
            player == PlayerType.White ? PlayerType.Black : PlayerType.White;

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
                    if (piece == null) continue;

                    var captured = move.CapturedPiece != null
                        ? newBoard.GetPiece(move.CapturedPiece.Row, move.CapturedPiece.Col)
                        : null;

                    newBoard.MovePiece(piece, move.ToRow, move.ToCol);
                    if (captured != null)
                        newBoard.RemovePiece(captured.Row, captured.Col);
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
                    if (piece == null) continue;

                    var captured = move.CapturedPiece != null
                        ? newBoard.GetPiece(move.CapturedPiece.Row, move.CapturedPiece.Col)
                        : null;

                    newBoard.MovePiece(piece, move.ToRow, move.ToCol);
                    if (captured != null)
                        newBoard.RemovePiece(captured.Row, captured.Col);

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
                    if (piece == null) continue;

                    int pieceValue = piece.Type == PieceType.King ? 10 : 3;

                    bool isCentral = row >= 2 && row <= 5 && col >= 2 && col <= 5;
                    int centralBonus = isCentral ? 1 : 0;

                    bool isEdge = row == 0 || row == 7 || col == 0 || col == 7;
                    int edgePenalty = isEdge ? -1 : 0;

                    bool isVulnerable = IsVulnerable(board, piece);

                    int vulnerabilityPenalty = isVulnerable ? -4 : 0;

                    int total = pieceValue + centralBonus + edgePenalty + vulnerabilityPenalty;

                    score += piece.Player == PlayerType.Black ? total : -total;
                }
            }

            return score;
        }

        private bool IsVulnerable(Board board, Piece piece)
        {
            var opponent = piece.Player == PlayerType.White ? PlayerType.Black : PlayerType.White;
            var opponentMoves = GetAllPossibleMoves(board, opponent);

            return opponentMoves.Any(m => m.CapturedPiece != null &&
                                          m.CapturedPiece.Row == piece.Row &&
                                          m.CapturedPiece.Col == piece.Col);
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