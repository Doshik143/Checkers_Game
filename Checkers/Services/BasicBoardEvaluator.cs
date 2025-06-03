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
        private const int KingValue = 10;
        private const int NormalPieceValue = 3;
        private const int CentralBonus = 1;
        private const int EdgePenalty = -1;
        private const int VulnerablePenalty = -4;
        private const int BoardSize = Board.Size;

        private static readonly (int Min, int Max) CentralRange = (2, 5);

        public int Evaluate(Board board, PlayerType maximizingPlayer)
        {
            int score = 0;

            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    var piece = board.GetPiece(r, c);
                    if (piece == null) continue;

                    int value = GetPieceValue(piece);
                    bool isCentral = IsCentralPosition(r, c);
                    bool isEdge = IsEdgePosition(r, c);
                    bool isVulnerable = IsVulnerable(board, piece);

                    int total = value
                                + (isCentral ? CentralBonus : 0)
                                + (isEdge ? EdgePenalty : 0)
                                + (isVulnerable ? VulnerablePenalty : 0);

                    score += piece.Player == maximizingPlayer ? total : -total;
                }
            }

            return score;
        }

        private int GetPieceValue(Piece piece)
        {
            return piece.Type == PieceType.King ? KingValue : NormalPieceValue;
        }

        private bool IsCentralPosition(int row, int col)
        {
            return row >= CentralRange.Min && row <= CentralRange.Max &&
                   col >= CentralRange.Min && col <= CentralRange.Max;
        }

        private bool IsEdgePosition(int row, int col)
        {
            return row == 0 || row == BoardSize - 1 || col == 0 || col == BoardSize - 1;
        }

        private bool IsVulnerable(Board board, Piece piece)
        {
            PlayerType opponent = piece.Player == PlayerType.White ? PlayerType.Black : PlayerType.White;
            var opponentMoves = new List<Move>();

            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    var p = board.GetPiece(r, c);
                    if (p?.Player == opponent)
                        opponentMoves.AddRange(board.GetValidMoves(p));
                }
            }

            return opponentMoves.Any(m => m.CapturedPiece?.Row == piece.Row && m.CapturedPiece?.Col == piece.Col);
        }
    }
}
