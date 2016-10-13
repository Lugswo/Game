using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes.Monsters
{
    public class TestMonster2 : Monster
    {
        public override void LoadContent()
        {
            image.Path = "Gameplay/Monsters/TestMonster2";
            image.spriteSheetEffect.AmountOfFrames.X = 2;
            image.spriteSheetEffect.AmountOfFrames.Y = 1;
            base.LoadContent();
            MonsterID = 2;
            XSpawn = 1;
            YSpawn = 2;
            Health = 1;
            Armor = 0;
            Attack = 0;
            EXP = 100;
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime, Player player)
        {
            base.Update(gameTime, player);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
