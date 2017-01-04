using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace The_Dream.Classes.GameMenu
{
    public class Skills : MenuTab
    {
        public List<Classes.Skills.Skill> skillList;
        public Classes.Skills.TestSkill testSkill;
        public Skills()
        {
            image.Path = "Gameplay/GUI/Menu/Skills";
            inImage.Path = "Gameplay/GUI/Menu/InSkills";
            skillList = new List<Classes.Skills.Skill>();
            testSkill = new Classes.Skills.TestSkill();
        }
        public override void LoadOnPause(Player player)
        {
            base.LoadOnPause(player);
        }
        public override void LoadContent()
        {
            base.LoadContent();
            testSkill.LoadIcon();
            skillList.Add(testSkill);
            int count = 0;
            foreach (Classes.Skills.Skill skill in skillList)
            {
                count++;
                skill.icon.Position.X += count * 50;
            }
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime, Player player, bool inMenu)
        {
            player.skillPointsImage.color = new Color(255, 255, 255, 255);
            player.skillPointsImage.Text = "Skill Points: " + player.skillPoints.ToString();
            if (inMenu == true)
            {
                if (InputManager.Instance.KeyPressed(Keys.Z))
                {
                    if (player.skillPoints > 0)
                    {
                        player.skills.Add(skillList[0]);
                        skillList[0].learned = true;
                        player.skillPoints--;
                    }
                    else
                    {

                    }
                }
            }
            base.Update(gameTime, player, inMenu);
        }
        public override void Draw(SpriteBatch spriteBatch, bool inMenu, Player player)
        {
            base.Draw(spriteBatch, inMenu, player);
            foreach (Classes.Skills.Skill skill in skillList)
            {
                if (skill.learned == true)
                {
                    skill.icon.Draw(spriteBatch);
                }
                else
                {
                    skill.icon.DrawFaded(spriteBatch);
                }
            }
            player.skillPointsImage.DrawString(spriteBatch);
        }
    }
}
