namespace Checkers.Models
{
    public class Piece
    {
        public PieceType Type { get; set; }
        public PlayerType Player { get; }
        public int Row { get; set; }
        public int Col { get; set; }

        public Piece(PlayerType player, int row, int col)
        {
            Player = player;
            Row = row;
            Col = col;
            Type = PieceType.Regular;
        }

        public void MakeKing()
        {
            Type = PieceType.King;
        }

        public Piece Clone()
        {
            return new Piece(Player, Row, Col)
            {
                Type = this.Type
            };
        }

        public override string ToString()
        {
            return $"{Player.ToString()[0]}{(Type == PieceType.King ? "K" : "")}";
        }
    }
}