using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG4
{
    public class ClippingCircle
    {
        //окружность
        //градус начала дуги
        private float _angle;
        //длина дуги
        private float _len;

        //находит величику угла соответсвующей точке
        private float findAngle(Point ang, Point center)
        {

            double angle = Math.Atan2(ang.X - center.X, ang.Y - center.Y) * (180 / Math.PI);

            //atan для 0 = 90
            //atan для 270 = 180
            //atan для 180 = -90
            //atan для 90 = 0
            switch (angle)
            {
                case 90:
                    angle = 0;
                    break;
                case 180:
                    angle = 270;
                    break;
                case 0:
                    angle = 90;
                    break;
                case -90:
                    angle = 180;
                    break;
                default:
                    {
                        //1 квадрант
                        if (angle > 0 && angle > 90)
                        {
                            angle = 450 - angle;
                        }
                        //2, 3, 4 квадрант
                        else
                        {
                            angle = 90 - angle;
                        }
                    }
                    break;
            }

            return (float)angle;

        }


        //для обрезки окружности (оставляет только обрезанный кусок)
        // mode = 1, окружность находится слева
        //mode определяется по радиусам двух окружностей
        public ClippingCircle(Point startAng, Point endAng, Point start, int radius, bool mode)
        {
            Point center = new Point(start.X + radius, start.Y + radius);

            float begin = findAngle(startAng, center);
            float end = findAngle(endAng, center);

            if (end < begin)
            {
                float temp = begin;
                begin = end;
                end = temp;
            }

            _len = end - begin;
            _angle = begin;

            if (_len > 180)
            {
                _len = 360 - end + begin;
                _angle = end;
            }

            if (mode && _len < 180)
            {
                _len = 360 - end + begin;
                _angle = end;

                if (_len < 180)
                {
                    _len = end - begin;
                    _angle = begin;
                }
            }
        }
    

        public float getAngle()
        {
            return _angle;
        }

        public float getLen()
        {
            return _len;
        }


        public ClippingCircle(float angle, float len)
        {
            _angle = angle;
            _len = len;
        }
    }



    public class Circle
    {
        private int _radius;
        private Point _point;
        private Point _center;
        private List<ClippingCircle> _clip = new List<ClippingCircle>();

        public Circle(int radius, Point point)
        {
            _radius = radius;
            _point = point;

        }

        public void Clear()
        {
            _clip.Clear();
        }

        //изменить радиус
        public void SetRadius(int radius)
        {
            _radius = radius;
        }

        //изменить координату
        public void SetPoint(Point point)
        {
            _point = point;
        }


        public Point GetCenter()
        {
            _center = new Point(_point.X + _radius, _point.Y + _radius);
            return _center;
        }

        //получить радиус
        public int GetRadius()
        {
            return _radius;
        }


        //получить координату
        public Point GetPoint()
        {
            return _point;
        }

        public void addClip(Point start, Point end, bool mode)
        {
            _clip.Add(new ClippingCircle(start, end, _point, _radius, mode));
        }

        public void addClipAllCircle()
        {
            _clip.Add(new ClippingCircle(0, 360));
        }


        private void sort()
        {

            if (_clip.Count != 0)
            {
                //объединить повторения

                _clip.Sort(delegate (ClippingCircle x, ClippingCircle y)
                {
                    if (Math.Round(x.getAngle()) == Math.Round(y.getAngle())) return 0;
                    else if (Math.Round(x.getAngle()) > Math.Round(y.getAngle())) return 1;
                    else return 1;
                });

                float tempAng = _clip[0].getAngle();
                float tempLen = _clip[0].getLen();


                List<ClippingCircle> result = new List<ClippingCircle>();
                foreach (var clip in _clip)
                {
                    if (tempAng < clip.getAngle())
                    {
                        if (tempAng + tempLen < clip.getAngle())
                        {
                            result.Add(new ClippingCircle(tempAng, tempLen));
                        }
                        else
                        {
                            tempLen = clip.getAngle() + clip.getLen() - tempAng;
                        }
                    }

                }
            }
        }

        public List<ClippingCircle> GetClippings()
        {
            return _clip;
        }

        public List<ClippingCircle> GetVisible()
        {
			List<ClippingCircle> drawingArcs = new List<ClippingCircle>();

			double startOfDrawingArc = 0.0;
			double lengthOfDrawingArc = 0.0;

            // Если градус начала + длина в градусах > 360
            // 309, 73
            if (_clip.Count == 0) return new List<ClippingCircle>() { new ClippingCircle(0, 360) };
			if (_clip[_clip.Count - 1].getAngle() + _clip[_clip.Count - 1].getLen() > 360.0)
			{
				double lastElemStartAngle = _clip[_clip.Count - 1].getAngle();
				// 360 - 309 = 51
				double changedLastElemLength = 360 - _clip[_clip.Count - 1].getAngle();

				// 73 - 51 = 22
				double newElemLength = _clip[_clip.Count - 1].getLen() - changedLastElemLength;

				_clip[_clip.Count - 1] = new ClippingCircle((float)lastElemStartAngle, (float)changedLastElemLength);
				_clip.Insert(0, new ClippingCircle(0f, (float)newElemLength));
			}

			for (int i = 1; i < _clip.Count; i++)
			{
				startOfDrawingArc = _clip[i - 1].getAngle() + _clip[i - 1].getLen();
				lengthOfDrawingArc = _clip[i].getAngle() - startOfDrawingArc;

				drawingArcs.Add(new ClippingCircle((float)startOfDrawingArc, (float)lengthOfDrawingArc));
			}

			double startOfLastDrawingArc = _clip[_clip.Count - 1].getAngle() + _clip[_clip.Count - 1].getLen();
			double lengthOfLastDrawingArc = 360 - startOfLastDrawingArc;

			if (startOfLastDrawingArc != 360 && lengthOfLastDrawingArc != 0)
			{
				drawingArcs.Add(new ClippingCircle((float)startOfLastDrawingArc, (float)lengthOfLastDrawingArc));
			}

			if (_clip[0].getAngle() > 0)
			{
				double startOfAdditionalDrawingArc = 0.0;
				double lengthOfAdditionalDrawingArc = _clip[0].getAngle();

				drawingArcs.Add(new ClippingCircle((float)startOfAdditionalDrawingArc, (float)lengthOfAdditionalDrawingArc));
			}

			return drawingArcs;



























			//List<ClippingCircle> result = new List<ClippingCircle>();

   //         double startOfDrawingArc;
   //         double lengthOfDrawingArc;

   //         double a, b;

   //         if (_clip.Count != 0)
   //         {

   //             for (int i = 1; i < _clip.Count; i++)
   //             {
   //                 startOfDrawingArc = _clip[i - 1].getAngle() + _clip[i - 1].getLen() + 0.1;
   //                 lengthOfDrawingArc = _clip[i].getAngle() - 0.1 - startOfDrawingArc;
                                   

   //                 if (lengthOfDrawingArc < 0)
   //                 {
   //                     a = startOfDrawingArc + lengthOfDrawingArc;
   //                     b = -lengthOfDrawingArc;

   //                     result.Add(new ClippingCircle((float)a, (float)b));
   //                 } else
   //                     result.Add(new ClippingCircle((float)startOfDrawingArc, (float)lengthOfDrawingArc));
   //             }

   //             double startOfLastDrawingArc = _clip[_clip.Count - 1].getAngle() + _clip[_clip.Count - 1].getLen() + 0.1;
   //             double lengthOfLastDrawingArc = 360 - startOfLastDrawingArc;

   //             result.Add(new ClippingCircle((float)startOfLastDrawingArc, (float)lengthOfLastDrawingArc));

   //             if (_clip[0].getAngle() != 0.0)
   //             {
   //                 startOfLastDrawingArc = 0.0;
   //                 lengthOfLastDrawingArc = _clip[0].getAngle() - 0.1;

   //                 result.Add(new ClippingCircle((float)startOfLastDrawingArc, (float)lengthOfLastDrawingArc));
   //             }


   //         }
   //         else
   //             result.Add(new ClippingCircle(0, 360));
   //         return result;

        }
    }
}
