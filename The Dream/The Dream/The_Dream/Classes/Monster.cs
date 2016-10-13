using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes
{
    public class Monster
    {
        public Image image = new Image();
        public Rectangle Hitbox;
        public bool CanSpawn, Boss, IsAlive;
        public int XSpawn, YSpawn, Health, Armor, Attack, MonsterID, EXP, X, Y, AreaX, AreaY, pX, pY;
        public Vector2 OriginalPosition;
        public Monster()
        {
            CanSpawn = Boss = false;
            IsAlive = true;
            OriginalPosition = Vector2.Zero;
            image.Effects = "SpriteSheetEffect";
        }
        public virtual void LoadContent()
        {
            image.LoadContent();
        }
        public virtual void UnloadContent()
        {
            image.UnloadContent();
        }
        public virtual void Update(GameTime gameTime, Player player)
        {
            image.Update(gameTime);
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            image.Draw(spriteBatch);
        }
    }
}
