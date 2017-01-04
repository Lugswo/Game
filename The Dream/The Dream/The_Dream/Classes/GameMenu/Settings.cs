using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace The_Dream.Classes.GameMenu
{
    public class Settings : MenuTab
    {
        bool hotKey;
        public Settings()
        {
            image.Path = "Gameplay/GUI/Menu/Settings/Settings";
            inImage.Path = "Gameplay/GUI/Menu/Settings/InSettings";
            subCat = true;
            hotKey = false;
        }
        public override void LoadContent()
        {
            base.LoadContent();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime, Player player, bool inMenu)
        {
            base.Update(gameTime, player, inMenu);
            if (inMenu == true)
            {
                if (inCategory == true)
                {
                    if (InputManager.Instance.KeyPressed(Keys.Z))
                    {
                        hotKey = true;
                        while (hotKey == true)
                        {
                            if (player.skills.Count > 0)
                            {
                                if (InputManager.Instance.KeyPressed(Keys.Z))
                                {
                                    player.shiftSkill = player.skills[0];
                                    hotKey = false;
                                }
                            }
                            else
                            {
                                hotKey = false;
                            }
                        }
                    }
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch, bool inMenu, Player player)
        {
            if (hotKey == true)
            {
                foreach (Classes.Skills.Skill skill in player.skills)
                {
                    skill.icon.Draw(spriteBatch);
                }
            }
            base.Draw(spriteBatch, inMenu, player);
        }
    }
}
