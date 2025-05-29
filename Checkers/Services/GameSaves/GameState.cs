using Checkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Services
{
    public class GameState
    {
        public Board Board { get; set; }
        public PlayerType CurrentPlayer { get; set; }
        public PieceInfo SelectedPiece { get; set; }
        public List<MoveInfo> ValidMoves { get; set; }
        public bool IsGameOver { get; set; }
        public PlayerType Winner { get; set; }
    }
}
