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
        public Vector2 Moved;
        public Textures()
        {
            Velocity = Vector2.Zero;
            Sprites = new List<Sprite>();
        }
        public void HorizontalMove()
        {
            foreach (Sprite sprite in Sprites)
            {
                sprite.image.Position.X = sprite.OriginalPosition.X - 10 - Moved.X + ScreenManager.instance.Dimensions.X / 2;
            }
            foreach (Sprite sprite in Sprites)
            {
                sprite.HitBox.X = (int)sprite.OriginalPosition.X - 10 - (int)Moved.X + (int)ScreenManager.instance.Dimensions.X / 2;
                sprite.Left.X = (int)sprite.OriginalPosition.X - 10 -(int)Moved.X + (int)ScreenManager.instance.Dimensions.X / 2;
                sprite.Right.X = (int)sprite.OriginalPosition.X - 10 + (int)sprite.image.texture.Width - (int)Moved.X + (int)ScreenManager.instance.Dimensions.X / 2;
                sprite.Up.X = (int)sprite.OriginalPosition.X - 10 -(int)Moved.X + (int)ScreenManager.instance.Dimensions.X / 2;
                sprite.Down.X = (int)sprite.OriginalPosition.X - 10 - (int)Moved.X + (int)ScreenManager.instance.Dimensions.X / 2;
            }
        }
        public void VerticalMove()
        {
            foreach (Sprite sprite in Sprites)
            {
                sprite.image.Position.Y = sprite.OriginalPosition.Y - Moved.Y + ScreenManager.instance.Dimensions.Y / 2;
            }
            foreach (Sprite sprite in Sprites)
            {
                sprite.HitBox.Y = (int)sprite.OriginalPosition.Y - (int)Moved.Y + (int)ScreenManager.instance.Dimensions.Y / 2;
                sprite.Left.Y = (int)sprite.OriginalPosition.Y - (int)Moved.Y + (int)ScreenManager.instance.Dimensions.Y / 2;
                sprite.Right.Y = (int)sprite.OriginalPosition.Y - (int)Moved.Y + (int)ScreenManager.instance.Dimensions.Y / 2;
                sprite.Up.Y = (int)sprite.OriginalPosition.Y - (int)Moved.Y + (int)ScreenManager.instance.Dimensions.Y / 2;
                sprite.Down.Y = (int)sprite.OriginalPosition.Y + (int)sprite.OriginalPosition.Y - (int)Moved.Y + (int)ScreenManager.instance.Dimensions.Y / 2;
            }
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
                sprite.HitBox = new Rectangle((int)sprite.image.Position.X, (int)sprite.image.Position.Y, sprite.image.texture.Width, sprite.image.texture.Height);
                sprite.Left = new Rectangle((int)sprite.OriginalPosition.X, (int)sprite.OriginalPosition.Y, 1, sprite.image.texture.Height);
                sprite.Right = new Rectangle(sprite.image.texture.Width + (int)sprite.OriginalPosition.X, (int)sprite.OriginalPosition.Y, -1, sprite.image.texture.Height);
                sprite.Up = new Rectangle((int)sprite.OriginalPosition.X, (int)sprite.OriginalPosition.Y, sprite.image.texture.Width, 1);
                sprite.Down = new Rectangle((int)sprite.OriginalPosition.X, sprite.image.texture.Height + (int)sprite.OriginalPosition.Y, sprite.image.texture.Width, -1);
            }
        }
        public void UnloadContent()
        {
            foreach (Sprite sprite in Sprites)
            {
                sprite.image.UnloadContent();
            }
        }
        public void Update(GameTime gameTime, Player player)
        {
            Moved = new Vector2(player.X, player.Y);
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
