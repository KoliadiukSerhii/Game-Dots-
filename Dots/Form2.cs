using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dots
{
    public partial class Form2 : Form
    {
        private Field field = new Field();
        private CellState currentPlayer = CellState.Red;
        private const int CELL_SIZE = 39;
        public Form2()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            int X = (int)Math.Round(1f * e.X / CELL_SIZE);
            int Y = (int)Math.Round(1f * e.Y / CELL_SIZE);
            Point point = new Point(X, Y);
            if (field[point] == CellState.Empty)
            {
                field.SetPoint(point, currentPlayer);
                currentPlayer = Field.Inverse(currentPlayer);
                BotMove(point);
                Invalidate();
            }
            CheckWin();
        }

        public void BotMove(Point enemyPoint)
        {
            Random rand = new Random();
            int X, Y;

            foreach (Point point in field.GetNeighbors8(enemyPoint))
            {
                if (field[point] == CellState.Empty)
                {
                    field.SetPoint(point, currentPlayer);
                    goto next;
                }
            }

        repeat:
            X = rand.Next(1, 16);
            Y = rand.Next(1, 16);
            Point point0 = new Point(X, Y);
            if (field[point0] == CellState.Empty)
            {
                field.SetPoint(point0, currentPlayer);
                goto next;
            }
            else
                goto repeat;

        next:
            currentPlayer = Field.Inverse(currentPlayer);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.ScaleTransform(CELL_SIZE, CELL_SIZE);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            Pen pen = new Pen(Color.LightGray, 0.1f);

            for (int x = 1; x < Field.SIZE; x++)
                e.Graphics.DrawLine(pen, x, 1, x, Field.SIZE - 1);
            for (int y = 1; y < Field.SIZE; y++)
                e.Graphics.DrawLine(pen, 1, y, Field.SIZE - 1, y);

            foreach (Tuple<CellState, HashSet<Point>> item in field.TakenAreas)
            {
                CellState state = item.Item1;
                HashSet<Point> area = item.Item2;
                IEnumerable<Point> contour = field.GetContour(area);

                Pen contourPen = new Pen(Color.White, 0.1f);
                SolidBrush brush = new SolidBrush(Color.White);

                contourPen.Color = StateToColor(state);
                brush.Color = StateToColor(state, 100);
                e.Graphics.FillPolygon(brush, contour.ToArray());
                e.Graphics.DrawPolygon(contourPen, contour.ToArray());
            }

            SolidBrush newBrush = new SolidBrush(Color.White);
            for (int x = 0; x < Field.SIZE; x++)
            {
                for (int y = 0; y < Field.SIZE; y++)
                {
                    Point point = new Point(x, y);
                    CellState cell = field[point];

                    if (cell != CellState.Empty)
                    {
                        newBrush.Color = StateToColor(cell);
                        e.Graphics.FillEllipse(newBrush, x - 0.25f, y - 0.25f, 0.5f, 0.5f);
                    }
                }
            }

            label1.Text = "Red captured areas: " + field.BlueCapturedDots;
            label2.Text = "Blue captured areas: " + field.RedCapturedDots;
        }

        public void CheckWin()
        {
            if ((field.BlueCapturedDots == field.numberOfCapturedDotsToWin) && (field.RedCapturedDots == field.numberOfCapturedDotsToWin))
            {
                MessageBox.Show("Draw!");
                Hide();
                Form3 mainMenu = new Form3();
                mainMenu.ShowDialog();
                Close();
            }
            else if (field.RedCapturedDots == field.numberOfCapturedDotsToWin)
            {
                MessageBox.Show("Blue Player Win!");
                Hide();
                Form3 mainMenu = new Form3();
                mainMenu.ShowDialog();
                Close();
            }
            else if (field.BlueCapturedDots == field.numberOfCapturedDotsToWin)
            {
                MessageBox.Show("Red Player Win!");
                Hide();
                Form3 mainMenu = new Form3();
                mainMenu.ShowDialog();
                Close();
            }
        }

        Color StateToColor(CellState state, byte alpha = 255)
        {
            Color res = state == CellState.Blue ? Color.Blue : Color.Red;
            return Color.FromArgb(alpha, res);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
            Form3 mainMenu = new Form3();
            mainMenu.ShowDialog();
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void BotMoveSecondVariant(int xByMouse, int yByMouse)
        {
            bool move = false;
            Random rand = new Random();
            int tmp, X, Y;

            do
            {
                tmp = rand.Next(0, 4);
                switch (tmp)
                {
                    case 0:
                        X = xByMouse - 1;
                        Y = yByMouse;
                        Point point0 = new Point(X, Y);
                        if (field[point0] == CellState.Empty)
                        {
                            field.SetPoint(point0, currentPlayer);
                            currentPlayer = Field.Inverse(currentPlayer);
                            move = true;
                            Invalidate();
                        }
                        break;
                    case 1:
                        X = xByMouse + 1;
                        Y = yByMouse;
                        Point point1 = new Point(X, Y);
                        if (field[point1] == CellState.Empty)
                        {
                            field.SetPoint(point1, currentPlayer);
                            currentPlayer = Field.Inverse(currentPlayer);
                            move = true;
                            Invalidate();
                        }
                        break;
                    case 2:
                        X = xByMouse;
                        Y = yByMouse - 1;
                        Point point2 = new Point(X, Y);
                        if (field[point2] == CellState.Empty)
                        {
                            field.SetPoint(point2, currentPlayer);
                            currentPlayer = Field.Inverse(currentPlayer);
                            move = true;
                            Invalidate();
                        }
                        break;
                    case 3:
                        X = xByMouse;
                        Y = yByMouse + 1;
                        Point point3 = new Point(X, Y);
                        if (field[point3] == CellState.Empty)
                        {
                            field.SetPoint(point3, currentPlayer);
                            currentPlayer = Field.Inverse(currentPlayer);
                            move = true;
                            Invalidate();
                        }
                        break;
                }
            } while (move == false);
        }
    }
}