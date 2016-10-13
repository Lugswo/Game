using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace The_Dream.Classes
{
    public class Map
    {
        public Vector2 Moved, prevMoved;
        public Rectangle DeadZone, Screen, OriginalDeadZone;
        public SoundManager soundManager;
        [XmlElement("Sprite")]
        public List<MapSprite> Maps;
        [XmlElement("Blank")]
        public List<MapSprite> Blanks;
        [XmlIgnore]
        public string[,] Area;
        public string AreaName;
        public int ScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        public int ScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        public int AreaX, AreaY;
        List<int> x, y;
        public bool IsTransitioning;
        public Textures textures;
        public bool Vertical, Horizontal, EdgeVertical, EdgeHorizontal, Right, Left, Up, Down, Column, Row, Pause, NewMap, ExitTop, ExitBottom, ExitRight, ExitLeft, SongPlaying, Centered, vCentered;
        public Map()
        {
            Moved = Vector2.Zero;
            Screen = new Rectangle(0, 0, ScreenWidth, ScreenHeight);
            Vertical = Horizontal = EdgeHorizontal = EdgeVertical = Pause = false;
            Area = new string[4, 4];
            Area[1, 1] = "Test Map";
            Area[2, 1] = "Right Map";
            Area[1, 2] = "Down Map";
            NewMap = SongPlaying = false;
        }
        public void GetReferences(Textures realTextures)
        {
            textures = realTextures;
        }
        public void NewArea(int X, int Y)
        {
            AreaX = X;
            AreaY = Y;
            IsTransitioning = true;
        }
        public void LoadContent()
        {
            x = new List<int>();
            y = new List<int>();
            foreach (MapSprite map in Maps)
            {
                map.image.LoadContent();
                map.OriginalPosition = map.image.Position;
                x.Add((int)map.OriginalPosition.X);
                y.Add((int)map.OriginalPosition.Y);
                x.Add((int)map.OriginalPosition.X + map.image.texture.Width);
                y.Add((int)map.OriginalPosition.Y + map.image.texture.Height);
                map.HitBox = new Rectangle((int)map.OriginalPosition.X, (int)map.OriginalPosition.Y, map.image.texture.Width, map.image.texture.Height);
            }
            foreach (MapSprite map in Blanks)
            {
                map.image.LoadContent();
                map.OriginalPosition = map.image.Position;
                map.HitBox = new Rectangle((int)map.OriginalPosition.X, (int)map.OriginalPosition.Y, map.image.texture.Width, map.image.texture.Height);
                map.Left = new Rectangle((int)map.OriginalPosition.X, (int)map.OriginalPosition.Y, 1, map.image.texture.Height);
                map.Right = new Rectangle(map.image.texture.Width, (int)map.OriginalPosition.Y, -1, map.image.texture.Height);
                map.Up = new Rectangle((int)map.OriginalPosition.X, (int)map.OriginalPosition.Y, map.image.texture.Width, 1);
                map.Down = new Rectangle((int)map.OriginalPosition.X, map.image.texture.Height, map.image.texture.Width, -1);
            }
            DeadZone = new Rectangle(x.Min(), y.Min(), x.Max(), y.Max());
            OriginalDeadZone = new Rectangle(x.Min(), y.Min(), x.Max(), y.Max());
        }
        public void UnloadContent()
        {
            foreach (MapSprite map in Maps)
            {
                map.image.UnloadContent();
            }
            foreach (MapSprite map in Blanks)
            {
                map.image.UnloadContent();
            }
        }
        public void Update(GameTime gameTime, Player player)
        {
        //    if (ScreenManager.Instance.IsTransitioning == false)
        //    {
        //        if (SongPlaying == false)
        //        {
        //            SongPlaying = true;
        //            soundManager.Music["Megalovania"].soundEffect.Play();
        //        }
        //    }
        //    if (Pause == false)
        //    {
            //EdgeHorizontal = EdgeVertical = Right = Left = Up = Down = Column = Row = false;
            Moved = new Vector2(player.X, player.Y);
            if (Moved.X < ScreenManager.instance.Dimensions.X / 2)
            {
                Left = true;
            }
            else
            {
                Left = false;
            }
            if (Moved.X > (DeadZone.Right - ScreenManager.Instance.Dimensions.X / 2))
            {
                Right = true;
            }
            else
            {
                Right = false;
            }
            if (Moved.Y < ScreenManager.Instance.Dimensions.Y / 2)
            {
                Up = true;
            }
            else
            {
                Up = false;
            }
            if (Moved.Y > DeadZone.Bottom - ScreenManager.Instance.Dimensions.Y / 2)
            {
                Down = true;
            }
            else
            {
                Down = false;
            }
        }
        public void HorizontalMove()
        {
            foreach (MapSprite map in Maps)
            {
                map.image.Position.X = map.OriginalPosition.X - Moved.X + ScreenManager.instance.Dimensions.X / 2;
            }
            foreach (MapSprite blank in Blanks)
            {
                blank.HitBox.X = (int)blank.OriginalPosition.X - (int)Moved.X + (int)ScreenManager.instance.Dimensions.X / 2;
                blank.Left.X = (int)blank.OriginalPosition.X -(int)Moved.X + (int)ScreenManager.instance.Dimensions.X / 2;
                blank.Right.X = (int)blank.OriginalPosition.X + (int)blank.image.texture.Width - (int)Moved.X + (int)ScreenManager.instance.Dimensions.X / 2;
                blank.Up.X = (int)blank.OriginalPosition.X - (int)Moved.X + (int)ScreenManager.instance.Dimensions.X / 2;
                blank.Down.X = (int)blank.OriginalPosition.X - (int)Moved.X + (int)ScreenManager.instance.Dimensions.X / 2;
            }
        }
        public void VerticalMove()
        {
            foreach (MapSprite map in Maps)
            {
                map.image.Position.Y = map.OriginalPosition.Y - Moved.Y + ScreenManager.instance.Dimensions.Y / 2;
            }
            foreach (MapSprite blank in Blanks)
            {
                blank.HitBox.Y = (int)blank.OriginalPosition.Y - (int)Moved.Y + (int)ScreenManager.instance.Dimensions.Y / 2;
                blank.Left.Y = (int)blank.OriginalPosition.Y - (int)Moved.Y + (int)ScreenManager.instance.Dimensions.Y / 2;
                blank.Right.Y = (int)blank.OriginalPosition.Y - (int)Moved.Y + (int)ScreenManager.instance.Dimensions.Y / 2;
                blank.Up.Y = (int)blank.OriginalPosition.Y - (int)Moved.Y + (int)ScreenManager.instance.Dimensions.Y / 2;
                blank.Down.Y = (int)blank.OriginalPosition.Y + (int)blank.OriginalPosition.Y - (int)Moved.Y + (int)ScreenManager.instance.Dimensions.Y / 2;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (MapSprite map in Maps)
            {
                map.image.Draw(spriteBatch);
            }
        }
    }
}
