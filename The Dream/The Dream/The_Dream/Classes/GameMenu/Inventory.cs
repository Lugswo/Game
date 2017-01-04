using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace The_Dream.Classes.GameMenu
{
    public class Inventory : MenuTab
    {
        public List<Item> inventory;
        int category;
        bool setImage;
        Vector2 pos;
        Image categoryImage;
        Image inCategoryImage;
        public Inventory()
        {
            category = 0;
            inventory = new List<Item>();
            setImage = false;
            categoryImage = new Image();
            inCategoryImage = new Image();
            categoryImage.LoadContent();
            inCategoryImage.LoadContent();
            subCat = true;
        }
        void DifferentImage(string s)
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
            inCategoryImage.Layer = 1.0f;
        }
        public override void LoadContent()
        {
            image.Path = "Gameplay/GUI/Menu/Inventory";
            inImage.Path = "Gameplay/GUI/Menu/InInventory";
            base.LoadContent();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime, Player player, bool inMenu)
        {
            if (inMenu == true)
            {
                if (inCategory == false)
                {
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
                if (setImage == false)
                {
                    inventory.Clear();
                    pos = image.Position;
                    if (category == 0)
                    {
                        DifferentImage("Gear");
                    }
                    else if (category == 1)
                    {
                        DifferentImage("Items");
                    }
                    foreach (Item item in player.inventory)
                    {
                        if (category == 0)
                        {
                            if (item.category == Item.Subcategory.GEAR)
                            {
                                inventory.Add(item);
                            }
                        }
                        else if (category == 1)
                        {
                            if (item.category == Item.Subcategory.ITEM)
                            {
                                inventory.Add(item);
                            }
                        }
                    }
                    setImage = true;
                }
                int count = 0;
                foreach (Item item in inventory)
                {
                    count++;
                    item.inventoryImage.Alpha = 1.0f;
                    item.itemFrame.Position.X = count * 50;
                    item.itemFrame.Layer = 1.0f;
                }
                base.Update(gameTime, player, inMenu);
            }
            else
            {
                setImage = false;
            }
        }
        public override void Draw(SpriteBatch spriteBatch, bool inMenu)
        {
            base.Draw(spriteBatch, inMenu);
            if (inMenu == true)
            {
                if (categoryImage.Path != string.Empty)
                {
                    categoryImage.Draw(spriteBatch);
                }
                if (inCategory == true)
                {
                    inCategoryImage.Draw(spriteBatch);
                }
                foreach (Item item in inventory)
                {
                    item.itemFrame.Draw(spriteBatch);
                    item.inventoryImage.Draw(spriteBatch);
                }
            }
        }
    }
}
