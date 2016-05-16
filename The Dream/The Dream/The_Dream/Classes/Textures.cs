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
    public class Textures
    {
        public Vector2 Velocity;
        public Map map;
        public PlayerUpdate player;
        [XmlElement("Sprite")]
        public List<Sprite> Sprites;
        public float MoveSpeed;
        public int ScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        public int ScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        bool Column, Row, Up, Down, Left, Right;
        public Textures()
        {
            Velocity = Vector2.Zero;
            Sprites = new List<Sprite>();
        }
        public void GetPlayerMap(PlayerUpdate RealPlayer, Map RealMap)
        {
            map = RealMap;
            player = RealPlayer;
        }
        public void LoadContent()
        {
            foreach (Sprite sprite in Sprites)
            {
                sprite.image.LoadContent();
                sprite.OriginalPosition = sprite.image.Position;
                sprite.Hitbox = new Rectangle((int)sprite.image.Position.X, (int)sprite.image.Position.Y, sprite.image.texture.Width, sprite.image.texture.Height);
            }
        }
        public void UnloadContent()
        {
            foreach (Sprite sprite in Sprites)
            {
                sprite.image.UnloadContent();
            }
        }
        public void Update(GameTime gameTime)
        {
            if (map.Pause == false)
            {
                map.EdgeHorizontal = map.EdgeVertical = Right = Left = Up = Down = Column = Row = false;
                foreach (Sprite sprite in Sprites)
                {
                    sprite.Hitbox = new Rectangle((int)sprite.image.Position.X, (int)sprite.image.Position.Y, sprite.image.texture.Width, sprite.image.texture.Height);
                    if ((player.HitBox.Left - 1 > sprite.Hitbox.Left && player.HitBox.Left - 1 < sprite.Hitbox.Right) || (player.HitBox.Right - 1 > sprite.Hitbox.Left && player.HitBox.Right - 1 < sprite.Hitbox.Right))
                    {
                        Column = true;
                    }
                    if ((player.HitBox.Top + 1 > sprite.Hitbox.Top && player.HitBox.Top + 1 < sprite.Hitbox.Bottom) || (player.HitBox.Bottom - 5 > sprite.Hitbox.Top && player.HitBox.Bottom - 5 < sprite.Hitbox.Bottom))
                    {
                        Row = true;
                    }
                    if (Column == true)
                    {
                        if (player.HitBox.Top <= sprite.Hitbox.Bottom)
                        {
                            Up = true;
                        }
                        if (player.HitBox.Bottom >= sprite.Hitbox.Top)
                        {
                            Down = true;
                        }
                    }
                    if (Row == true)
                    {
                        if (player.HitBox.Left <= sprite.Hitbox.Right)
                        {
                            Left = true;
                        }
                        if (player.HitBox.Right >= sprite.Hitbox.Left)
                        {
                            Right = true;
                        }
                    }
                    if (Column == true)
                    {
                        if (Up == true && Down == true)
                        {
                            map.EdgeVertical = true;
                            map.Moved.Y = map.prevMoved.Y;
                            if (map.Vertical == true)
                            {
                                player.PlayerMoved.Y = player.PrevPlayerMoved.Y;
                            }
                        }
                    }
                    if (Row == true)
                    {
                        if (Right == true && Left == true)
                        {
                            map.EdgeHorizontal = true;
                            map.Moved.X = map.prevMoved.X;
                            if (map.Horizontal == true)
                            {
                                player.PlayerMoved.X = player.PrevPlayerMoved.X;
                            }
                        }
                    }
                }
                if (map.Horizontal == false && map.EdgeHorizontal == false && player.Attacking == false)
                {
                    foreach (Sprite sprite in Sprites)
                    {
                        sprite.image.Position.X = sprite.OriginalPosition.X - map.Moved.X;
                    }
                }
                if (map.Vertical == false && map.EdgeVertical == false && player.Attacking == false)
                {
                    foreach (Sprite sprite in Sprites)
                    {
                        sprite.image.Position.Y = sprite.OriginalPosition.Y - map.Moved.Y;
                    }
                }
                foreach (Sprite sprite in Sprites)
                {
                    sprite.image.Update(gameTime);
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite sprite in Sprites)
            {
                sprite.image.Draw(spriteBatch);
            }
        }
    }
}
