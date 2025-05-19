namespace Checkers.Models
{
    public class Move
    {
        public Piece Piece { get; }
        public int ToRow { get; }
        public int ToCol { get; }
        public Piece CapturedPiece { get; }

        public Move(Piece piece, int toRow, int toCol, Piece capturedPiece = null)
        {
            Piece = piece;
            ToRow = toRow;
            ToCol = toCol;
            CapturedPiece = capturedPiece;
        }

        public override string ToString()
        {
            return $"{Piece} -> ({ToRow},{ToCol})" +
                   (CapturedPiece != null ? $" [захоплено {CapturedPiece}]" : "");
        }
    }
}