using System;
using System.Windows.Forms;
using Checkers.Models;
using Checkers.Services;

namespace Checkers.Views
{
    public class GameSettingsForm : Form
    {
        private CheckBox checkHuman;
        private CheckBox checkAI;
        private RadioButton radioWhite;
        private RadioButton radioBlack;
        private RadioButton radioRandom;
        private ComboBox comboDifficulty;
        private Label labelDifficulty;
        private ComboBox comboStyle;

        public bool PlayAgainstAI => checkAI.Checked;

        public PlayerType PlayerColor
        {
            get
            {
                if (radioWhite.Checked) return PlayerType.White;
                if (radioBlack.Checked) return PlayerType.Black;
                return new Random().Next(2) == 0 ? PlayerType.White : PlayerType.Black;
            }
        }

        public AIService.Difficulty SelectedDifficulty
        {
            get
            {
                switch (comboDifficulty.SelectedIndex)
                {
                    case 0: return AIService.Difficulty.Easy;
                    case 1: return AIService.Difficulty.Medium;
                    case 2: return AIService.Difficulty.Hard;
                    case 3: return AIService.Difficulty.Pro;
                    default: return AIService.Difficulty.Medium;
                }
            }
        }

        public string SelectedStyle => comboStyle.SelectedItem.ToString();

        public GameSettingsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Налаштування гри";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ClientSize = new System.Drawing.Size(340, 340);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // --- Game Style ---
            Label labelStyle = new Label
            {
                Text = "Тип гри:",
                Left = 20,
                Top = 10,
                Width = 200
            };
            this.Controls.Add(labelStyle);

            comboStyle = new ComboBox
            {
                Left = 40,
                Top = 35,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboStyle.Items.AddRange(new string[] { "Шашки", "Хвостики" });
            comboStyle.SelectedIndex = 0;
            comboStyle.SelectedIndexChanged += (s, e) => UpdateCheckerColorOptions();
            this.Controls.Add(comboStyle);

            // --- Player Type ---
            Label labelPlayers = new Label
            {
                Text = "Оберіть гравця:",
                Left = 20,
                Top = 70,
                Width = 200
            };
            this.Controls.Add(labelPlayers);

            checkHuman = new CheckBox
            {
                Text = "Людина",
                Left = 40,
                Top = 95,
                Width = 200,
                Checked = true
            };
            checkHuman.CheckedChanged += (s, e) =>
            {
                if (checkHuman.Checked)
                    checkAI.Checked = false;
                GameModeChanged();
            };
            this.Controls.Add(checkHuman);

            checkAI = new CheckBox
            {
                Text = "Комп'ютер",
                Left = 40,
                Top = 120,
                Width = 200
            };
            checkAI.CheckedChanged += (s, e) =>
            {
                if (checkAI.Checked)
                    checkHuman.Checked = false;
                GameModeChanged();
            };
            this.Controls.Add(checkAI);

            // --- Color Of Checkers ---
            Label labelColor = new Label
            {
                Text = "Колір шашок:",
                Left = 20,
                Top = 155,
                Width = 200
            };
            this.Controls.Add(labelColor);

            radioWhite = new RadioButton
            {
                Left = 40,
                Top = 180,
                Width = 100,
                Checked = true
            };
            this.Controls.Add(radioWhite);

            radioBlack = new RadioButton
            {
                Left = 140,
                Top = 180,
                Width = 100
            };
            this.Controls.Add(radioBlack);

            radioRandom = new RadioButton
            {
                Left = 240,
                Top = 180,
                Width = 100
            };
            this.Controls.Add(radioRandom);

            // --- Complexity ---
            labelDifficulty = new Label
            {
                Text = "Складність комп'ютера:",
                Left = 20,
                Top = 215,
                Width = 200
            };
            this.Controls.Add(labelDifficulty);

            comboDifficulty = new ComboBox
            {
                Left = 40,
                Top = 240,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboDifficulty.Items.AddRange(new string[] { "Легка", "Середня", "Складна", "Профі" });
            comboDifficulty.SelectedIndex = 1;
            this.Controls.Add(comboDifficulty);

            // --- OK Button ---
            Button btnOK = new Button
            {
                Text = "OK",
                Left = 120,
                Top = 280,
                Width = 80,
                DialogResult = DialogResult.OK
            };
            this.Controls.Add(btnOK);
            this.AcceptButton = btnOK;

            UpdateCheckerColorOptions();
            GameModeChanged();
        }

        private void GameModeChanged()
        {
            bool isAI = checkAI.Checked;
            labelDifficulty.Enabled = isAI;
            comboDifficulty.Enabled = isAI;
        }

        private void UpdateCheckerColorOptions()
        {
            if (SelectedStyle == "Хвостики")
            {
                radioWhite.Text = "Ковбаса";
                radioBlack.Text = "Хвостики";
                radioRandom.Text = "Випадкові";
            }
            else
            {
                radioWhite.Text = "Білі";
                radioBlack.Text = "Чорні";
                radioRandom.Text = "Випадкові";
            }
        }
    }
}