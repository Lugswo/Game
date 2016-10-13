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
        public override void LoadContent()
        {
            image.Path = "Gameplay/Monsters/TestMonster";
            image.spriteSheetEffect.AmountOfFrames.X = 2;
            image.spriteSheetEffect.AmountOfFrames.Y = 1;
            base.LoadContent();
            MonsterID = 1;
            XSpawn = 2;
            YSpawn = 1;
            Health = 1;
            Armor = 0;
            Attack = 0;
            EXP = 100;
            Vision = new Rectangle(X - 100, Y - 100, 200, 200);
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime, Player player)
        {
            base.Update(gameTime, player);
            Vision = new Rectangle(X - 500, Y - 500, 1000 + image.texture.Width, 1000 + image.texture.Height);
            if (player.AreaX == AreaX && player.AreaY == AreaY)
            {
                if (Vision.Intersects(player.HitBox))
                {
                    if (X < player.X - 5)
                    {
                        X = X + 5;
                    }
                    else if (X > player.X + 5)
                    {
                        X = X - 5;
                    }
                    if (Y < player.Y - 5)
                    {
                        Y = Y + 5;
                    }
                    else if (Y > player.Y + 5)
                    {
                        Y = Y - 5;
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
