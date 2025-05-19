using System.Collections.Generic;
using System.Linq;

namespace Checkers.Models
{
    public class Board
    {
        public const int Size = 8;
        private readonly Piece[,] _pieces = new Piece[Size, Size];

        public Board() => InitializeBoard();

        private void InitializeBoard()
        {
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    if ((row + col) % 2 != 0)
                    {
                        if (row < 3) _pieces[row, col] = new Piece(PlayerType.Black, row, col);
                        else if (row > 4) _pieces[row, col] = new Piece(PlayerType.White, row, col);
                    }
                }
            }
        }

        public Piece GetPiece(int row, int col) =>
            (row >= 0 && row < Size && col >= 0 && col < Size) ? _pieces[row, col] : null;

        public void MovePiece(Piece piece, int toRow, int toCol)
        {
            _pieces[piece.Row, piece.Col] = null;
            _pieces[toRow, toCol] = piece;
            piece.Row = toRow;
            piece.Col = toCol;

            if ((piece.Player == PlayerType.White && toRow == 0) ||
                (piece.Player == PlayerType.Black && toRow == Size - 1))
            {
                piece.MakeKing();
            }
        }

        public void RemovePiece(int row, int col) => _pieces[row, col] = null;

        public List<Move> GetValidMoves(Piece piece)
        {
            var moves = new List<Move>();
            if (piece == null) return moves;

            int[] rowDirections;
            if (piece.Type == PieceType.Regular)
            {
                rowDirections = piece.Player == PlayerType.White
                    ? new[] { -1, 1 }
                    : new[] { 1, -1 };
            }
            else
            {
                rowDirections = new[] { -1, 1 };
            }

            foreach (int rowDir in rowDirections)
            {
                foreach (int colDir in new[] { -1, 1 })
                {
                    int toRow = piece.Row + rowDir;
                    int toCol = piece.Col + colDir;

                    if (toRow < 0 || toRow >= Size || toCol < 0 || toCol >= Size)
                        continue;

                    Piece targetPiece = GetPiece(toRow, toCol);

                    if (targetPiece == null)
                    {
                        if (piece.Type == PieceType.Regular &&
                            ((piece.Player == PlayerType.White && rowDir == 1) ||
                             (piece.Player == PlayerType.Black && rowDir == -1)))
                        {
                            continue;
                        }
                        moves.Add(new Move(piece, toRow, toCol));
                    }
                    else if (targetPiece.Player != piece.Player)
                    {
                        int jumpRow = toRow + rowDir;
                        int jumpCol = toCol + colDir;

                        if (jumpRow >= 0 && jumpRow < Size && jumpCol >= 0 && jumpCol < Size &&
                            GetPiece(jumpRow, jumpCol) == null)
                        {
                            moves.Add(new Move(piece, jumpRow, jumpCol, targetPiece));
                        }
                    }
                }
            }

            var captureMoves = moves.Where(m => m.CapturedPiece != null).ToList();
            return captureMoves.Any() ? captureMoves : moves;
        }

        public Board Clone()
        {
            var newBoard = new Board();
            for (int row = 0; row < Size; row++)
                for (int col = 0; col < Size; col++)
                    if (_pieces[row, col] != null)
                        newBoard._pieces[row, col] = new Piece(_pieces[row, col].Player, row, col)
                        {
                            Type = _pieces[row, col].Type
                        };
            return newBoard;
        }
    }
}