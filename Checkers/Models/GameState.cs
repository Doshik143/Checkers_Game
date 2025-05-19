using System.Collections.Generic;

namespace Checkers.Models
{
    public class GameState
    {
        public Board Board { get; set; }
        public PlayerType CurrentPlayer { get; set; }
        public Piece SelectedPiece { get; set; }
        public List<Move> ValidMoves { get; set; }
        public bool IsGameOver { get; set; }
        public PlayerType Winner { get; set; }
        public int WhitePieces { get; set; }
        public int BlackPieces { get; set; }
        public int WhiteKings { get; set; }
        public int BlackKings { get; set; }
    }
}