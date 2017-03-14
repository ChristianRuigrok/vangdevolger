﻿using System.Drawing;
using System.Windows.Forms;

namespace VangDeVolger.Elements
{
    public abstract class Element
    {
        public int X;
        public int Y;
        public PictureBox Pb;

        protected Element(int x, int y)
        {
            X = x;
            Y = y;
            Pb = new PictureBox
            {
                Size = new Size(Level.Scaling, Level.Scaling),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(x * Level.Scaling, y * Level.Scaling)
            };
        }

        public abstract bool Move(Direction direction);
    }
}
