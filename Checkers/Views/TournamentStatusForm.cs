using System.Windows.Forms;

namespace Checkers.Views
{
    public class TournamentStatusForm : Form
    {
        private Label lblGameNumber;
        private Label lblScore;
        private ProgressBar progress;

        public TournamentStatusForm()
        {
            Text = "Турнір триває...";
            Width = 300;
            Height = 160;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            lblGameNumber = new Label
            {
                Text = "Матч 1 з N",
                Top = 20,
                Left = 30,
                Width = 220
            };
            Controls.Add(lblGameNumber);

            lblScore = new Label
            {
                Text = "Білі: 0, Чорні: 0",
                Top = 50,
                Left = 30,
                Width = 220
            };
            Controls.Add(lblScore);

            progress = new ProgressBar
            {
                Top = 80,
                Left = 30,
                Width = 220,
                Height = 20,
                Minimum = 0
            };
            Controls.Add(progress);
        }

        public void UpdateStatus(int currentGame, int totalGames, int whiteWins, int blackWins)
        {
            lblGameNumber.Text = $"Матч {currentGame + 1} з {totalGames}";
            lblScore.Text = $"Білі: {whiteWins} | Чорні: {blackWins}";
            progress.Maximum = totalGames;
            progress.Value = currentGame + 1;
            Application.DoEvents();
        }
    }
}