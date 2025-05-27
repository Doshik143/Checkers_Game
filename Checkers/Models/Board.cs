using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Checkers.Models
{
    [JsonObject]
    public class Board
    {
        public const int Size = 8;
        private const int InitialBlackRows = 3;
        private const int InitialWhiteStartRow = 5;

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
                        if (row < InitialBlackRows)
                            _pieces[row, col] = new Piece(PlayerType.Black, row, col);
                        else if (row >= InitialWhiteStartRow)
                            _pieces[row, col] = new Piece(PlayerType.White, row, col);
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

            var allMoves = GetAllPossibleMoves(piece);
            bool hasAnyCaptures = HasAnyCaptures(piece.Player);

            return hasAnyCaptures
                ? allMoves.Where(m => m.CapturedPiece != null).ToList()
                : allMoves;
        }

        private List<Move> GetAllPossibleMoves(Piece piece)
        {
            var moves = new List<Move>();

            if (piece.Type == PieceType.Regular)
            {
                var directions = GetRegularPieceDirections(piece.Player);
                foreach (var (rowDir, colDir) in directions)
                {
                    CheckSimpleMoveOrCapture(piece, rowDir, colDir, moves);
                }
            }
            else
            {
                var allDirections = GetAllDirections();
                foreach (var (rowDir, colDir) in allDirections)
                {
                    CheckKingMovesInDirection(piece, rowDir, colDir, moves);
                }
            }

            return moves;
        }

        private List<(int rowDir, int colDir)> GetRegularPieceDirections(PlayerType player)
        {
            var directions = new List<(int, int)>();

            if (player == PlayerType.White)
            {
                directions.AddRange(new[] { (-1, -1), (-1, 1), (1, -1), (1, 1) });
            }
            else
            {
                directions.AddRange(new[] { (1, -1), (1, 1), (-1, -1), (-1, 1) });
            }

            return directions;
        }

        private List<(int rowDir, int colDir)> GetAllDirections()
        {
            return new List<(int, int)>
            {
                (-1, -1), (-1, 1), (1, -1), (1, 1)
            };
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
                        var moves = GetAllPossibleMoves(piece);
                        if (moves.Any(m => m.CapturedPiece != null))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void CheckSimpleMoveOrCapture(Piece piece, int rowDir, int colDir, List<Move> moves)
        {
            int toRow = piece.Row + rowDir;
            int toCol = piece.Col + colDir;

            if (!IsValidPosition(toRow, toCol))
                return;

            Piece targetPiece = GetPiece(toRow, toCol);

            if (targetPiece == null)
            {
                if (CanMoveInDirection(piece, rowDir))
                {
                    moves.Add(new Move(piece, toRow, toCol));
                }
            }
            else if (targetPiece.Player != piece.Player)
            {
                TryAddCaptureMove(piece, targetPiece, rowDir, colDir, moves);
            }
        }

        private bool CanMoveInDirection(Piece piece, int rowDir)
        {
            return (piece.Player == PlayerType.White && rowDir == -1) ||
                   (piece.Player == PlayerType.Black && rowDir == 1);
        }

        private void TryAddCaptureMove(Piece piece, Piece targetPiece, int rowDir, int colDir, List<Move> moves)
        {
            int jumpRow = targetPiece.Row + rowDir;
            int jumpCol = targetPiece.Col + colDir;

            if (IsValidPosition(jumpRow, jumpCol) && GetPiece(jumpRow, jumpCol) == null)
            {
                moves.Add(new Move(piece, jumpRow, jumpCol, targetPiece));
            }
        }

        private bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < Size && col >= 0 && col < Size;
        }

        private void CheckKingMovesInDirection(Piece king, int rowDir, int colDir, List<Move> moves)
        {
            int currentRow = king.Row + rowDir;
            int currentCol = king.Col + colDir;
            Piece firstEnemy = null;
            bool hasCaptured = false;

            while (IsValidPosition(currentRow, currentCol))
            {
                Piece currentPiece = GetPiece(currentRow, currentCol);

                if (currentPiece == null)
                {
                    ProcessEmptySquareForKing(king, currentRow, currentCol, firstEnemy, hasCaptured, moves);
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
                    }
                    else
                    {
                        break; // Друга ворожа фігура - неможливо стрибнути
                    }
                }

                currentRow += rowDir;
                currentCol += colDir;
            }
        }

        private void ProcessEmptySquareForKing(Piece king, int row, int col, Piece firstEnemy, bool hasCaptured, List<Move> moves)
        {
            if (firstEnemy == null && !hasCaptured)
            {
                moves.Add(new Move(king, row, col));
            }
            else if (firstEnemy != null)
            {
                moves.Add(new Move(king, row, col, firstEnemy));
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
            {
                for (int col = 0; col < Size; col++)
                {
                    if (_pieces[row, col] != null)
                    {
                        newBoard._pieces[row, col] = new Piece(_pieces[row, col].Player, row, col)
                        {
                            Type = _pieces[row, col].Type
                        };
                    }
                }
            }
            return newBoard;
        }
    }
}