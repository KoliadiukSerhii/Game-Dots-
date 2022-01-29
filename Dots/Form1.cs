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
    public partial class Form1 : Form
    {
        private Field field = new Field();
        private CellState currentPlayer = CellState.Red;
        private const int CELL_SIZE = 39;
        
        public Form1()  
        {
            InitializeComponent();
            // щоб не мигала сітка після ходу
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e); // без базової події користувацька не прцює
            int X = (int)Math.Round(1f * e.X / CELL_SIZE); // перетворюємо пікселі в координати від 0 до <16
            int Y = (int)Math.Round(1f * e.Y / CELL_SIZE); // перше перехрестя = координати першоъ клітинки = (39,39) 
            Point point = new Point(X, Y); // створили точку
            if (field[point] == CellState.Empty) 
            {
                field.SetPoint(point, currentPlayer);
                currentPlayer = Field.Inverse(currentPlayer); // після ходу змінюємо гравця (колір наступної точки)

                Invalidate(); // Скасує певну область елемента керування та 
                             // спричиняє надсилання повідомлення про фарбування до елемента керування.
            }

            // перевірка на перемогу
            CheckWin();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // без базової події користувацька не прцює
            e.Graphics.ScaleTransform(CELL_SIZE, CELL_SIZE); // відстань між лініями сітки = cell_size (масштаб клітинок)
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality; // згладжує точки (малюються якісніше)

            // малюємо сітку
            #region grid
            Pen pen = new Pen(Color.LightGray, 0.1f); // (колір, ширина)

            for (int x = 1; x < Field.SIZE; x++)
                e.Graphics.DrawLine(pen, x, 1, x, Field.SIZE - 1);
            for (int y = 1; y < Field.SIZE; y++)
                e.Graphics.DrawLine(pen, 1, y, Field.SIZE - 1, y);
            #endregion

            // малюємо заняті точки
            foreach (Tuple<CellState, HashSet<Point>> item in field.TakenAreas)
            {
                CellState state = item.Item1; // стан клітинки поля
                HashSet<Point> area = item.Item2; // набір клітинок    HashSet - Представляє унікальний набір значень
                IEnumerable<Point> contour = field.GetContour(area); // контур замкненої області
             // Ienumerable - Надає перелічувач, який підтримує просту ітерацію над незагальною колекцією.

                // малюємо контур
                Pen contourPen = new Pen(Color.White, 0.1f); // (колір, ширина)
                SolidBrush brush = new SolidBrush(Color.White);

                contourPen.Color = StateToColor(state); // контур відповідного кольору
                brush.Color = StateToColor(state, 100); // заповнюємо прозорим кольором (насиченість = 100)
                e.Graphics.FillPolygon(brush, contour.ToArray()); // заповнюємо контур
                e.Graphics.DrawPolygon(contourPen, contour.ToArray()); // малюємо контур
            }

            // малюємо виставлені точки
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

        Color StateToColor(CellState state, byte alpha = 255) // відповідний колір
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

        //private void OnPaint(object sender, PaintEventArgs e)
        //{ }
    }
}