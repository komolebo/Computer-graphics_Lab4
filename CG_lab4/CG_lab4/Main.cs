using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

struct _triangle_
{
    public Point point1, point2, point3;
} 
struct _line_
{
    public Point point1, point2;
}

namespace CG_lab4
{
    public partial class Main : Form
    {
        private Bitmap bitmap;
        private Pen pen = new Pen(Color.LightGreen, 2);
        private int xSize, ySize;        // Розміри екрану

        private short MODE = 0;          // 1 - трикутник Серпінського

        private _triangle_ triangle; 
        private _line_ line;
        public Main()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            InitializeComponent();

            labelStatus.Text = "";
            trackBarV.Value = trackBarH.Value = 2;
            radioButton2.Checked = true;

            xSize = pBox.Size.Width;
            ySize = pBox.Size.Height;
            bitmap = new Bitmap(xSize, ySize);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void drawBtn_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                int xZoom = (9 - Convert.ToInt32(trackBarH.Value)) * 100;
                int yZoom = Convert.ToInt32(trackBarV.Value) * 100;
                labelStatus.ForeColor = Color.Red;
                labelStatus.Text = "ПОЧЕКАЙ";
    
                Thread th = new Thread(() => { cloudyWorld(xZoom, yZoom, (int)numericDifficulty.Value); }, 1024 * 1024 * 256);
                th.Start();
                th.Join();

