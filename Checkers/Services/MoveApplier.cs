using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Checkers.Models;

namespace Checkers.Services
{
    public class MoveApplier
    {
        public void Apply(Board board, Move move)
        {
            var piece = board.GetPiece(move.Piece.Row, move.Piece.Col);
            var captured = move.CapturedPiece != null ? board.GetPiece(move.CapturedPiece.Row, move.CapturedPiece.Col) : null;

            board.MovePiece(piece, move.ToRow, move.ToCol);
            if (captured != null)
                board.RemovePiece(captured.Row, captured.Col);
        }
    }
}

