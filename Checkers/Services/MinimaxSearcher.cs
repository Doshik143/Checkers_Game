using Checkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Services
{
    public class MinimaxSearcher
    {
        private readonly IBoardEvaluator _evaluator;
        private readonly MoveApplier _moveApplier;

        public MinimaxSearcher(IBoardEvaluator evaluator, MoveApplier moveApplier)
        {
            _evaluator = evaluator;
            _moveApplier = moveApplier;
        }

        public Move FindBestMove(Board board, PlayerType player, List<Move> moves, int depth)
        {
            Move best = null;
            int bestScore = int.MinValue;

            foreach (var m in moves)
            {
                var b2 = board.Clone();
                _moveApplier.Apply(b2, m);

                int score = Minimax(b2, depth - 1, int.MinValue, int.MaxValue, player != PlayerType.Black);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = m;
                }
            }

            return best;
        }

        private int Minimax(Board board, int depth, int alpha, int beta, bool maximizing)
        {
            if (depth == 0)
                return _evaluator.Evaluate(board, maximizing ? PlayerType.Black : PlayerType.White);

            var player = maximizing ? PlayerType.Black : PlayerType.White;
            var moves = GetMoves(board, player);
            int value;

            if (maximizing)
            {
                value = int.MinValue;
                foreach (var m in moves)
                {
                    var b2 = board.Clone();
                    _moveApplier.Apply(b2, m);
                    value = Math.Max(value, Minimax(b2, depth - 1, alpha, beta, false));
                    alpha = Math.Max(alpha, value);
                    if (alpha >= beta) break;
                }
            }
            else
            {
                value = int.MaxValue;
                foreach (var m in moves)
                {
                    var b2 = board.Clone();
                    _moveApplier.Apply(b2, m);
                    value = Math.Min(value, Minimax(b2, depth - 1, alpha, beta, true));
                    beta = Math.Min(beta, value);
                    if (beta <= alpha) break;
                }
            }

            return value;
        }

        private List<Move> GetMoves(Board board, PlayerType p)
        {
            var all = new List<Move>();
            for (int r = 0; r < Board.Size; r++)
                for (int c = 0; c < Board.Size; c++)
                {
                    var pc = board.GetPiece(r, c);
                    if (pc?.Player == p)
                        all.AddRange(board.GetValidMoves(pc));
                }
            var caps = all.Where(m => m.CapturedPiece != null).ToList();
            return caps.Any() ? caps : all;
        }
    }
}
