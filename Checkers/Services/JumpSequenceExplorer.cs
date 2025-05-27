using Checkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Services
{
    public class JumpSequenceExplorer
    {
        private const int MaxDepth = 10;

        public List<List<Move>> Explore(Board board, IEnumerable<Move> initialMoves)
        {
            var sequences = new List<List<Move>>();
            foreach (var move in initialMoves)
                Explore(board.Clone(), move, new List<Move> { move }, sequences, 0);
            return sequences;
        }

        private void Explore(Board board, Move move, List<Move> sequence, List<List<Move>> result, int depth)
        {
            if (depth > MaxDepth) return;

            var b = board.Clone();
            var piece = b.GetPiece(move.Piece.Row, move.Piece.Col);
            var captured = move.CapturedPiece != null
                ? b.GetPiece(move.CapturedPiece.Row, move.CapturedPiece.Col)
                : null;

            b.MovePiece(piece, move.ToRow, move.ToCol);
            if (captured != null) b.RemovePiece(captured.Row, captured.Col);

            var next = b.GetValidMoves(piece).Where(m => m.CapturedPiece != null).ToList();
            if (next.Any())
            {
                foreach (var nm in next)
                    Explore(b, nm, new List<Move>(sequence) { nm }, result, depth + 1);
            }
            else
            {
                result.Add(sequence);
            }
        }
    }

}
