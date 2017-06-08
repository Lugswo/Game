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
        int category;
        bool enterCategory, setImage;
        Vector2 pos;
        Image categoryImage;
        Image inCategoryImage;
        public Settings()
        {
            image.Path = "Gameplay/GUI/Menu/Settings/Settings";
            inImage.Path = "Gameplay/GUI/Menu/Settings/InSettings";
            subCat = true;
            pos = new Vector2();
            categoryImage = new Image();
            categoryImage = new Image();
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
                if (inCategory == false)
                {
                    enterCategory = true;
                    if (InputManager.Instance.KeyPressed(Keys.Down))
                    {
                        category++;
                        setImage = false;
                    }
                    if (InputManager.Instance.KeyPressed(Keys.Up))
                    {
                        category--;
                        setImage = false;
                    }
                    if (category < 0)
                    {
                        category++;
                    }
                    if (category > 1)
                    {
                        category--;
                    }
                }
                else
                {
                    if (setImage == false)
                    {
                        pos = image.Position;
                        if (category == 0)
                        {
                            DifferentImage("Gear", categoryImage, inCategoryImage, pos);
                        }
                        else if (category == 1)
                        {
                            DifferentImage("Items", categoryImage, inCategoryImage, pos);
                        }

                    }
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch, bool inMenu, Player player)
        {
            base.Draw(spriteBatch, inMenu, player);
        }
    }
}
