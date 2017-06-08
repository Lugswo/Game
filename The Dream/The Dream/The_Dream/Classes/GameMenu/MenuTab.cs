using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes.GameMenu
{
    public class MenuTab
    {
        public Image image;
        public Image inImage;
        public bool inCategory;
        public bool subCat;
        public const int categoryOffset = 760;
        public const int xOffset = 610;
        public const int yOffset = 156;
        public class Category
        {
            public Image image;
        }
        public MenuTab()
        {
            image = new Image();
            inImage = new Image();
            subCat = false;
            inCategory = false;
        }
        public void DifferentImage(string s, Image categoryImage, Image inCategoryImage, Vector2 pos)
        {
            categoryImage.UnloadContent();
            inCategoryImage.UnloadContent();
            categoryImage = new Image();
            inCategoryImage = new Image();
            categoryImage.Path = "Gameplay/GUI/Menu/Inventory/" + s;
            inCategoryImage.Path = "Gameplay/GUI/Menu/Inventory/In" + s;
            categoryImage.LoadContent();
            inCategoryImage.LoadContent();
            categoryImage.Position = pos;
            inCategoryImage.Position = pos;
            inCategoryImage.Layer = .91f;
        }
        public virtual void LoadOnPause(Player player)
        {

        }
        public virtual void LoadContent()
        {
            image.LoadContent();
            image.Position.X = (ScreenManager.instance.Dimensions.X - image.texture.Width) / 2;
            image.Position.Y = (ScreenManager.instance.Dimensions.Y - image.texture.Height) / 2;
            inImage.LoadContent();
            inImage.Position.X = (ScreenManager.instance.Dimensions.X - image.texture.Width) / 2;
            inImage.Position.Y = (ScreenManager.instance.Dimensions.Y - image.texture.Height) / 2;
            image.Layer = .8f;
            inImage.Layer = .8f;
        }
        public virtual void LoadContent(Player player)
        {
            LoadContent();
        }
        public virtual void UnloadContent()
        {
            image.UnloadContent();
        }
        public virtual void Update(GameTime gameTime, Player player, bool inMenu)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch, bool inMenu)
        {
            if (inMenu == true)
            {
                inImage.Draw(spriteBatch);
            }
            else
            {
                image.Draw(spriteBatch);
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch, bool inMenu, Player player)
        {
            if (inMenu == true)
            {
                inImage.Draw(spriteBatch);
            }
            else
            {
                image.Draw(spriteBatch);
            }
        }
    }
}
