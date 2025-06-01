namespace Checkers.Models
{
    public class Move
    {
        public Piece Piece { get; set; }
        public int ToRow { get; set; }
        public int ToCol { get; set; }
        public Piece CapturedPiece { get; set; }

        // Властивості-координати
        public int PieceRow => Piece?.Row ?? -1;
        public int PieceCol => Piece?.Col ?? -1;
        public int CapturedPieceRow => CapturedPiece?.Row ?? -1;
        public int CapturedPieceCol => CapturedPiece?.Col ?? -1;

        public Move() { }

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
