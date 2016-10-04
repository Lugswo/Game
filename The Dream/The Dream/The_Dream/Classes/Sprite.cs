using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes
{
    public class Sprite
    {
        public Image image;
        public Vector2 OriginalPosition;
        public Rectangle HitBox, Left, Right, Up, Down;
        public bool Row, Column;
    }
}
