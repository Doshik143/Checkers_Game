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
                    SelectedPiece = game.SelectedPiece,
                    ValidMoves = game.ValidMoves,
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
}