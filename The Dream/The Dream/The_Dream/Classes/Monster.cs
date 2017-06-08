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
        public bool CanSpawn, Boss, IsAlive, Hit;
        public int XSpawn, YSpawn, Health, Armor, Attack, MonsterID, EXP, X, Y, AreaX, AreaY, pX, pY, hitCounter, hitTimer, PositionX, PositionY, moveSpeed;
        public Vector2 OriginalPosition;
        public Random random;
        public List<Items.Item> DropList;
        public List<Items.Item> Drops;
        public Monster()
        {
            CanSpawn = Boss = false;
            IsAlive = true;
            OriginalPosition = Vector2.Zero;
            hitCounter = 0;
            image.Effects = "SpriteSheetEffect";
            DropList = new List<Items.Item>();
            Drops = new List<Items.Item>();
            random = new Random();
        }
        public void SetItem<T>(ref T item)
        {
            if (item == null)
            {
                item = (T)Activator.CreateInstance(typeof(T));
            }
            DropList.Add(item as Items.Item);
        }
        public void Drop()
        {
            int rand = random.Next(0, 1000000);
            foreach (Items.Item item in DropList)
            {
                if (rand <= item.dropChance)
                {
                    item.X = X;
                    item.Y = Y;
                    item.itemAdded = true;
                    item.LoadContent();
                    Drops.Add(item);
                }
            }
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
            Hitbox = new Rectangle(X, Y, image.spriteSheetEffect.FrameWidth, image.spriteSheetEffect.FrameHeight);
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            image.Draw(spriteBatch);
        }
    }
}
