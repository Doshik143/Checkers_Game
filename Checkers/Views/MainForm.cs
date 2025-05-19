using Checkers.Models;
using Checkers.Controllers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Checkers.Views
{
    public partial class MainForm : Form
    {
        private const int CellSize = 60;
        private const int PieceSize = 50;
        private const int PieceOffset = (CellSize - PieceSize) / 2;
        private const int CrownSize = 20;

        private readonly GameController _controller;
        private readonly Brush[] _pieceBrushes;
        private readonly Brush[] _crownBrushes;

        public MainForm()
        {
            _pieceBrushes = new Brush[]
            {
            Brushes.White,
            Brushes.Black,
            Brushes.White,
            Brushes.Black
            };

            _crownBrushes = new Brush[]
            {
            Brushes.Gold,
            Brushes.Goldenrod
            };

            _controller = new GameController(this);

            SetupForm();
            SetupMenu();
        }

        private void SetupForm()
        {
            ClientSize = new Size(Board.Size * CellSize, Board.Size * CellSize);
            Text = "Шашки";
            DoubleBuffered = true;
            BackColor = Color.White;

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
        }

        private void SetupMenu()
        {
            var menu = new MenuStrip();

            var gameMenu = new ToolStripMenuItem("Гра");
            gameMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("Нова гра", null, (s, e) => _controller.NewGame()),
                new ToolStripMenuItem("Зберегти", null, (s, e) => _controller.SaveGame()),
                new ToolStripMenuItem("Завантажити", null, (s, e) => _controller.LoadGame()),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Статистика", null, (s, e) => ShowStatistics()),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Вихід", null, (s, e) => Close())
            });

            menu.Items.Add(gameMenu);
            Controls.Add(menu);
        }

        private void ShowStatistics()
        {
            var stats = _controller.GetStatistics();
            MessageBox.Show(stats.ToString(), "Статистика ігор",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawBoard(e.Graphics);
            DrawPieces(e.Graphics);
            DrawValidMoves(e.Graphics);
            DrawGameStatus(e.Graphics);
        }

        private void DrawBoard(Graphics g)
        {
            for (int row = 0; row < Board.Size; row++)
            {
                for (int col = 0; col < Board.Size; col++)
                {
                    Brush brush = (row + col) % 2 == 0 ? Brushes.White : Brushes.SaddleBrown;
                    g.FillRectangle(brush, col * CellSize, row * CellSize, CellSize, CellSize);
                }
            }
        }

        private void DrawPieces(Graphics g)
        {
            var game = _controller.GetGame();
            for (int row = 0; row < Board.Size; row++)
            {
                for (int col = 0; col < Board.Size; col++)
                {
                    var piece = game.Board.GetPiece(row, col);
                    if (piece != null)
                    {
                        int brushIndex = (piece.Player == PlayerType.White ? 0 : 1) +
                                        (piece.Type == PieceType.King ? 2 : 0);

                        int x = col * CellSize + PieceOffset;
                        int y = row * CellSize + PieceOffset;

                        //Drawing a checker
                        g.FillEllipse(_pieceBrushes[brushIndex], x, y, PieceSize, PieceSize);
                        g.DrawEllipse(Pens.Black, x, y, PieceSize, PieceSize);

                        //Drawing a crown
                        if (piece.Type == PieceType.King)
                        {
                            int crownX = x + (PieceSize - CrownSize) / 2;
                            int crownY = y + (PieceSize - CrownSize) / 2;
                            int crownBrushIndex = piece.Player == PlayerType.White ? 0 : 1;

                            Point[] crownPoints = new Point[]
                            {
                                new Point(crownX + CrownSize/2, crownY),
                                new Point(crownX + CrownSize, crownY + CrownSize),
                                new Point(crownX, crownY + CrownSize)
                            };

                            g.FillPolygon(_crownBrushes[crownBrushIndex], crownPoints);
                            g.DrawPolygon(Pens.Black, crownPoints);
                        }

                        //Highlighting the selected checker
                        if (piece == game.SelectedPiece)
                        {
                            g.DrawEllipse(new Pen(Color.Yellow, 3), x - 2, y - 2, PieceSize + 4, PieceSize + 4);
                        }
                    }
                }
            }
        }

        private void DrawValidMoves(Graphics g)
        {
            var game = _controller.GetGame();
            foreach (var move in game.ValidMoves)
            {
                int centerX = move.ToCol * CellSize + CellSize / 2;
                int centerY = move.ToRow * CellSize + CellSize / 2;
                int markerSize = 10;

                Brush brush = move.CapturedPiece != null ? Brushes.Red : Brushes.LimeGreen;
                g.FillEllipse(brush,
                    centerX - markerSize,
                    centerY - markerSize,
                    markerSize * 2,
                    markerSize * 2);
            }
        }

        private void DrawGameStatus(Graphics g)
        {
            var game = _controller.GetGame();
            string status = $"Хід: {(game.CurrentPlayer == PlayerType.White ? "Білі" : "Чорні")}";

            if (game.IsGameOver)
            {
                status = $"Гра завершена! Перемогли {(game.Winner == PlayerType.White ? "Білі" : "Чорні")} шашки!";
                var font = new Font("Arial", 14, FontStyle.Bold);
                var size = g.MeasureString(status, font);
                g.DrawString(status, font, Brushes.Black,
                    (ClientSize.Width - size.Width) / 2,
                    (ClientSize.Height - size.Height) / 2);
            }
            else
            {
                g.DrawString(status, Font, Brushes.Black, 10, 10);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            int row = e.Y / CellSize;
            int col = e.X / CellSize;

            if (row >= 0 && row < Board.Size && col >= 0 && col < Board.Size)
            {
                _controller.HandleClick(row, col);
                Invalidate();
            }
        }

        public void UpdateGameState()
        {
            Invalidate();
        }

        public void ShowGameOver(PlayerType winner)
        {
            MessageBox.Show($"Гра завершена! Перемігли {(winner == PlayerType.White ? "білі" : "чорні")} шашки!",
                "Гра закінчена",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainForm";
            this.ResumeLayout(false);

        }
    }
}