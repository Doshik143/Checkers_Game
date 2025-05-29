using Checkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Services
{
    public class PieceInfo
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public PlayerType Player { get; set; }
        public PieceType Type { get; set; }

        public PieceInfo() { }
        public PieceInfo(Piece piece)
        {
            Row = piece.Row;
            Col = piece.Col;
            Player = piece.Player;
            Type = piece.Type;
        }
    }
}
