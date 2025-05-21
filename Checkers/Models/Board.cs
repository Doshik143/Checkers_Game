using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Checkers.Models
{
    [JsonObject]
    public class Board
    {
        public const int Size = 8;

        [JsonProperty]
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
            if (piece == null || _pieces == null)
                return new List<Move>();

            var allMoves = new List<Move>();

            if (piece.Type == PieceType.Regular)
            {
                int[] rowDirections = piece.Player == PlayerType.White
                    ? new[] { -1, 1 }
                    : new[] { 1, -1 };

                foreach (int rowDir in rowDirections)
                {
                    foreach (int colDir in new[] { -1, 1 })
                    {
                        CheckSimpleMoveOrCapture(piece, rowDir, colDir, allMoves);
                    }
                }
            }
            else
            {
                foreach (int rowDir in new[] { -1, 1 })
                {
                    foreach (int colDir in new[] { -1, 1 })
                    {
                        CheckKingMovesInDirection(piece, rowDir, colDir, allMoves);
                    }
                }
            }

            bool hasAnyCaptures = HasAnyCaptures(piece.Player);

            if (hasAnyCaptures)
            {
                return allMoves.Where(m => m.CapturedPiece != null).ToList();
            }

            return allMoves;
        }

        private bool HasAnyCaptures(PlayerType player)
        {
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    var piece = GetPiece(row, col);
                    if (piece != null && piece.Player == player)
                    {
                        var moves = GetRawMovesForPiece(piece);
                        if (moves.Any(m => m.CapturedPiece != null))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private List<Move> GetRawMovesForPiece(Piece piece)
        {
            var moves = new List<Move>();

            if (piece.Type == PieceType.Regular)
            {
                int[] rowDirections = { -1, 1 };
                foreach (int rowDir in rowDirections)
                {
                    foreach (int colDir in new[] { -1, 1 })
                    {
                        CheckSimpleMoveOrCapture(piece, rowDir, colDir, moves);
                    }
                }
            }
            else
            {
                foreach (int rowDir in new[] { -1, 1 })
                {
                    foreach (int colDir in new[] { -1, 1 })
                    {
                        CheckKingMovesInDirection(piece, rowDir, colDir, moves);
                    }
                }
            }

            return moves;
        }

        private void CheckSimpleMoveOrCapture(Piece piece, int rowDir, int colDir, List<Move> moves)
        {
            int toRow = piece.Row + rowDir;
            int toCol = piece.Col + colDir;

            if (toRow < 0 || toRow >= Size || toCol < 0 || toCol >= Size)
                return;

            Piece targetPiece = GetPiece(toRow, toCol);

            if (targetPiece == null)
            {
                if ((piece.Player == PlayerType.White && rowDir == -1) ||
                    (piece.Player == PlayerType.Black && rowDir == 1))
                {
                    moves.Add(new Move(piece, toRow, toCol));
                }
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

        private void CheckKingMovesInDirection(Piece king, int rowDir, int colDir, List<Move> moves)
        {
            int currentRow = king.Row + rowDir;
            int currentCol = king.Col + colDir;
            Piece firstEnemy = null;
            int enemyRow = -1, enemyCol = -1;
            bool hasCaptured = false;

            while (currentRow >= 0 && currentRow < Size && currentCol >= 0 && currentCol < Size)
            {
                Piece currentPiece = GetPiece(currentRow, currentCol);

                if (currentPiece == null)
                {
                    if (firstEnemy == null && !hasCaptured)
                    {
                        moves.Add(new Move(king, currentRow, currentCol));
                    }
                    else if (firstEnemy != null && !hasCaptured)
                    {
                        moves.Add(new Move(king, currentRow, currentCol, firstEnemy));
                        hasCaptured = true;
                    }
                    else if (hasCaptured)
                    {
                        moves.Add(new Move(king, currentRow, currentCol, firstEnemy));
                    }
                }
                else if (currentPiece.Player == king.Player)
                {
                    break;
                }
                else
                {
                    if (firstEnemy == null)
                    {
                        firstEnemy = currentPiece;
                        enemyRow = currentRow;
                        enemyCol = currentCol;
                    }
                    else
                    {
                        break;
                    }
                }

                currentRow += rowDir;
                currentCol += colDir;
            }

            if (firstEnemy != null && !hasCaptured &&
                !moves.Any(m => m.CapturedPiece == firstEnemy))
            {
                int jumpRow = enemyRow + rowDir;
                int jumpCol = enemyCol + colDir;

                if (jumpRow >= 0 && jumpRow < Size && jumpCol >= 0 && jumpCol < Size &&
                    GetPiece(jumpRow, jumpCol) == null)
                {
                    moves.Add(new Move(king, jumpRow, jumpCol, firstEnemy));
                }
            }
        }

        public Board(bool initialize = true)
        {
            if (initialize)
                InitializeBoard();
        }

        public Board Clone()
        {
            var newBoard = new Board(false);
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