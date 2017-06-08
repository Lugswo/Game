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
        public List<Items.Item> inventory;
        int category, selectedItem;
        bool setImage, enterCategory;
        Vector2 pos;
        Image categoryImage;
        Image inCategoryImage;
        public Inventory()
        {
            category = 0;
            inventory = new List<Items.Item>();
            setImage = false;
            categoryImage = new Image();
            inCategoryImage = new Image();
            categoryImage.LoadContent();
            inCategoryImage.LoadContent();
            subCat = true;
            enterCategory = true;
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
                    if (inventory.Count > 0 && enterCategory == true)
                    {
                        enterCategory = false;
                        inventory[0].selected = true;
                        selectedItem = 0;
                    }
                    if (inventory.Count > 0)
                    {
                        if (selectedItem > 0)
                        {
                            if (InputManager.Instance.KeyPressed(Keys.Left))
                            {
                                inventory[selectedItem].selected = false;
                                selectedItem -= 1;
                                inventory[selectedItem].selected = true;
                            }
                        }
                        if (selectedItem < inventory.Count - 1)
                        {
                            if (InputManager.Instance.KeyPressed(Keys.Right))
                            {
                                inventory[selectedItem].selected = false;
                                selectedItem += 1;
                                inventory[selectedItem].selected = true;
                            }
                        }
                        if (selectedItem >= 5)
                        {
                            if (InputManager.Instance.KeyPressed(Keys.Up))
                            {
                                inventory[selectedItem].selected = false;
                                selectedItem -= 6;
                                inventory[selectedItem].selected = true;
                            }
                        }
                        if (selectedItem <= inventory.Count - 6)
                        {
                            if (InputManager.Instance.KeyPressed(Keys.Down))
                            {
                                inventory[selectedItem].selected = false;
                                selectedItem += 6;
                                inventory[selectedItem].selected = true;
                            }
                        }
                    }
                }
                if (setImage == false)
                {
                    inventory.Clear();
                    pos = image.Position;
                    if (category == 0)
                    {
                        DifferentImage("Gear", categoryImage, inCategoryImage, pos);
                    }
                    else if (category == 1)
                    {
                        DifferentImage("Items", categoryImage, inCategoryImage, pos);
                    }
                    foreach (Items.Item item in player.inventory)
                    {
                        string inventoryItems = string.Empty;
                        foreach (Items.Item item2 in inventory)
                        {
                            inventoryItems += item2.name.Text;
                        }
                        if (category == 0)
                        {
                            if (item.category == Items.Item.Subcategory.GEAR)
                            {
                                if (!(inventoryItems.Contains(item.name.Text)))
                                {
                                    Items.Item temp = (Items.Item)Activator.CreateInstance(item.GetType());
                                    temp.LoadContent();
                                    inventory.Add(temp);
                                }
                                else
                                {
                                    foreach (Items.Item item3 in inventory)
                                    {
                                        if (item.name.Text == item3.name.Text)
                                        {
                                            item3.numberOwned++;
                                            item3.numberOwnedImage.Text = item3.numberOwned.ToString() + "x";
                                        }
                                    }
                                }
                            }
                        }
                        else if (category == 1)
                        {
                            if (item.category == Items.Item.Subcategory.ITEM)
                            {
                                if (!(inventoryItems.Contains(item.name.Text)))
                                {
                                    Items.Item temp = (Items.Item)Activator.CreateInstance(item.GetType());
                                    temp.LoadContent();
                                    inventory.Add(temp);
                                }
                                else
                                {
                                    foreach (Items.Item item3 in inventory)
                                    {
                                        if (item.name.Text == item3.name.Text)
                                        {
                                            item3.numberOwned++;
                                            item3.numberOwnedImage.Text = item3.numberOwned.ToString() + "x";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    int count = 0;
                    int yCount = 0;
                    foreach (Items.Item item in inventory)
                    {
                        if (count == 6)
                        {
                            count = 0;
                            yCount++;
                        }
                        item.inventoryImage.Alpha = 1.0f;
                        item.inventoryImage.Layer = .95f;
                        item.itemFrame.Position.X = categoryOffset + (count * 130) + 10;
                        item.itemFrame.Position.Y = yOffset + 10 + (yCount * 130);
                        item.itemFrame.Layer = .94f;
                        item.inventoryImage.Position.X = (item.itemFrame.Position.X + (item.itemFrame.texture.Width / 2)) - (item.inventoryImage.texture.Width / 2);
                        item.inventoryImage.Position.Y = (item.itemFrame.Position.Y + (item.itemFrame.texture.Height / 2)) - (item.inventoryImage.texture.Height / 2);
                        item.numberOwnedImage.Position.X = item.itemFrame.Position.X + item.itemFrame.texture.Width - item.numberOwnedImage.Font.MeasureString(item.numberOwnedImage.Text).X;
                        item.numberOwnedImage.Position.Y = item.itemFrame.Position.Y + item.itemFrame.texture.Height - item.numberOwnedImage.Font.MeasureString(item.numberOwnedImage.Text).Y;
                        count++;
                    }
                    setImage = true;
                }
                base.Update(gameTime, player, inMenu);
            }
            else
            {
                setImage = false;
            }
        }
        public override void Draw(SpriteBatch spriteBatch, bool inMenu, Player player)
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
                    foreach (Items.Item item in inventory)
                    {
                        if (item.selected == true)
                        {
                            item.itemFrame.Draw(spriteBatch);
                            item.inventoryImage.Draw(spriteBatch);
                            item.name.DrawString(spriteBatch);
                            item.description.DrawString(spriteBatch);
                            item.numberOwnedImage.DrawString(spriteBatch);
                        }
                        else
                        {
                            item.itemFrame.DrawFaded(spriteBatch);
                            item.inventoryImage.DrawFaded(spriteBatch);
                            item.numberOwnedImage.DrawString(spriteBatch);
                        }
                    }
                }
                else
                {
                    foreach (Items.Item item in inventory)
                    {
                        item.itemFrame.DrawFaded(spriteBatch);
                        item.inventoryImage.DrawFaded(spriteBatch);
                        item.numberOwnedImage.DrawString(spriteBatch);
                    }
                }
            }
        }
    }
}