                labelStatus.ForeColor = Color.Green;
                labelStatus.Text = "ГОТОВО";
            }
            else if (radioButton2.Checked)
            {
                numericDifficulty.Value = 10;

                labelStatus.Text = "ОЧІКУЮ ВЕРШИНИ";

                bitmap = new Bitmap(xSize, ySize);
                MODE = 1;
            }
            else if(radioButton3.Checked)
            {
                numericDifficulty.Value = 10;

                labelStatus.Text = "ОЧІКУЮ ВЕРШИНИ";

                bitmap = new Bitmap(xSize, ySize);
                MODE = 2;
            }
        }
        private void cloudyWorld(int zoomX, int zoomY, int DEPTH)
        {
            double zi, zr, ci, cr, tmp;
            int k, xMax = 530, yMax = 310;

            bitmap = new Bitmap(xSize, ySize);

            for (int x = -xMax; x < xMax; x++)
            {                      
                ci = Convert.ToDouble(x) / zoomX;                    
                for (int y = -yMax; y < yMax; y++)
                {                 
                    cr = ((float)y) / zoomY;              
                    zi = zr = 0.0;                      // Очищуємо z
                    for (k = 0; k < DEPTH; k++)         // Обчислюємо множину Мандельброта
                    {        
                        tmp = zr * zr - zi * zi;        /* tmp = zn^2          */
                        zi = 2 * zr * zi + ci;          /* zi = zi + ci        */
                        zr = tmp + cr;                  /* zr = zr + cr        */
                        if (zr * zr + zi * zi > 1.0E16)         //  якщо |zn| > R, то
                            break;                              //  виходимо з циклу
                    }

                    if (k < DEPTH)
                    {                    
                        //m = k % 8 + 1;                       
                        //setpallette(m, m * 8 + 7, 0, 0);     
                        //bitmap.SetPixel(x + xMax, y + yMax, Color.DarkBlue);       
                    }
                    else
                        bitmap.SetPixel(x + xMax, y + yMax, Color.LightGreen);     
                } 
            }
            pBox.Image = bitmap;
        }

        void triangleWorld(int x1, int y1, int x2, int y2, int x3, int y3, int N)
        {
            int x12, y12, x23, y23, x31, y31;

            if(N == 0)  return;
 
            x12 = (x1+x2) / 2;// { вычисление координат нового треугольника}
            y12 = (y1+y2) / 2; 
            x23 = (x2+x3) / 2;    
            y23 = (y2+y3) / 2;
            x31 = (x3+x1) / 2;    
            y31 = (y3+y1) / 2;

            Graphics g = Graphics.FromImage(bitmap);

            g.DrawLine(pen, x12, y12, x31, y31);
            g.DrawLine(pen, x23, y23, x31, y31);
            g.DrawLine(pen, x12, y12, x23, y23);

            triangleWorld(x1, y1, x12, y12, x31, y31, N - 1);
            triangleWorld(x2, y2, x12, y12, x23, y23, N - 1);
            triangleWorld(x3, y3, x31, y31, x23, y23, N - 1);
        }
        private void curvyWorld(int order, int X1, int Y1, int X2, int Y2)
        {
            if (order == 0)
            {
                Graphics g = Graphics.FromImage(bitmap);

                g.DrawLine(pen, X1, Y1, X2, Y2);
            }
            else
            {
                double alpha = Math.Atan2(Y2 - Y1, X2 - X1);
                double R = Math.Sqrt((X2 - X1) * (X2 - X1) + (Y2 - Y1) * (Y2 - Y1));
                double Xa = X1 + R * Math.Cos(alpha) / 3,
                Ya = Y1 + R * Math.Sin(alpha) / 3;

                double Xc = Xa + R * Math.Cos(alpha - Math.PI / 3) / 3,
                Yc = Ya + R * Math.Sin(alpha - Math.PI / 3) / 3;

                double Xb = X1 + 2 * R * Math.Cos(alpha) / 3,
                Yb = Y1 + 2 * R * Math.Sin(alpha) / 3;

                curvyWorld(order - 1, X1, Y1, (int)Xa, (int)Ya);
                curvyWorld(order - 1, (int)Xa, (int)Ya, (int)Xc, (int)Yc);
                curvyWorld(order - 1, (int)Xc, (int)Yc, (int)Xb, (int)Yb);
                curvyWorld(order - 1, (int)Xb, (int)Yb, X2, Y2);

            }
        }
        public void mouseMoving(object sender, MouseEventArgs e)
        {
            xPos.Text = "x: " + e.X;
            yPos.Text = "y: " + e.Y;
        }
        public void mouseClick(object sender, MouseEventArgs e)
        {
            if (MODE == 1)
            {
                Graphics g = Graphics.FromImage(bitmap);

                g.DrawRectangle(pen, new Rectangle(new Point(e.X, e.Y), new Size(4, 4)));
                pBox.Image = bitmap;

                addVerticesTriangle(new Point(e.X, e.Y));
            }
            else if(MODE == 2)
            {
                Graphics g = Graphics.FromImage(bitmap);

                g.DrawRectangle(pen, new Rectangle(new Point(e.X, e.Y), new Size(4, 4)));
                pBox.Image = bitmap;

                addVerticesLine(new Point(e.X, e.Y));
            }
        }
        private void addVerticesTriangle(Point point)
        {
            if (triangle.point1.X == 0)
                triangle.point1 = point;
            else if (triangle.point2.X == 0)
                triangle.point2 = point;
            else if (triangle.point3.X == 0)
            {
                triangle.point3 = point;

                bitmap = new Bitmap(xSize, ySize);
                Thread th = new Thread(() =>
                {       triangleWorld(triangle.point1.X, triangle.point1.Y, triangle.point2.X, triangle.point2.Y,
                            triangle.point3.X, triangle.point3.Y, (int)numericDifficulty.Value); },    1024 * 1024 * 256);
                th.Start();
                th.Join();
                
                pBox.Image = bitmap;
                triangle.point1.X = triangle.point2.X = triangle.point3.X = 0;
            }
        }
        private void addVerticesLine(Point point)
        {
            if (line.point1.X == 0)
                line.point1 = point;
            else if (line.point2.X == 0)
            {
                line.point2 = point;

                labelStatus.ForeColor = Color.Red;
                labelStatus.Text = "ПОЧЕКАЙ";

                bitmap = new Bitmap(xSize, ySize);
                Thread th = new Thread(() =>
                {
                    curvyWorld((int)numericDifficulty.Value, line.point1.X, line.point1.Y, line.point2.X, line.point2.Y);
                }, 1024 * 1024 * 256);
                th.Start();
                th.Join();

                pBox.Image = bitmap;
                line.point1.X = line.point2.X = 0;

                labelStatus.ForeColor = Color.Green;
                labelStatus.Text = "ОЧІКУЮ ВЕРШИНИ";
            }
        }
        private void ClearBtn_Click(object sender, EventArgs e)
        {
            bitmap = new Bitmap(pBox.Size.Width, pBox.Size.Height);
            MODE = 0;
            pBox.Image = bitmap;
            labelStatus.Text = "";

            triangle.point1.X = triangle.point2.X = triangle.point3.X = 0;
            line.point1.X = line.point2.X = 0;
        }
    }
}
