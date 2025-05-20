using Newtonsoft.Json;

namespace Checkers.Models
{
    [JsonObject]
    public class Piece
    {
        [JsonProperty]
        public PieceType Type { get; set; }

        [JsonProperty]
        public PlayerType Player { get; }

        [JsonProperty]
        public int Row { get; set; }

        [JsonProperty]
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