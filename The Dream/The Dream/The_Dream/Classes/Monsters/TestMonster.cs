using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes.Monsters
{
    public class TestMonster : Monster
    {
        Rectangle Vision;
        Items.TestItem testItem;
        public TestMonster()
        {
            MonsterID = 1;
        }
        public override void LoadContent()
        {
            SetItem<Items.TestItem>(ref testItem);
            image.Path = "Gameplay/Monsters/TestMonster";
            image.spriteSheetEffect.AmountOfFrames.X = 2;
            image.spriteSheetEffect.AmountOfFrames.Y = 1;
            base.LoadContent();
            XSpawn = 2;
            YSpawn = 1;
            Health = 1;
            Armor = 0;
            Attack = 1;
            EXP = 100;
            moveSpeed = 5;
            hitTimer = 100;
            Vision = new Rectangle(X - 100, Y - 100, 200, 200);
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime, Player player)
        {
            base.Update(gameTime, player);
            hitCounter += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            Vision = new Rectangle(X - 500, Y - 500, 1000 + image.texture.Width, 1000 + image.texture.Height);
            if (player.AreaX == AreaX && player.AreaY == AreaY)
            {
                if (Vision.Intersects(player.HitBox))
                {
                    if (X < player.X - moveSpeed)
                    {
                        X = X + moveSpeed;
                    }
                    else if (X > player.X + moveSpeed)
                    {
                        X = X - moveSpeed;
                    }
                    if (Y < player.Y - moveSpeed)
                    {
                        Y = Y + moveSpeed;
                    }
                    else if (Y > player.Y + moveSpeed)
                    {
                        Y = Y - moveSpeed;
                    }
                }
                if (hitCounter > hitTimer)
                {
                    if (Hitbox.Intersects(player.HitBox))
                    {
                        player.Health -= Attack;
                        player.inCombat = true;
                        player.combatTimer = 0;
                        hitCounter = 0;
                    }
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
