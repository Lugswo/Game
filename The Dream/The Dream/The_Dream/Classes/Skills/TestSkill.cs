using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes.Skills
{
    public class TestSkill : Skill
    {
        public TestSkill()
        {
            image.Path = "Gameplay/Skills/TestSkill/Sprite";
            icon.Path = "Gameplay/Skills/TestSkill/Icon";
            SkillID = 1;
            skillDamage = 1;
            range = 500;
            projSpeed = 15;
            startLag = 0;
            endLag = 100;
            cooldown = 0;
            description = "Shoots a small, damage dealing projectile in front of you.";
            icon.Position.X = ScreenManager.instance.Dimensions.X / 2 - halfIcon;
            icon.Position.Y = ScreenManager.instance.Dimensions.Y / 2 - halfIcon;
        }
        public override void LoadContent(int fX, int fY)
        {
            base.LoadContent(fX, fY);
            X = fX + (25 + image.texture.Width) / 2;
            Y = fY + (25 + image.texture.Height) / 2;
            hitBox = new Rectangle(X, Y, image.texture.Width, image.texture.Height);
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (moved < range)
            {
                moved += projSpeed;
                if (up == true)
                {
                    hitBox.Y -= projSpeed;
                    Y -= projSpeed;
                }
                else if (down == true)
                {
                    hitBox.Y += projSpeed;
                    Y += projSpeed;
                }
                else if (left == true)
                {
                    hitBox.X -= projSpeed;
                    X -= projSpeed;
                }
                else if (right == true)
                {
                    hitBox.X += projSpeed;
                    X += projSpeed;
                }
            }
            else
            {
                despawn = true;
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
