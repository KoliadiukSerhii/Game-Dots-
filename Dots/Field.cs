using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using System.Text;

namespace Dots
{
    // можливі стану комірок поля
    enum CellState
    {
        Empty, Red, Blue, OutOfField
    }

    /// Поле гри
    class Field
    {
        public const int SIZE = 16;  // розмір поля 
        public int RedCapturedDots = 0, BlueCapturedDots = 0, numberOfCapturedDotsToWin = 5; // захоплені точки, виграшна кількість захоплених
        public CellState[,] cells = new CellState[SIZE, SIZE]; // двовимірний масив комірок 
        public List<Tuple<CellState, HashSet<Point>>> TakenAreas = new List<Tuple<CellState, HashSet<Point>>>(); // виставлені точки

        public CellState this[Point point] // cell state: if out of field // індексатор CellState - тип об'єета який присвоєюмо і 
        {                                              // повертаємо [Point poit] - спосіб доступу до елементів
            get
            {
                if (point.X < 0 || point.X >= SIZE || point.Y < 0 || point.Y >= SIZE)
                    return CellState.OutOfField;

                return cells[point.X, point.Y];
            }

            set { cells[point.X, point.Y] = value; }
        }

        public IEnumerable<Point> GetNeighbors4(Point point) // 4 сусідні точки
        {
            yield return new Point(point.X - 1, point.Y); //   *
            yield return new Point(point.X, point.Y - 1); // * 0 *
            yield return new Point(point.X + 1, point.Y); //   *
            yield return new Point(point.X, point.Y + 1); // для итератора треба використовувати yield return замість звичайного return
        }

        public IEnumerable<Point> GetNeighbors8(Point point) // 8 сусідніх точок
        {
            yield return new Point(point.X - 1, point.Y);
            yield return new Point(point.X - 1, point.Y - 1);
            yield return new Point(point.X, point.Y - 1);
            yield return new Point(point.X + 1, point.Y - 1); //      * * *
                                                              //      * 0 *
            yield return new Point(point.X + 1, point.Y);     //      * * *
            yield return new Point(point.X + 1, point.Y + 1);
            yield return new Point(point.X, point.Y + 1);
            yield return new Point(point.X - 1, point.Y + 1);
        }

        // шукаємо замкнуту область
        private IEnumerable<HashSet<Point>> GetClosedArea(Point lastPoint)
        {
            CellState myState = this[lastPoint];
            foreach (Point point in GetNeighbors4(lastPoint)) // перебираємо пусті точки в окрузі і намагаємось добратися до краю поля через них
            {
                if (this[point] != myState) // звіряємо колір
                {
                    HashSet<Point> list = GetClosedArea(point, myState); // шукаємо замкнуту область
                    if (list != null) // якщо знайшли
                        yield return list; // повертаємо зайняті точки
                }
            }
        }

        // Замальовуємо область, якщо не вийшли за край поля тоді повертаємо залиту область
        private HashSet<Point> GetClosedArea(Point point, CellState myState)
        {
            // Шукаємо рекурсивним способом заливки
            Stack<Point> stack = new Stack<Point>();
            HashSet<Point> visited = new HashSet<Point>();
            stack.Push(point);
            visited.Add(point);
            while (stack.Count > 0)
            {
                Point tmpPoint = stack.Pop(); // дістаємо перший елемент
                CellState state = this[tmpPoint]; 

                if (state == CellState.OutOfField) // Якщо вийшли за межі поля то контур не замкнений, повертаємо null
                    return null;

                foreach (Point curPoint in GetNeighbors4(tmpPoint)) // рекурсивно перебираємо сусідів
                {
                    if (this[curPoint] != myState)
                    {
                        if (!visited.Contains(curPoint))
                        {
                            visited.Add(curPoint);
                            stack.Push(curPoint);
                        }
                    }
                }
            }

            return visited; // повертаємо відвідані точки
        }

        public static CellState Inverse(CellState state) // змінюємо гравця (колір наступної точки)
        {
            return state == CellState.Blue ? CellState.Red : CellState.Blue;
        }

        public void SetPoint(Point point, CellState state) // виставляємо точку на полі
        {
            this[point] = state; // колір точки

            foreach (HashSet<Point> taken in GetClosedArea(point))
            {
                TakenAreas.Add(new Tuple<CellState, HashSet<Point>>(state, taken)); // додаємо точку 

                if (state == CellState.Blue) // додаємо бали
                    RedCapturedDots++;
                else if (state == CellState.Red)
                    BlueCapturedDots++;
            }
        }

        // контур залитої області
        public IEnumerable<Point> GetContour(HashSet<Point> taken)
        {
            // шукаємо будь-яку точку з контуру
            Point start = new Point();
            foreach (Point point in taken)
            {
                foreach (Point neighborPoints in GetNeighbors4(point))
                {
                    if (!taken.Contains(neighborPoints))
                    {
                        start = neighborPoints;
                        goto next;
                    }
                }
            }
        next:

            // обходимо вздовж контура 
            yield return start;
            Point nextPoint = GetNext(start, taken);
            while (nextPoint != start)
            {
                yield return nextPoint;
                nextPoint = GetNext(nextPoint, taken);
            }


        }

        Point GetNext(Point point, HashSet<Point> taken) // щоб обійти контур шукаємо наступну точку в ньому
        {
            List<Point> temp = GetNeighbors8(point).ToList();
            List<Point> list = new List<Point>(temp);
            list.AddRange(temp);
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (!taken.Contains(list[i]) && taken.Contains(list[i + 1]))
                    return list[i];
            }

            throw new Exception("Error!");
        }
    }
}