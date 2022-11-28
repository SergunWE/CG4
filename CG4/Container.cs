using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG4
{
    public struct Container
    {
        public List<Circle> circles;
        public List<Rectangle> rectangles;

        public void clear()
        {
            foreach (var circl in circles)
            {
                circl.Clear();
            }

            foreach (var rect in rectangles)
            {
                rect.Clear();
            }

        }

        /* public Circle N1;0
         public Circle N2;1
         public Circle N3;2
         public Circle N4;3

         public Rectangle N5;0
         public Rectangle N6;1
         public Rectangle N7;2*/


    }
}
