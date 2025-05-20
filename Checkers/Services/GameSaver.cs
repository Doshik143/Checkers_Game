using Checkers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace Checkers.Services
{
    public class GameSaver
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public bool Save(Game game, string filePath)
        {
            try
            {
                var state = new GameState
                {
                    Board = game.Board,
                    CurrentPlayer = game.CurrentPlayer,
                    SelectedPiece = game.SelectedPiece != null ?
                        new PieceInfo(game.SelectedPiece) : null,
                    ValidMoves = game.ValidMoves.ConvertAll(m => new MoveInfo(m)),
                    IsGameOver = game.IsGameOver,
                    Winner = game.Winner
                };

                string json = JsonConvert.SerializeObject(state, _settings);
                File.WriteAllText(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public Game Load(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Файл не знайдено", "Помилка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                string json = File.ReadAllText(filePath);
                var state = JsonConvert.DeserializeObject<GameState>(json, _settings);

                var game = new Game
                {
                    Board = state.Board,
                    CurrentPlayer = state.CurrentPlayer,
                    IsGameOver = state.IsGameOver,
                    Winner = state.Winner
                };

                if (state.SelectedPiece != null)
                {
                    game.SelectedPiece = game.Board.GetPiece(
                        state.SelectedPiece.Row,
                        state.SelectedPiece.Col);
                }

                game.ValidMoves = state.ValidMoves.ConvertAll(mi =>
                    new Move(
                        game.Board.GetPiece(mi.PieceRow, mi.PieceCol),
                        mi.ToRow,
                        mi.ToCol,
                        mi.CapturedPieceRow >= 0 ?
                            game.Board.GetPiece(mi.CapturedPieceRow, mi.CapturedPieceCol) : null));

                return game;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }

    public class GameState
    {
        public Board Board { get; set; }
        public PlayerType CurrentPlayer { get; set; }
        public PieceInfo SelectedPiece { get; set; }
        public List<MoveInfo> ValidMoves { get; set; }
        public bool IsGameOver { get; set; }
        public PlayerType Winner { get; set; }
    }

    public class PieceInfo
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public PlayerType Player { get; set; }
        public PieceType Type { get; set; }

        public PieceInfo() { }
        public PieceInfo(Piece piece)
        {
            Row = piece.Row;
            Col = piece.Col;
            Player = piece.Player;
            Type = piece.Type;
        }
    }

    public class MoveInfo
    {
        public int PieceRow { get; set; }
        public int PieceCol { get; set; }
        public int ToRow { get; set; }
        public int ToCol { get; set; }
        public int CapturedPieceRow { get; set; } = -1;
        public int CapturedPieceCol { get; set; } = -1;

        public MoveInfo() { }
        public MoveInfo(Move move)
        {
            PieceRow = move.Piece.Row;
            PieceCol = move.Piece.Col;
            ToRow = move.ToRow;
            ToCol = move.ToCol;
            if (move.CapturedPiece != null)
            {
                CapturedPieceRow = move.CapturedPiece.Row;
                CapturedPieceCol = move.CapturedPiece.Col;
            }
        }
    }
}