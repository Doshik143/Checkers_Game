using System.Collections.Generic;

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