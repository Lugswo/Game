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
        bool hotKey, keyReleased;
        Image shiftKey;
        public Settings()
        {
            image.Path = "Gameplay/GUI/Menu/Settings/Settings";
            inImage.Path = "Gameplay/GUI/Menu/Settings/InSettings";
            subCat = true;
            hotKey = false;
            shiftKey = new Image();
        }
        public override void LoadContent()
        {
            base.LoadContent();
            shiftKey.LoadContent();
            shiftKey.Text = "Shift: No Skill";
            shiftKey.color = new Color(252, 252, 252, 252);
            shiftKey.Position.X = 100;
            shiftKey.Position.Y = 100;
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
                    if (hotKey == false)
                    {
                        if (InputManager.Instance.KeyPressed(Keys.Z))
                        {
                            hotKey = true;
                            keyReleased = false;
                        }
                    }
                    if (InputManager.Instance.KeyUp(Keys.Z))
                    {
                        keyReleased = true;
                    }
                    if (hotKey == true && keyReleased == true)
                    {
                        if (player.skills.Count > 0)
                        {
                            if (InputManager.Instance.KeyPressed(Keys.Z))
                            {
                                //player.binds[0] = player.skills[0];
                                //player.skill1.icon.Position.X = 100 + shiftKey.Font.MeasureString("Shift: ").X;
                                //player.skill1.icon.Position.Y = 100;
                                //shiftKey.Text = "Shift: ";
                                //hotKey = false;
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
        public override void Draw(SpriteBatch spriteBatch, bool inMenu, Player player)
        {
            if (hotKey == true)
            {
                foreach (Classes.Skills.Skill skill in player.skills)
                {
                    skill.icon.Draw(spriteBatch);
                }
            }
            shiftKey.DrawString(spriteBatch);
            //if (player.skill1.icon.texture != null)
            //{
            //    player.skill1.icon.Draw(spriteBatch);
            //}
            base.Draw(spriteBatch, inMenu, player);
        }
    }
}
