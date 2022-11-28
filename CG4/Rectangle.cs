using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG4
{

    public class ClippingRectangle
    {
        private Point _startL;
        private Point _endL;

        public ClippingRectangle(Point start, Point end)
        {
            _startL = start;
            _endL = end;
        }

        public Point getStart()
        {
            return _startL;
        }

        public Point getEnd()
        {
            return _endL;
        }

        public void swap()
        {
            Point temp = _startL;
            _startL = _endL;
            _endL = temp;
        }

    }

    public class Rectangle
    {
        private int _width;
        private int _height;
        private Point _point;

        private List<ClippingRectangle> _clip = new List<ClippingRectangle>();

        public Rectangle(int width, int height, Point start)
        {
            _width = width;
            _height = height;
            _point = start;
        }

        public void Clear()
        {
            _clip.Clear();
        }

        //изменить длину
        public void SetWidht(int width)
        {            
            _width = width;

        }

        //изменить высоту
        public void SetHeight(int height)
        {
            _height = height;
        }


        //изменить координату начала
        public void SetPoint(Point start)
        { 
            _point = start;
        }


        //получить длину
        public int GetWidht()
        {
            return _width;
        }

        //получить высоту
        public int GetHeight()
        {
            return _height;
        }


        //получить координату
        public Point GetPoint()
        {
            return _point;
        }

        public void addClip(Point start, Point end)
        {
            _clip.Add(new ClippingRectangle(start, end));
        }

        private void sort()
        {

            if (_clip.Count != 0)
            {
                //удаляет повторения
                for(int i=0; i<_clip.Count() - 1; i++)
                {

                    List<ClippingRectangle> find = _clip.FindAll(x => x.getStart() == _clip[i].getStart() && x.getEnd() == _clip[i].getEnd());

                    if (find.Count > 1)
                    {
                        ClippingRectangle temp = _clip[i];
                        _clip.RemoveAll(x => x.getStart() == _clip[i].getStart() && x.getEnd() == _clip[i].getEnd());
                        _clip.Add(temp);
                    }

                }



            }
        }


        public List<ClippingRectangle> GetClippings()
        {
            return _clip;
        }


        private List<ClippingRectangle> ByX(ClippingRectangle clip, List<ClippingRectangle> result, int resX)
        {
            int y1 = clip.getStart().Y;
            int y2 = clip.getEnd().Y;

            if (y1 > y2)
            {
                int temp = y1;
                y1 = y2;
                y2 = temp;
            }

            List<ClippingRectangle> clrec = result.FindAll(x => x.getStart().X == resX && x.getEnd().X == resX);
            result.RemoveAll(x => x.getStart().X == resX && x.getEnd().X == resX);

            foreach (var res in clrec)
            {
                if (res.getEnd().Y < res.getStart().Y)
                {
                    res.swap();
                }

                if (y1 >= res.getStart().Y && y1 <= res.getEnd().Y)
                {

                    if (y2 < res.getEnd().Y)
                    {
                        if (res.getStart().Y != y1)
                            result.Add(new ClippingRectangle(new Point(resX, res.getStart().Y), new Point(resX, y1)));
                        if (res.getEnd().Y != y2)
                            result.Add(new ClippingRectangle(new Point(resX, y2), new Point(resX, res.getEnd().Y)));
                    }
                    else
                    if (y2 >= res.getEnd().Y)
                    {
                        result.Add(new ClippingRectangle(new Point(resX, res.getStart().Y), new Point(resX, y1)));
                    }
                } else if (y2 > res.getStart().Y && y2 < res.getEnd().Y)
                {
                    result.Add(new ClippingRectangle(new Point(resX, y2), new Point(resX, res.getEnd().Y)));
                } else if (!(y1 <= res.getStart().Y && y2 >= res.getEnd().Y))
                {
                    result.Add(new ClippingRectangle(new Point(resX, res.getStart().Y), new Point(resX, res.getEnd().Y)));
                }
            }
            return result;
        }

        private List<ClippingRectangle> ByY(ClippingRectangle clip, List<ClippingRectangle> result, int resY)
        {
            int x1 = clip.getStart().X;
            int x2 = clip.getEnd().X;

            if (x1 > x2)
            {
                int temp = x1;
                x1 = x2;
                x2 = temp;
            }

            List<ClippingRectangle> clrec = result.FindAll(x => x.getStart().Y == resY && x.getEnd().Y == resY);
            result.RemoveAll(x => x.getStart().Y == resY && x.getEnd().Y == resY);

            foreach (var res in clrec)
            {
                if (res.getEnd().X < res.getStart().X)
                {
                    res.swap();
                }

                if (x1 >= res.getStart().X && x1 <= res.getEnd().X)
                {

                    if (x2 < res.getEnd().X)
                    {
                        if(res.getStart().X != x1)
                        result.Add(new ClippingRectangle(new Point(res.getStart().X, resY), new Point(x1, resY)));
                        if(res.getEnd().X != x2)
                        result.Add(new ClippingRectangle(new Point(x2, resY), new Point(res.getEnd().X, resY)));
                    }
                    else
                    if (x2 >= res.getEnd().X)
                    {
                        result.Add(new ClippingRectangle(new Point(res.getStart().X, resY), new Point(x1, resY)));
                    }
                }else if(x2 > res.getStart().X && x2 < res.getEnd().X)
                {
                    result.Add(new ClippingRectangle(new Point(x2, resY), new Point(res.getEnd().X, resY)));
                } else if (!(x1 <= res.getStart().X && x2 >= res.getEnd().X))
                {
                    result.Add(new ClippingRectangle(new Point(res.getStart().X, resY), new Point(res.getEnd().X, resY)));
                }

            } 
            return result;
        }


        public List<ClippingRectangle> GetVisible()
        {
            sort();

            List<ClippingRectangle> result = new List<ClippingRectangle>();

            result.Add(new ClippingRectangle(new Point(_point.X, _point.Y), new Point(_point.X + _width, _point.Y)));
            result.Add(new ClippingRectangle(new Point(_point.X, _point.Y), new Point(_point.X, _point.Y + _height)));
            result.Add(new ClippingRectangle(new Point(_point.X, _point.Y + _height), new Point(_point.X + _width, _point.Y + _height)));
            result.Add(new ClippingRectangle(new Point(_point.X + _width, _point.Y), new Point(_point.X + _width, _point.Y + _height)));

            if (_clip.Count != 0)
            {

                foreach (var clip in _clip)
                {
                    //левая граница 
                    if (clip.getStart().X == _point.X && clip.getEnd().X == _point.X)
                    {
                        ByX(clip, result, _point.X);
                    }
                    else
                    //правая граница 
                    if (clip.getStart().X == _point.X + _width && clip.getEnd().X == _point.X + _width)
                    {
                        ByX(clip, result, _point.X + _width);
                    }
                    else
                    //вверхняя граница
                    if (clip.getStart().Y == _point.Y && clip.getEnd().Y == _point.Y)
                    {
                        ByY(clip, result, _point.Y);
                    }
                    else
                    //нижняя граница
                    if (clip.getStart().Y == _point.Y + _height && clip.getEnd().Y == _point.Y + _height)
                    {
                        ByY(clip, result, _point.Y + _height);
                    }
                }
            }

            return result;
        }
    }
}
