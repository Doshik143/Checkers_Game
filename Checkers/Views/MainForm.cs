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

        private PlayerType _humanPlayerColor = PlayerType.White;

        private ToolStripLabel _statusLabel;

        private string _style = "Шашки";

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

            _statusLabel = new ToolStripLabel
            {
                Text = "Хід: Білі",
                Alignment = ToolStripItemAlignment.Left,
                Margin = new Padding(150, 0, 0, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            menu.Items.Add(_statusLabel);

            var undoButton = new ToolStripMenuItem("⏪ Скасувати хід", null, (s, e) => _controller.UndoLastMove())
            {
                Alignment = ToolStripItemAlignment.Right
            };
            menu.Items.Add(undoButton);

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
            e.Graphics.Clear(BackColor);

            DrawBoard(e.Graphics);
            DrawPieces(e.Graphics);
            DrawValidMoves(e.Graphics);
        }

        private void DrawBoard(Graphics g)
        {
            for (int row = 0; row < Board.Size; row++)
            {
                for (int col = 0; col < Board.Size; col++)
                {
                    int visualRow = TransformRow(row);
                    int visualCol = TransformCol(col);

                    Brush brush = (row + col) % 2 == 0 ? Brushes.White : Brushes.SaddleBrown;
                    g.FillRectangle(brush, visualCol * CellSize, visualRow * CellSize, CellSize, CellSize);
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

                        int visualRow = TransformRow(row);
                        int visualCol = TransformCol(col);

                        int x = visualCol * CellSize + PieceOffset;
                        int y = visualRow * CellSize + PieceOffset;

                        // Drawing a checker
                        if (_style == "Хвостики")
                        {
                            if (piece.Player == PlayerType.White)
                                DrawSausage(g, x, y, PieceSize);
                            else
                                DrawCatHead(g, x, y, PieceSize);
                        }
                        else
                        {
                            g.FillEllipse(_pieceBrushes[brushIndex], x, y, PieceSize, PieceSize);
                            g.DrawEllipse(Pens.Black, x, y, PieceSize, PieceSize);
                        }

                        // Drawing a crown
                        if (piece.Type == PieceType.King)
                        {
                            int crownX = x + (PieceSize - CrownSize) / 2;
                            int crownY = y + (PieceSize - CrownSize) / 2;
                            int crownBrushIndex = piece.Player == PlayerType.White ? 0 : 1;

                            Point[] crownPoints = new Point[]
                            {
                                new Point(crownX + CrownSize / 2, crownY),
                                new Point(crownX + CrownSize, crownY + CrownSize),
                                new Point(crownX, crownY + CrownSize)
                            };

                            g.FillPolygon(_crownBrushes[crownBrushIndex], crownPoints);
                            g.DrawPolygon(Pens.Black, crownPoints);
                        }

                        // Highlighting the selected checker
                        if (piece == game.SelectedPiece)
                        {
                            g.DrawEllipse(new Pen(Color.Yellow, 3), x - 2, y - 2, PieceSize + 4, PieceSize + 4);
                        }
                    }
                }
            }
        }

        private void DrawSausage(Graphics g, int x, int y, int size)
        {
            var rect = new Rectangle(x + 5, y + size / 4, size - 10, size / 2);
            using (var brush = new SolidBrush(Color.LightPink))
            using (var pen = new Pen(Color.Brown, 2))
            {
                g.FillEllipse(brush, rect);
                g.DrawEllipse(pen, rect);
            }

            var dotBrush = Brushes.White;
            var rand = new Random();
            int dotCount = 8;
            int dotSize = 4;

            for (int i = 0; i < dotCount; i++)
            {
                int dx = rand.Next(rect.Left + 5, rect.Right - 5 - dotSize);
                int dy = rand.Next(rect.Top + 5, rect.Bottom - 5 - dotSize);

                g.FillEllipse(dotBrush, dx, dy, dotSize, dotSize);
            }
        }

        private void DrawCatHead(Graphics g, int x, int y, int size)
        {
            var headRect = new Rectangle(x + 4, y + 4, size - 8, size - 8);
            using (var headBrush = new SolidBrush(Color.Black))
            {
                g.FillEllipse(headBrush, headRect);

                Point[] leftEar = {
                    new Point(x + size / 4, y + 6),
                    new Point(x + size / 3, y - 6),
                    new Point(x + size / 2, y + 6)
                };
                Point[] rightEar = {
                    new Point(x + size / 2, y + 6),
                    new Point(x + 2 * size / 3, y - 6),
                    new Point(x + 3 * size / 4, y + 6)
                };
                g.FillPolygon(headBrush, leftEar);
                g.FillPolygon(headBrush, rightEar);
            }

            using (var eyeBrush = new SolidBrush(Color.Yellow))
            {
                g.FillEllipse(eyeBrush, x + size / 3 - 3, y + size / 3, 6, 6);
                g.FillEllipse(eyeBrush, x + 2 * size / 3 - 3, y + size / 3, 6, 6);
            }

            using (var pen = new Pen(Color.White, 1.5f))
            {
                int cx = x + size / 2;
                int cy = y + size * 2 / 3;

                Point start = new Point(cx - 6, cy);
                Point control1 = new Point(cx - 3, cy + 6);
                Point control2 = new Point(cx + 3, cy + 6);
                Point end = new Point(cx + 6, cy);

                g.DrawBezier(pen, start, control1, control2, end);
            }
        }

        private void DrawValidMoves(Graphics g)
        {
            var game = _controller.GetGame();
            foreach (var move in game.ValidMoves)
            {
                int visualRow = TransformRow(move.ToRow);
                int visualCol = TransformCol(move.ToCol);

                int centerX = visualCol * CellSize + CellSize / 2;
                int centerY = visualRow * CellSize + CellSize / 2;
                int markerSize = 10;

                Brush brush = move.CapturedPiece != null ? Brushes.Red : Brushes.LimeGreen;
                g.FillEllipse(brush,
                    centerX - markerSize,
                    centerY - markerSize,
                    markerSize * 2,
                    markerSize * 2);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            int visualRow = e.Y / CellSize;
            int visualCol = e.X / CellSize;

            int logicRow = _humanPlayerColor == PlayerType.White ? visualRow : Board.Size - 1 - visualRow;
            int logicCol = _humanPlayerColor == PlayerType.White ? visualCol : Board.Size - 1 - visualCol;

            if (logicRow >= 0 && logicRow < Board.Size && logicCol >= 0 && logicCol < Board.Size)
            {
                _controller.HandleClick(logicRow, logicCol);
                Invalidate();
            }
        }

        public void UpdateGameState()
        {
            if (_controller != null)
            {
                var game = _controller.GetGame();

                if (_statusLabel != null && game != null)
                {
                    string currentPlayerText;

                    if (_style == "Хвостики")
                    {
                        currentPlayerText = game.CurrentPlayer == PlayerType.White ? "Ковбаса" : "Хвостики";
                    }
                    else
                    {
                        currentPlayerText = game.CurrentPlayer == PlayerType.White ? "Білі" : "Чорні";
                    }

                    _statusLabel.Text = $"Хід: {currentPlayerText}";
                }
            }

            Invalidate();
        }


        public void ShowGameOver(PlayerType winner, string style)
        {
            using (var gameOverForm = new GameOverForm(winner, style))
            {
                var result = gameOverForm.ShowDialog();
                if (result == DialogResult.Retry)
                {
                    _controller.NewGame();
                }
                else if (result == DialogResult.Abort)
                {
                    Close();
                }
            }
        }

        public void SetHumanPlayerColor(PlayerType color)
        {
            _humanPlayerColor = color;
        }

        private int TransformRow(int row)
        {
            return _humanPlayerColor == PlayerType.White ? row : Board.Size - 1 - row;
        }

        private int TransformCol(int col)
        {
            return _humanPlayerColor == PlayerType.White ? col : Board.Size - 1 - col;
        }

        public void SetStyle(string style)
        {
            _style = style;
            Invalidate();
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

        public string GetCurrentStyle() => _style;
    }
}