using Checkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Services
{
    public class MediumStrategy : IAIStrategy
    {
        public Move ChooseMove(Board board, PlayerType player, List<Move> moves)
        {
            return moves.OrderByDescending(m => {
                var b2 = board.Clone();
                var p = b2.GetPiece(m.Piece.Row, m.Piece.Col);
                b2.MovePiece(p, m.ToRow, m.ToCol);
                if (m.CapturedPiece != null)
                    b2.RemovePiece(m.CapturedPiece.Row, m.CapturedPiece.Col);

                bool king = (p.Player == PlayerType.White && m.ToRow == 0)
                            || (p.Player == PlayerType.Black && m.ToRow == Board.Size - 1);
                bool vulnerable = new BasicBoardEvaluator().Evaluate(b2, player) < 0;
                int score = (king ? 3 : 0) + (m.CapturedPiece != null ? 2 : 0) + (vulnerable ? -4 : 0);
                return score;
            }).FirstOrDefault();
        }
    }
}
