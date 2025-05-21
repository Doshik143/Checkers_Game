using System;
using System.Windows.Forms;

namespace Checkers.Views
{
    public class TournamentResultsForm : Form
    {
        public TournamentResultsForm(int totalGames, int whiteWins, int blackWins)
        {
            Text = "Результати турніру";
            Width = 360;
            Height = 250;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            string winner = whiteWins > blackWins ? "Білі" :
                            blackWins > whiteWins ? "Чорні" :
                            "Нічия";

            var label = new Label
            {
                Text = $"🏁 Турнір завершено!\n\n" +
                       $"Партій: {totalGames}\n" +
                       $"Перемоги білих: {whiteWins}\n" +
                       $"Перемоги чорних: {blackWins}\n" +
                       $"Нічиїх: {totalGames - whiteWins - blackWins}\n\n" +
                       $"Переможець турніру: {winner}!",
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 140,
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Regular)
            };
            Controls.Add(label);

            var btnNewGame = new Button
            {
                Text = "Нова гра",
                DialogResult = DialogResult.Retry,
                Width = 100,
                Left = 60,
                Top = 150
            };
            Controls.Add(btnNewGame);

            var btnExit = new Button
            {
                Text = "Вийти",
                DialogResult = DialogResult.Abort,
                Width = 100,
                Left = 180,
                Top = 150
            };
            Controls.Add(btnExit);

            AcceptButton = btnNewGame;
            CancelButton = btnExit;
        }
    }
}