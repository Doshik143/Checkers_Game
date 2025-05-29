using Checkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Services
{
    public class MoveInfo
    {
        public int PieceRow { get; set; }
        public int PieceCol { get; set; }
        public int ToRow { get; set; }
        public int ToCol { get; set; }
        public int CapturedPieceRow { get; set; } = -1;
        public int CapturedPieceCol { get; set; } = -1;

        public MoveInfo() { }
        public MoveInfo(Move move)
        {
            PieceRow = move.Piece.Row;
            PieceCol = move.Piece.Col;
            ToRow = move.ToRow;
            ToCol = move.ToCol;
            if (move.CapturedPiece != null)
            {
                CapturedPieceRow = move.CapturedPiece.Row;
                CapturedPieceCol = move.CapturedPiece.Col;
            }
        }
    }
}
