using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace The_Dream.Classes
{
    public class UpdateGameMenu
    {
        public Image PlayerImage, Level, charName;
        public int MenuNumber;
        public bool InMenu, paused, releasedPause;
        public GameMenu.MenuTab menu;
        Dictionary<int, GameMenu.MenuTab> dMenu;
        public GameMenu.Stats stats;
        public GameMenu.Equipment equipment;
        public GameMenu.Inventory inventory;
        public GameMenu.Skills skills;
        public GameMenu.Relics relics;
        public GameMenu.Beastiary beastiary;
        public GameMenu.Map map;
        public GameMenu.Save save;
        public GameMenu.Settings settings;
        public UpdateGameMenu()
        {
            MenuNumber = 0;
            Level = new Image();
            charName = new Image();
            paused = false;
            releasedPause = true;
            dMenu = new Dictionary<int, GameMenu.MenuTab>();
        }
        void SetMenu<T>(ref T menu, int ID)
        {
            if (menu == null)
            {
                menu = (T)Activator.CreateInstance(typeof(T));
            }
            dMenu.Add(ID, (menu as GameMenu.MenuTab));
        }
        public void LoadContent(Player player)
        {
            PlayerImage = new Image();
            PlayerImage.Path = "Gameplay/Characters/Player/Player";
            PlayerImage.Effects = "ShowSpriteEffect";
            PlayerImage.LoadContent();
            PlayerImage.showSpriteEffect.AmountOfSprites.X = 6;
            PlayerImage.showSpriteEffect.AmountOfSprites.Y = 16;
            PlayerImage.showSpriteEffect.Sprite.X = 1;
            PlayerImage.showSpriteEffect.Sprite.Y = 0;
            Level.Text = player.Level.ToString();
            Level.LoadContent();
            charName.Text = player.Name;
            charName.LoadContent();
            PlayerImage.Position.X = ((ScreenManager.instance.Dimensions.X - 1300) / 2) + 118;
            PlayerImage.Position.Y = ((ScreenManager.instance.Dimensions.Y - 835) / 2) + 100;
            Level.Position.X = PlayerImage.Position.X + 25;
            Level.Position.Y = PlayerImage.Position.Y + 126;
            charName.Position.X = ((ScreenManager.instance.Dimensions.X - 1300) / 2) + (300 - charName.Font.MeasureString(charName.Text).X) / 2;
            charName.Position.Y = ((ScreenManager.instance.Dimensions.Y - 835) / 2);
            SetMenu<GameMenu.Stats>(ref stats, 0);
            SetMenu<GameMenu.Equipment>(ref equipment, 1);
            SetMenu<GameMenu.Inventory>(ref inventory, 2);
            SetMenu<GameMenu.Skills>(ref skills, 3);
            SetMenu<GameMenu.Relics>(ref relics, 4);
            SetMenu<GameMenu.Beastiary>(ref beastiary, 5);
            SetMenu<GameMenu.Map>(ref map, 6);
            SetMenu<GameMenu.Save>(ref save, 7);
            SetMenu<GameMenu.Settings>(ref settings, 8);
            stats.LoadContent(player);
            equipment.LoadContent(player);
            inventory.LoadContent();
            skills.LoadContent(player);
            relics.LoadContent();
            beastiary.LoadContent();
            map.LoadContent();
            save.LoadContent();
            settings.LoadContent();
            PlayerImage.Layer = .95f;
            charName.Layer = .95f;
            Level.Layer = .95f;
        }
        public void UnloadContent()
        {
            PlayerImage.UnloadContent();
            Level.UnloadContent();
        }
        public void Update(GameTime gameTime, Player player)
        {
            Level.Text = player.Level.ToString();
            if (paused == true)
            {
                if (InputManager.Instance.KeyPressed(Keys.Escape))
                {
                    paused = false;
                    menu.inCategory = false;
                    InMenu = false;
                }
            }
            if (paused == false)
            {
                if (InputManager.Instance.KeyUp(Keys.Escape))
                {
                    releasedPause = true;
                }
            }
            if (releasedPause == true)
            {
                if (InputManager.Instance.KeyPressed(Keys.Escape))
                {
                    paused = true;
                    releasedPause = false;
                }
            }
            if (paused == true)
            {
                Level.Update(gameTime);
                PlayerImage.Update(gameTime);
                if (InMenu == false)
                {
                    if (InputManager.Instance.KeyPressed(Keys.Left))
                    {
                        MenuNumber--;
                    }
                    if (InputManager.Instance.KeyPressed(Keys.Right))
                    {
                        MenuNumber++;
                    }
                    if (InputManager.Instance.KeyPressed(Keys.X))
                    {
                        paused = false;
                    }
                }
                if (MenuNumber > 8)
                {
                    MenuNumber = 0;
                }
                if (MenuNumber < 0)
                {
                    MenuNumber = 8;
                }
                menu = dMenu[MenuNumber];
                menu.Update(gameTime, player, InMenu);
                if (InMenu == true)
                {
                    if (menu.subCat == true)
                    {
                        if (InputManager.Instance.KeyPressed(Keys.Z))
                        {
                            menu.inCategory = true;
                        }
                    }
                    else
                    {
                        if (InputManager.Instance.KeyPressed(Keys.X))
                        {
                            InMenu = false;
                        }
                    }
                    if (menu.inCategory == true)
                    {
                        if (InputManager.Instance.KeyPressed(Keys.X))
                        {
                            menu.inCategory = false;
                        }
                    }
                    else
                    {
                        if (InputManager.Instance.KeyPressed(Keys.X))
                        {
                            InMenu = false;
                        }
                    }
                }
                if (InputManager.Instance.KeyPressed(Keys.Z))
                {
                    InMenu = true;
                }
                if (MenuNumber < 0)
                {
                    MenuNumber++;
                }
                if (MenuNumber > 8)
                {
                    MenuNumber--;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch, Player player)
        {
            if (paused == true)
            {
                menu.Draw(spriteBatch, InMenu, player);
                Level.DrawString(spriteBatch);
                PlayerImage.Draw(spriteBatch);
                charName.Draw(spriteBatch);
            }
        }
    }
}
