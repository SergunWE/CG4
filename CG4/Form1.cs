using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CG4
{
	public partial class Form1 : Form
	{
		Bitmap bitmap;
		Graphics graphics;
		Color backColor = Color.White;
		int trackValue;
		int mode = 0;
		bool modeV = false;
		Container con = new Container();
		int zeroX = 0;
		Random rand = new Random();

		public Form1()
		{
			InitializeComponent();
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			trackValue = trackBar1.Value;

			int N = (int)numericUpDown11.Value;

			//если двигаемый предмет - слой с окружностью
			if (N > 0 && N < 5)
			{
				N = N - 1;

				Circle circ = con.circles[N];

				int freePlaceX = pictureBox1.Width - 2 * circ.GetRadius();
				int newX = (freePlaceX / 12) * trackValue;

				circ.SetPoint(new Point(newX, circ.GetPoint().Y));
			}
			else
			//если двигаемый предмет - прямоугольник
			{
				N = N - 5;

				int freePlaceX = pictureBox1.Width - con.rectangles[N].GetWidht();
				int newX = (freePlaceX / 12) * trackValue;

				con.rectangles[N].SetPoint(new Point(newX, con.rectangles[N].GetPoint().Y));
			}



			Draw();
		}

		//заменить ширину или высоту или радиус:
		//заменить составляющую и координату по y
		//нарисовать новую фигуру
		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			int y1 = rand.Next() % (pictureBox1.Height - 2 * (int)numericUpDown1.Value);

			con.circles[0].SetRadius((int)numericUpDown1.Value);
			con.circles[0].SetPoint(new Point(con.circles[0].GetPoint().X, y1));

			Draw();
		}

		private void numericUpDown2_ValueChanged(object sender, EventArgs e)
		{
			int y1 = rand.Next() % (pictureBox1.Height - 2 * (int)numericUpDown2.Value);

			con.circles[1].SetRadius((int)numericUpDown2.Value);
			con.circles[1].SetPoint(new Point(con.circles[1].GetPoint().X, y1));

			Draw();
		}

		private void numericUpDown4_ValueChanged(object sender, EventArgs e)
		{
			int y1 = rand.Next() % (pictureBox1.Height - 2 * (int)numericUpDown4.Value);

			con.circles[2].SetRadius((int)numericUpDown4.Value);
			con.circles[2].SetPoint(new Point(con.circles[2].GetPoint().X, y1));

			Draw();

		}

		private void numericUpDown5_ValueChanged(object sender, EventArgs e)
		{
			int y1 = rand.Next() % (pictureBox1.Height - 2 * (int)numericUpDown5.Value);

			con.circles[3].SetRadius((int)numericUpDown5.Value);
			con.circles[3].SetPoint(new Point(con.circles[3].GetPoint().X, y1));

			Draw();

		}

		private void numericUpDown8_ValueChanged(object sender, EventArgs e)
		{
			con.rectangles[0].SetWidht((int)numericUpDown8.Value);
			con.rectangles[0].SetPoint(new Point((con.rectangles[0].GetPoint().X + (int)numericUpDown8.Value - con.rectangles[0].GetWidht()), con.rectangles[0].GetPoint().Y));

			Draw();
		}

		private void numericUpDown6_ValueChanged(object sender, EventArgs e)
		{
			int y = rand.Next() % (pictureBox1.Height - (int)numericUpDown6.Value);

			con.rectangles[0].SetHeight((int)numericUpDown6.Value);
			con.rectangles[0].SetPoint(new Point(con.rectangles[0].GetPoint().X, y));

			Draw();
		}

		private void numericUpDown9_ValueChanged(object sender, EventArgs e)
		{
			con.rectangles[1].SetWidht((int)numericUpDown9.Value);
			con.rectangles[1].SetPoint(new Point((con.rectangles[1].GetPoint().X + (int)numericUpDown9.Value) - con.rectangles[1].GetWidht(), con.rectangles[1].GetPoint().Y));

			Draw();
		}

		private void numericUpDown7_ValueChanged(object sender, EventArgs e)
		{
			int y = rand.Next() % (pictureBox1.Height - (int)numericUpDown7.Value);

			con.rectangles[1].SetHeight((int)numericUpDown7.Value);
			con.rectangles[1].SetPoint(new Point(con.rectangles[1].GetPoint().X, y));

			Draw();
		}

		private void numericUpDown10_ValueChanged(object sender, EventArgs e)
		{
			con.rectangles[2].SetWidht((int)numericUpDown10.Value);
			con.rectangles[2].SetPoint(new Point((con.rectangles[2].GetPoint().X + (int)numericUpDown10.Value) - con.rectangles[2].GetWidht(), con.rectangles[2].GetPoint().Y));

			Draw();
		}

		private void numericUpDown3_ValueChanged(object sender, EventArgs e)
		{
			int y = rand.Next() % (pictureBox1.Height - (int)numericUpDown3.Value);

			con.rectangles[2].SetHeight((int)numericUpDown3.Value);
			con.rectangles[2].SetPoint(new Point(con.rectangles[2].GetPoint().X, y));

			Draw();
		}

		private int codePack(int xl, int xp, int yn, int yv, Point a)
		{
			return (a.X < xl ? 1 : 0) + (a.X > xp ? 2 : 0) + (a.Y < yv ? 4 : 0) + (a.Y > yn ? 8 : 0);
		}

		//точки прямой, порядковый номер прямоугольника, которому принадледит прямая, окно видимости
		private void alClipping(Point a, Point b, int i, Rectangle rec)
		{
			Point copya = a;
			Point copyb = b;

			Point c;
			//Алгоритм отсечения Сазерленда-Коэна
			//для ортогонального окна 6
			int xl = rec.GetPoint().X;
			int yv = rec.GetPoint().Y;
			int xp = rec.GetPoint().X + rec.GetWidht();
			int yn = rec.GetPoint().Y + rec.GetHeight();

			//код концов отрезка
			int code_a, code_b, code;

			code_a = codePack(xl, xp, yn, yv, a);
			code_b = codePack(xl, xp, yn, yv, b);

			//если 2 прямой не лежат по одну сторону от окна
			if (code_a != 0 && code_b != 0)
			{
				//если две точки находятся по одну сторону 
				if (((code_a & 8) != 0 && (code_b & 8) != 0 || (code_b & 4) != 0 && (code_a & 4) != 0 || (code_a & 1) != 0 && (code_b & 1) != 0 || (code_a & 2) != 0 && (code_b & 2) != 0))
				{
					con.rectangles[i].addClip(copya, copyb);
					return;

				}

				//если две точки не находятся в противоположных сторонах друг от друга
				if (!((code_a & 8) != 0 && (code_b & 4) != 0 || (code_b & 8) != 0 && (code_a & 4) != 0 || (code_a & 1) != 0 && (code_b & 2) != 0 || (code_a & 2) != 0 && (code_b & 1) != 0))
				{
					return;
				}
			}




			while ((code_a | code_b) != 0)
			{
				// выбираем точку c с ненулевым кодом 
				if (code_a != 0)
				{
					code = code_a;
					c = a;
				}
				else
				{
					code = code_b;
					c = b;
				}

				//если с левее окна, то передвигаем c на прямую x = хл
				//если c правее окна, то передвигаем c на прямую x = хп

				if ((code & 1) != 0)
				{
					c.X = xl;
				}
				else
				if ((code & 2) != 0)
				{
					c.X = xp;
				}
				// если c ниже окна, то передвигаем c на прямую y = yn
				//если c выше окна, то передвигаем c на прямую y = yv
				else if ((code & 4) != 0)
				{
					c.Y = yv;
				}
				else if ((code & 8) != 0)
				{
					c.Y = yn;
				}

				//обновляем код
				if (code == code_a)
				{
					code_a = codePack(xl, xp, yn, yv, c);
					a = c;
				}
				else
				{
					code_b = codePack(xl, xp, yn, yv, c);
					b = c;
				}
			}
			//отсылаем кусок стороны, которую не видно
			if (copya != a)
			{
				con.rectangles[i].addClip(copya, a);

			}
			if (copyb != b)
			{
				con.rectangles[i].addClip(b, copyb);
			}
		}


		Point[] GetIntersectionOfTwoCircles(Point circle1, int radius1, Point circle2, int radius2)
		{
			Point p1 = new Point(-1, -1);
			Point p2 = new Point(-1, -1);

			// Уравнение окружности 1
			int xCoeffEq1 = 2 * (-circle1.X);
			int xAbsoluteTermEq1 = circle1.X * circle1.X;

			int yCoeffEq1 = 2 * (-circle1.Y);
			int yAbsoluteTermEq1 = circle1.Y * circle1.Y;

			int totalAbsoluteTermEq1 = xAbsoluteTermEq1 + yAbsoluteTermEq1 - radius1 * radius1;

			// Уравнение окружности 2
			int xCoeffEq2 = 2 * (-circle2.X);
			int xAbsoluteTermEq2 = circle2.X * circle2.X;

			int yCoeffEq2 = 2 * (-circle2.Y);
			int yAbsoluteTermEq2 = circle2.Y * circle2.Y;

			int totalAbsoluteTermEq2 = xAbsoluteTermEq2 + yAbsoluteTermEq2 - radius2 * radius2;

			// Вычитаем уравнение 2 из уравнения 1
			// 4x - 6y - 16 = 0 - получаем в итоге
			int shortedXCoef = xCoeffEq1 - xCoeffEq2; // 4x
			int shortedYCoef = yCoeffEq1 - yCoeffEq2; // -6y
			int shortedAbsoluteTerm = totalAbsoluteTermEq1 - totalAbsoluteTermEq2; // -16

			if (shortedXCoef != 0)
			{
				// 4x - 6y - 16 = 0 в x = (6y + 16)/4 => x = 1.5y + 4;
				// double reducedXCoefRelativeX = 1.0; // x
				double reducedYCoefRelativeX = -((double)shortedYCoef / shortedXCoef); // 6y / 4 = 1.5y;
				double reducedAbsoluteTermRelativeX = -((double)shortedAbsoluteTerm / shortedXCoef); // 16 / 4 = 4;


				// x^2 = (1.5y + 4)^2, раскрываем этот квадрат и находим коэффициенты ay^2 + by + c
				double aCoefInReplacedDoubledX = reducedYCoefRelativeX * reducedYCoefRelativeX; // (1.5y)^2 = 2.25y^2
				double bCoefInReplacedDoubledX = 2 * reducedYCoefRelativeX * reducedAbsoluteTermRelativeX; // 2 * 1.5y * 4 = 12y;
				double cCoefInReplacedDoubledX = reducedAbsoluteTermRelativeX * reducedAbsoluteTermRelativeX; // 4 * 4 = 16;

				// Т.к. мы будем подставлять в уравнение 1, то смотрим на коэффициент X в переменной xCoeffEq1
				// -2x = -2*(1.5y + 4)
				double aCoefInReplacedX = xCoeffEq1 * reducedYCoefRelativeX; // -2 * 1.5y = -3y
				double bCoefInReplacedX = xCoeffEq1 * reducedAbsoluteTermRelativeX; // -2 * 4 = -8

				// 3,25y^2 + 7y + 1 = 0
				double finalACoef = aCoefInReplacedDoubledX + 1; // 2.25y^2 + y^2 = 3.25y^2
				double finalBCoef = bCoefInReplacedDoubledX + aCoefInReplacedX + yCoeffEq1; // 12y - 3y - 2y = 7y
				double finalCCoef = cCoefInReplacedDoubledX + bCoefInReplacedX + totalAbsoluteTermEq1; // 16 - 8 - 7 = 1

				// D = b^2 - 4ac
				double discriminant = finalBCoef * finalBCoef - 4 * finalACoef * finalCCoef;

				if (discriminant > 0)
				{
					// x1 = (-b + sqrt(D) ) / 2a
					double y1 = (-finalBCoef + Math.Sqrt(discriminant)) / (2 * finalACoef);
					double y2 = (-finalBCoef - Math.Sqrt(discriminant)) / (2 * finalACoef);

					double x1 = reducedYCoefRelativeX * y1 + reducedAbsoluteTermRelativeX;
					double x2 = reducedYCoefRelativeX * y2 + reducedAbsoluteTermRelativeX;

					p1 = new Point((int)Math.Round(x1), (int)Math.Round(y1));
					p2 = new Point((int)Math.Round(x2), (int)Math.Round(y2));
				}
			}
			else
			{
				double reducedXCoefRelativeY = -((double)shortedXCoef / shortedYCoef);
				double reducedAbsoluteTermRelativeY = -((double)shortedAbsoluteTerm / shortedYCoef);

				double aCoefInReplacedDoubledY = reducedXCoefRelativeY * reducedXCoefRelativeY;
				double bCoefInReplacedDoubledY = 2 * reducedXCoefRelativeY * reducedAbsoluteTermRelativeY;
				double cCoefInReplacedDoubledY = reducedAbsoluteTermRelativeY * reducedAbsoluteTermRelativeY;

				double aCoefInReplacedY = yCoeffEq1 * reducedXCoefRelativeY;
				double bCoefInReplacedY = yCoeffEq1 * reducedAbsoluteTermRelativeY;

				double finalACoef = aCoefInReplacedDoubledY + 1;
				double finalBCoef = bCoefInReplacedDoubledY + aCoefInReplacedY + xCoeffEq1;
				double finalCCoef = cCoefInReplacedDoubledY + bCoefInReplacedY + totalAbsoluteTermEq1;

				double discriminant = finalBCoef * finalBCoef - 4 * finalACoef * finalCCoef;

				if (discriminant > 0)
				{
					double x1 = (-finalBCoef + Math.Sqrt(discriminant)) / (2 * finalACoef);
					double x2 = (-finalBCoef - Math.Sqrt(discriminant)) / (2 * finalACoef);

					double y1 = reducedXCoefRelativeY * x1 + reducedAbsoluteTermRelativeY;
					double y2 = reducedXCoefRelativeY * x2 + reducedAbsoluteTermRelativeY;

					p1 = new Point((int)Math.Round(x1), (int)Math.Round(y1));
					p2 = new Point((int)Math.Round(x2), (int)Math.Round(y2));
				}
			}

			Point[] result = { p1, p2 };

			return result;
		}

		Point[] GetIntersectionOfLineAndCircle(Point circleCenter, int circleRadius, Point linePoint1, Point linePoint2)
		{
			Point p1 = new Point(-1, -1);
			Point p2 = new Point(-1, -1);

			// Т.к. мы накладываем ортогональные окна,
			// то у каждой прямой будут равны либо координаты по одной из осей
			if (linePoint1.Y == linePoint2.Y)
			{
				double y = linePoint1.Y;

				// (x-1)^2 + (y-1)^2 = 3^2 -> x^2 - 2x + 1 + (y-1)^2 = 3^2
				int xDoubledCoeff = 1; // x^2
				int xSingleCoeff = 2 * (-circleCenter.X); // -1 * 2 * x = -2x
				int xAbsoluteTerm = circleCenter.X * circleCenter.X; // 1 * 1 = 1

				// x^2 - 2x + 1 + (3-1)^2 - 3^2 = 0 
				// x^2 - 2x - 4 = 0;
				int finalAbsoluteTerm = xAbsoluteTerm + (int)Math.Pow(y - circleCenter.Y, 2) - (int)Math.Pow(circleRadius, 2);

				double discriminant = (int)Math.Pow(xSingleCoeff, 2) - 4 * xDoubledCoeff * finalAbsoluteTerm;

				if (discriminant > 0)
				{
					double x1 = (-xSingleCoeff + Math.Sqrt(discriminant)) / (2 * xDoubledCoeff);
					double x2 = (-xSingleCoeff - Math.Sqrt(discriminant)) / (2 * xDoubledCoeff);

					p1 = new Point((int)Math.Round(x1), (int)Math.Round(y));
					p2 = new Point((int)Math.Round(x2), (int)Math.Round(y));
				}
			}
			else if (linePoint1.X == linePoint2.X)
			{
				double x = linePoint1.X;

				// (x-1)^2 + (y-1)^2 = 3^2 -> (x-1)^2 + y^2 - 2y + 1 = 3^2
				int yDoubledCoeff = 1; // x^2
				int ySingleCoeff = 2 * (-circleCenter.Y); // -1 * 2 * x = -2x
				int yAbsoluteTerm = circleCenter.Y * circleCenter.Y; // 1 * 1 = 1

				// (2-1)^2 + y^2 - 2y + 1 - 3^2 = 0
				// y^2 - 2y - 7 = 0;
				int finalAbsoluteTerm = yAbsoluteTerm + (int)Math.Pow(x - circleCenter.X, 2) - (int)Math.Pow(circleRadius, 2);

				double discriminant = (int)Math.Pow(ySingleCoeff, 2) - 4 * yDoubledCoeff * finalAbsoluteTerm;

				if (discriminant > 0)
				{
					double y1 = (-ySingleCoeff + Math.Sqrt(discriminant)) / (2 * yDoubledCoeff);
					double y2 = (-ySingleCoeff - Math.Sqrt(discriminant)) / (2 * yDoubledCoeff);

					p1 = new Point((int)Math.Round(x), (int)Math.Round(y1));
					p2 = new Point((int)Math.Round(x), (int)Math.Round(y2));
				}
			}

			List<Point> points = new List<Point>();
			if (p1 != new Point(-1, -1))
			{
				points.Add(p1);
			}

			if (p2 != new Point(-1, -1))
			{
				points.Add(p2);
			}
			//Point[] result = { p1, p2 };

			return points.ToArray();
		}

		//определение стороны по которую лежит точка относительно прямой, организованной 2мя точками
		private int orien(Point a, Point b, Point c)
		{
			//точка центра прмяой
			Point m = new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);

			//точка слева - 1
			//точка справа - 2
			//точка сверху - 4
			//точка снизу - 8

			return (m.X > c.X ? 1 : 0) + (m.X < c.X ? 2 : 0) + (m.Y < c.Y ? 4 : 0) + (m.Y > c.Y ? 8 : 0);

		}

		private void Draw()
		{
			graphics.Clear(backColor);
			//рисовать алгоритмом А
			con.clear();
			//найти пересения, принимая, что на каждой итерации нижний и верхний слой целые и ничем ранее не перекрыты
			//нахождение перекрытх частей окружностей окружностями (слои от 1 до 3 с слоями от 2 до 4)
			for (int i = 0; i < con.circles.Count - 1; i++)
			{
				for (int j = 1 + i; j < con.circles.Count; j++)
				{
					Point circi = con.circles[i].GetCenter();
					Point circj = con.circles[j].GetCenter();
					int radi = con.circles[i].GetRadius();
					int radj = con.circles[j].GetRadius();

					Point[] mac = GetIntersectionOfTwoCircles(circi, radi, circj, radj);

					//если 2 точки пересечения
					if (mac[0] != new Point(-1, -1) && mac[1] != new Point(-1, -1))
					{
						bool mod = false;
						int x1 = orien(mac[0], mac[1], circj);
						int x2 = orien(mac[0], mac[1], circi);


						if ((x1 & x2) != 0 && radi < radj)
						{
							mod = true;
						}

						con.circles[i].addClip(mac[0], mac[1], mod);
					}
					else
					//добавить добавление всей окружности если нижняя лежит полностю под верхней
					if (radi <= radj && circi.X >= (circj.X - radj) && circi.X <= (circj.X + radj) && circi.Y >= (circj.Y - radj) && circi.Y <= (circj.Y + radj))
					{
						con.circles[i].addClipAllCircle();
					}
				}
			}

			//нахождение перекрытых частей окружностей прямоугольниками (слои от 1 до 4 с слоями от 5 до 7)

			//ДОБАВИТЬ ВЗАИМОДЕЙСТВИЕ МЕЖДУ ОКНАМИ И ОКРУЖНОСТЯМИ

			for (int i = 0; i < con.circles.Count; i++)
			{
				Circle currentCircle = con.circles[i];
				Point circleCenter = currentCircle.GetCenter();
				int circleRadius = currentCircle.GetRadius();
				for (int j = 0; j < con.rectangles.Count; j++)
				{
					List<Point> macX = new List<Point>();
					Rectangle currentRectangle = con.rectangles[j];
					Point recPoint = currentRectangle.GetPoint();
					int widht = currentRectangle.GetWidht();
					int height = currentRectangle.GetHeight();

					int macPrev = 0;
					bool top, bottom, left, right;

					//для стороны 1: верхней
					Point a = new Point(recPoint.X, recPoint.Y);
					Point b = new Point(recPoint.X + widht, recPoint.Y);
					macX.AddRange(GetIntersectionOfLineAndCircle(circleCenter, circleRadius, a, b));
					top = macPrev != macX.Count;
					macPrev = macX.Count;

					//для стороны 2 левой
					a = new Point(recPoint.X, recPoint.Y);
					b = new Point(recPoint.X, recPoint.Y + height);
					macX.AddRange(GetIntersectionOfLineAndCircle(circleCenter, circleRadius, a, b));
					left = macPrev != macX.Count;
					macPrev = macX.Count;

					//для стороны 3 нижней
					a = new Point(recPoint.X, recPoint.Y + height);
					b = new Point(recPoint.X + widht, recPoint.Y + height);
					macX.AddRange(GetIntersectionOfLineAndCircle(circleCenter, circleRadius, a, b));
					bottom = macPrev != macX.Count;
					macPrev = macX.Count;

					//для стороны 4 правой
					a = new Point(recPoint.X + widht, recPoint.Y);
					b = new Point(recPoint.X + widht, recPoint.Y + height);
					macX.AddRange(GetIntersectionOfLineAndCircle(circleCenter, circleRadius, a, b));
					right = macPrev != macX.Count;

					if (macX.Count == 0)
					{
						if (codePack(recPoint.X, recPoint.X + widht,
							recPoint.Y + height, recPoint.Y, circleCenter) != 0)
						{
							con.circles[i].addClipAllCircle();
							break;
						}
					}
					else
					{
						if (macX.Count == 2)
						{
							//точка слева - 1
							//точка справа - 2
							//точка сверху - 4
							//точка снизу - 8
							bool mod = false;
							int x1 = orien(macX[0], macX[1], circleCenter);
							if (right)
							{
								mod = x1 == 2;
							}
							else
							if (left)
							{
								mod = x1 == 1;
							}
							if (top)
							{
								mod = x1 == 8;
							}
							if (bottom)
							{
								mod = x1 == 4;
							}
							//if ((x1 == 1 && right) || (x1 == 2 && left) || (x1 == 8 && top) || (x1 == 4 && bottom)) mod = false;
							currentCircle.addClip(macX[0], macX[1], mod);
						}
					}
				}
			}

			//нахождение перекрытых частей прямоугольников прямоугольниками (слои от 5 до 7 с слоями от 6 до 7)
			for (int i = 0; i < con.rectangles.Count - 1; i++)
			{
				//для каждой стороны рассматриваемого прямоугольника решаем задачу отчесения ортогональным окном
				for (int j = 1 + i; j < con.rectangles.Count; j++)
				{
					//для стороны 1: , new Point(_point.X + _width, _point.Y))
					Point a = new Point(con.rectangles[i].GetPoint().X, con.rectangles[i].GetPoint().Y);
					Point b = new Point(con.rectangles[i].GetPoint().X + con.rectangles[i].GetWidht(), con.rectangles[i].GetPoint().Y);
					alClipping(a, b, i, con.rectangles[j]);

					//для стороны 2 new Point(_point.X, _point.Y), new Point(_point.X, _point.Y + _height))
					a = new Point(con.rectangles[i].GetPoint().X, con.rectangles[i].GetPoint().Y);
					b = new Point(con.rectangles[i].GetPoint().X, con.rectangles[i].GetPoint().Y + con.rectangles[i].GetHeight());
					alClipping(a, b, i, con.rectangles[j]);

					//для стороны 3 new Point(_point.X, _point.Y + _height), new Point(_point.X + _width, _point.Y + _height)
					a = new Point(con.rectangles[i].GetPoint().X, con.rectangles[i].GetPoint().Y + con.rectangles[i].GetHeight());
					b = new Point(con.rectangles[i].GetPoint().X + con.rectangles[i].GetWidht(), con.rectangles[i].GetPoint().Y + con.rectangles[i].GetHeight());
					alClipping(a, b, i, con.rectangles[j]);

					//для стороны 4 (new Point(_point.X + _width, _point.Y), new Point(_point.X + _width, _point.Y + _height))
					a = new Point(con.rectangles[i].GetPoint().X + con.rectangles[i].GetWidht(), con.rectangles[i].GetPoint().Y);
					b = new Point(con.rectangles[i].GetPoint().X + con.rectangles[i].GetWidht(), con.rectangles[i].GetPoint().Y + con.rectangles[i].GetHeight());
					alClipping(a, b, i, con.rectangles[j]);
				}
			}


			//рисовать алгоритмом А
			Pen pen1 = new Pen(Color.Black);

			//рисовать алгоритмом Б
			Pen pen2 = new Pen(Color.Red);

			// отрисовка окружностей
			for (int i = 0; i < con.circles.Count; i++)
			{
				//массив видимых частей окружности
				List<ClippingCircle> vis1 = con.circles[i].GetVisible();

				foreach (var v in vis1)
				{
					graphics.DrawArc(pen1, con.circles[i].GetPoint().X, con.circles[i].GetPoint().Y, con.circles[i].GetRadius() * 2, con.circles[i].GetRadius() * 2, v.getAngle(), v.getLen());
				}
				if(modeV)
				{
					Task.Delay(200).Wait();
					pictureBox1.Image = bitmap;
				}


				if (mode == 0)
				{
					//массив невидимых частей окружности
					List<ClippingCircle> vis2 = con.circles[i].GetClippings();

					foreach (var v in vis2)
					{
						graphics.DrawArc(pen2, con.circles[i].GetPoint().X, con.circles[i].GetPoint().Y, con.circles[i].GetRadius() * 2, con.circles[i].GetRadius() * 2, v.getAngle(), v.getLen());
					}
					if (modeV)
					{
						Task.Delay(200).Wait();
						pictureBox1.Image = bitmap;
					}
				}
			}


			for (int i = 0; i < con.rectangles.Count; i++)
			{
				List<ClippingRectangle> vis1 = con.rectangles[i].GetVisible();

				foreach (var v in vis1)
				{
					graphics.DrawLine(pen1, v.getStart(), v.getEnd());
				}
				if (modeV)
				{
					Task.Delay(200).Wait();
					pictureBox1.Image = bitmap;
				}

				if (mode == 0)
				{
					List<ClippingRectangle> vis2 = con.rectangles[i].GetClippings();

					foreach (var v in vis2)
					{
						graphics.DrawLine(pen2, v.getStart(), v.getEnd());
					}
					if (modeV)
					{
						Task.Delay(200).Wait();
						pictureBox1.Image = bitmap;
					}
				}
			}

			pictureBox1.Image = bitmap;
		}

		//Начальная инициализация каждого кадра
		private void pictureBox1_Paint(object sender, PaintEventArgs e)
		{
			base.OnPaint(e);
			if(bitmap == null)
			bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
			graphics = Graphics.FromImage(bitmap);
		}

		//если mode = 1, то режим А
		//если mode = 0, то режим Б       
		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			//если галочка на А
			if (mode == 1) return;
			mode = (checkBox1.Checked ? 1 : 0);
			checkBox2.Checked = !checkBox1.Checked;
			//if (checkBox1.Checked)
			//{
			//	mode = 1;
			//	checkBox2.Checked = false;
			//}
			//else
			//{
			//	mode = 0;
			//	checkBox2.Checked = true;
			//}

			Draw();
		}
		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
			//если галочка на Б
			if (mode == 0) return;
			mode = (checkBox2.Checked ? 0 : 1);
			checkBox1.Checked = !checkBox2.Checked;
			//if (checkBox2.Checked)
			//{
			//	mode = 0;
			//	checkBox1.Checked = false;
			//}
			//else
			//{
			//	mode = 1;
			//	checkBox1.Checked = true;
			//}

			Draw();
		}

		private void checkBox3_CheckedChanged(object sender, EventArgs e)
		{
			modeV = ((CheckBox)sender).Checked;
			Draw();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			trackValue = trackBar1.Value;
			con.circles = new List<Circle>();

			//нужно заменить на рандом в промежутке от 0 до pictureBox1.Height - 2*(int)numericUpDown1.Value
			int y1 = rand.Next() % (pictureBox1.Height - 2 * (int)numericUpDown1.Value);
			int y2 = rand.Next() % (pictureBox1.Height - 2 * (int)numericUpDown2.Value);
			int y3 = rand.Next() % (pictureBox1.Height - 2 * (int)numericUpDown4.Value);
			int y4 = rand.Next() % (pictureBox1.Height - 2 * (int)numericUpDown5.Value);

			con.circles.Add(new Circle((int)numericUpDown1.Value, new Point(zeroX, y1)));
			con.circles.Add(new Circle((int)numericUpDown2.Value, new Point(zeroX, y2)));
			con.circles.Add(new Circle((int)numericUpDown4.Value, new Point(zeroX, y3)));
			con.circles.Add(new Circle((int)numericUpDown5.Value, new Point(zeroX, y4)));

			con.rectangles = new List<Rectangle>();

			int y5 = rand.Next() % (pictureBox1.Height - (int)numericUpDown6.Value);
			int y6 = rand.Next() % (pictureBox1.Height - (int)numericUpDown7.Value);
			int y7 = rand.Next() % (pictureBox1.Height - (int)numericUpDown3.Value);

			con.rectangles.Add(new Rectangle((int)numericUpDown8.Value, (int)numericUpDown6.Value, new Point(zeroX, y5)));
			con.rectangles.Add(new Rectangle((int)numericUpDown9.Value, (int)numericUpDown7.Value, new Point(zeroX, y6)));
			con.rectangles.Add(new Rectangle((int)numericUpDown10.Value, (int)numericUpDown3.Value, new Point(zeroX, y7)));

		}
	}

}
