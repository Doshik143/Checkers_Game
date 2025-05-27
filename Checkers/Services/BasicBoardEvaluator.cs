using Checkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Services
{
    public class BasicBoardEvaluator : IBoardEvaluator
    {
        public int Evaluate(Board board, PlayerType maximizingPlayer)
        {
            int score = 0;
            for (int r = 0; r < Board.Size; r++)
                for (int c = 0; c < Board.Size; c++)
                {
                    var piece = board.GetPiece(r, c);
                    if (piece == null) continue;

                    int value = piece.Type == PieceType.King ? 10 : 3;
                    bool central = r >= 2 && r <= 5 && c >= 2 && c <= 5;
                    bool edge = r == 0 || r == 7 || c == 0 || c == 7;
                    bool vulnerable = IsVulnerable(board, piece);

                    int total = value + (central ? 1 : 0) + (edge ? -1 : 0) + (vulnerable ? -4 : 0);
                    score += piece.Player == PlayerType.Black ? total : -total;
                }
            return score;
        }

        private bool IsVulnerable(Board board, Piece piece)
        {
            var opp = piece.Player == PlayerType.White ? PlayerType.Black : PlayerType.White;
            var moves = new List<Move>();
            for (int r = 0; r < Board.Size; r++)
                for (int c = 0; c < Board.Size; c++)
                {
                    var p = board.GetPiece(r, c);
                    if (p?.Player == opp)
                        moves.AddRange(board.GetValidMoves(p));
                }
            return moves.Exists(m => m.CapturedPiece?.Row == piece.Row && m.CapturedPiece?.Col == piece.Col);
        }
    }
}
