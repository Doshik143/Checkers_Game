using System;
using System.Windows.Forms;

namespace Checkers.Views
{
    public class TournamentForm : Form
    {
        private NumericUpDown numGames;
        private Button btnStart;
        public int GamesCount => (int)numGames.Value;

        public TournamentForm()
        {
            Text = "Налаштування турніру";
            Width = 300;
            Height = 150;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            var label = new Label
            {
                Text = "Кількість партій:",
                Top = 20,
                Left = 30,
                Width = 200
            };
            Controls.Add(label);

            numGames = new NumericUpDown
            {
                Top = 50,
                Left = 30,
                Width = 100,
                Minimum = 2,
                Maximum = 100,
                Value = 5
            };
            Controls.Add(numGames);

            btnStart = new Button
            {
                Text = "Почати",
                DialogResult = DialogResult.OK,
                Top = 80,
                Left = 100
            };
            Controls.Add(btnStart);
        }
    }
}