using System;
using System.Windows.Forms;
using Checkers.Models;

namespace Checkers.Views
{
    public class GameOverForm : Form
    {
        public GameOverForm(PlayerType winner, string style)
        {
            Text = "Гра завершена";
            Width = 300;
            Height = 160;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            string winnerText;

            if (style == "Хвостики")
                winnerText = winner == PlayerType.White ? "Ковбаса" : "Хвостики";
            else
                winnerText = winner == PlayerType.White ? "Білі шашки" : "Чорні шашки";

            var label = new Label
            {
                Text = $"Переможець: {winnerText}!",
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 40,
                Font = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold)
            };
            Controls.Add(label);

            var btnNewGame = new Button
            {
                Text = "Нова гра",
                DialogResult = DialogResult.Retry,
                Width = 100,
                Left = 50,
                Top = 60
            };
            Controls.Add(btnNewGame);

            var btnExit = new Button
            {
                Text = "Вийти",
                DialogResult = DialogResult.Abort,
                Width = 100,
                Left = 150,
                Top = 60
            };
            Controls.Add(btnExit);

            AcceptButton = btnNewGame;
            CancelButton = btnExit;
        }
    }
}