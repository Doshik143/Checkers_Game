using Checkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Checkers.Services
{
    public class AIService
    {
        public enum Difficulty { Easy, Medium, Hard, Pro }
        private Difficulty _difficulty = Difficulty.Medium;
        private readonly Dictionary<Difficulty, IAIStrategy> _strategies;
        private readonly JumpSequenceExplorer _explorer;

        public AIService(
            Dictionary<Difficulty, IAIStrategy> strategies,
            JumpSequenceExplorer explorer)
        {
            _strategies = strategies;
            _explorer = explorer;
        }

        public void SetDifficulty(Difficulty d) => _difficulty = d;

        public Move GetBestMove(Game game)
        {
            var board = game.Board.Clone();
            var all = GetAllMoves(board, game.CurrentPlayer);
            if (!all.Any()) return null;

            var captures = all.Where(m => m.CapturedPiece != null).ToList();
            var moves = captures.Any()
                        ? _explorer.Explore(board, captures)
                           .OrderByDescending(seq => seq.Count)
                           .ThenByDescending(seq => seq.Sum(m => m.CapturedPiece.Type == PieceType.King ? 2 : 1))
                           .First()
                        : null;
            if (moves != null && moves.Any())
                return moves.First();

            return _strategies[_difficulty].ChooseMove(board, game.CurrentPlayer, captures.Any() ? captures : all);
        }

        private List<Move> GetAllMoves(Board board, PlayerType player)
        {
            var list = new List<Move>();
            for (int r = 0; r < Board.Size; r++)
                for (int c = 0; c < Board.Size; c++)
                {
                    var p = board.GetPiece(r, c);
                    if (p?.Player == player)
                        list.AddRange(board.GetValidMoves(p));
                }
            return list;
        }
    }
}