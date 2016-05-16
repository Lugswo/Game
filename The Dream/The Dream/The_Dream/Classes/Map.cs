﻿using System;
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
        public PlayerUpdate player;
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
        public bool Vertical, Horizontal, EdgeVertical, EdgeHorizontal, Right, Left, Up, Down, Column, Row, Pause, NewMap, ExitTop, ExitBottom, ExitRight, ExitLeft, SongPlaying;
        public Map()
        {
            Moved = Vector2.Zero;
            Screen = new Rectangle(0, 0, ScreenWidth, ScreenHeight);
            Vertical = Horizontal = EdgeHorizontal = EdgeVertical = Pause = false;
            Area = new string[1, 1];
            Area[0, 0] = "Test Map";
            NewMap = SongPlaying = false;
        }
        void NewArea()
        {
            if (ExitBottom == true)
            {

            }
            if (ExitTop == true)
            {

            }
            if (ExitRight == true)
            {

            }
            if (ExitLeft == true)
            {

            }
        }
        public void GetReferences(PlayerUpdate RealPlayer, SoundManager RealSounds)
        {
            player = RealPlayer;
            soundManager = RealSounds;
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
        public void Update(GameTime gameTime)
        {
            if (ScreenManager.Instance.IsTransitioning == false)
            {
                if (SongPlaying == false)
                {
                    SongPlaying = true;
                    soundManager.Music["Megalovania"].soundEffect.Play();
                }
            }
            if (Pause == false)
            {
                EdgeHorizontal = EdgeVertical = Right = Left = Up = Down = Column = Row = false;
                prevMoved = Moved;
                Moved -= player.Velocity;
                DeadZone.X = (int)-Moved.X + OriginalDeadZone.X;
                DeadZone.Width = OriginalDeadZone.Width;
                DeadZone.Y = (int)-Moved.Y + OriginalDeadZone.Y;
                DeadZone.Height = OriginalDeadZone.Height;
                if (!(DeadZone.Contains(Screen)))
                {
                    if (Screen.Y < DeadZone.Y || Screen.Height - 1 > DeadZone.Height - Moved.Y)
                    {
                        Vertical = true;
                        foreach (MapSprite map in Maps)
                        {
                            Moved.Y = prevMoved.Y;
                            map.image.Position.Y = map.OriginalPosition.Y - Moved.Y;
                        }
                    }
                    if (Screen.X < DeadZone.X || Screen.Width > DeadZone.Width - Moved.X)
                    {
                        Horizontal = true;
                        foreach (MapSprite map in Maps)
                        {
                            Moved.X = prevMoved.X;
                            map.image.Position.X = map.OriginalPosition.X - Moved.X;
                        }
                    }
                }
                if (NewMap == true)
                {
                    NewArea();
                    SongPlaying = false;
                }
                foreach (MapSprite map in Blanks)
                {
                    map.HitBox.X = (int)-Moved.X + (int)map.OriginalPosition.X;
                    map.HitBox.Y = (int)-Moved.Y + (int)map.OriginalPosition.Y;
                    if ((player.HitBox.Left - 1 > map.HitBox.Left && player.HitBox.Left - 1 < map.HitBox.Right) || (player.HitBox.Right - 1 > map.HitBox.Left && player.HitBox.Right - 1 < map.HitBox.Right))
                    {
                        Column = true;
                    }
                    if ((player.HitBox.Top + 1 > map.HitBox.Top && player.HitBox.Top + 1 < map.HitBox.Bottom) || (player.HitBox.Bottom - 5 > map.HitBox.Top && player.HitBox.Bottom - 5 < map.HitBox.Bottom))
                    {
                        Row = true;
                    }
                    if (Column == true)
                    {
                        if (player.HitBox.Top <= map.HitBox.Bottom)
                        {
                            Up = true;
                        }
                        if (player.HitBox.Bottom >= map.HitBox.Top)
                        {
                            Down = true;
                        }
                    }
                    if (Row == true)
                    {
                        if (player.HitBox.Left <= map.HitBox.Right)
                        {
                            Left = true;
                        }
                        if (player.HitBox.Right >= map.HitBox.Left)
                        {
                            Right = true;
                        }
                    }
                    if (Column == true)
                    {
                        if (Up == true && Down == true)
                        {
                            EdgeVertical = true;
                            Moved.Y = prevMoved.Y;
                        }
                    }
                    if (Row == true)
                    {
                        if (Right == true && Left == true)
                        {
                            EdgeHorizontal = true;
                            Moved.X = prevMoved.X;
                        }
                    }
                }
                foreach (MapSprite map in Maps)
                {
                    map.image.Update(gameTime);
                }
                if (Horizontal == false && EdgeHorizontal == false && player.Attacking == false)
                {
                    foreach (MapSprite map in Maps)
                    {
                        map.image.Position.X = map.OriginalPosition.X - Moved.X;
                    }
                }
                if (Vertical == false && EdgeVertical == false && player.Attacking == false)
                {
                    foreach (MapSprite map in Maps)
                    {
                        map.image.Position.Y = map.OriginalPosition.Y - Moved.Y;
                    }
                }
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
