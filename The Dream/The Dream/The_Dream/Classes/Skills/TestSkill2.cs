using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes.Skills
{
    [Serializable]
    public class TestSkill2 : Skill
    {
        public TestSkill2()
        {
            image.Path = "Gameplay/Skills/TestSkill2/Sprite";
            icon.Path = "Gameplay/Skills/TestSkill2/Icon";
            SkillID = 2;
            skillDamage = 5;
            startLag = 0;
            endLag = 200;
            cooldown = 0;
            description = "Attacks in front of you.";
            parentSkill = new TestSkill();
            requiredSkills.Add(new TestSkill());
            offset = new Vector2(0, -200);
        }
        public override void LoadContent(int fX, int fY)
        {
            hitBox = new Rectangle(X, Y, image.texture.Width, image.texture.Height);
            base.LoadContent(fX, fY);
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
