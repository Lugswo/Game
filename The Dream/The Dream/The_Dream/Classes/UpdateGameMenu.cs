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
        [XmlElement("Item")]
        public List<Image> MenuItems;
        public Image PlayerImage, Level, charName;
        public int MenuNumber;
        public bool InMenu, paused, releasedPause;
        public UpdateGameMenu()
        {
            MenuNumber = 0;
            Level = new Image();
            charName = new Image();
            paused = false;
            releasedPause = true;
        }
        void LoadMenu(int menu, Player player)
        {
            foreach (Image image in MenuItems)
            {
                image.Alpha = 0;
            }
            if (InMenu == false)
            {
                MenuItems[menu].Alpha = 1;
            }
            else
            {
                MenuItems[menu + 9].Alpha = 1;
            }
            if (menu == 0)
            {
                
            }
            if (menu == 1)
            {
                
            }
            if (menu == 2)
            {

            }
            if (menu == 3)
            {

            }
            if (menu == 4)
            {

            }
            if (menu == 5)
            {

            }
            if (menu == 6)
            {

            }
            if (menu == 7)
            {
                if (InMenu == true)
                {
                    if (InputManager.Instance.KeyPressed(Keys.Z))
                    {
                        XmlDocument SaveFile = new XmlDocument();
                        SaveFile.Load("Load/Gameplay/SaveFile.xml");
                        XmlNode node;
                        node = SaveFile.DocumentElement;
                        foreach (XmlNode node1 in node.ChildNodes)
                        {
                            if (node1.Name == "Strength")
                            {
                                node1.InnerText = player.Strength.ToString();
                            }
                            if (node1.Name == "Defense")
                            {
                                node1.InnerText = player.Defense.ToString();
                            }
                            if (node1.Name == "Dexterity")
                            {
                                node1.InnerText = player.Dexterity.ToString();
                            }
                            if (node1.Name == "Intelligence")
                            {
                                node1.InnerText = player.Intelligence.ToString();
                            }
                            if (node1.Name == "Speed")
                            {
                                node1.InnerText = player.Speed.ToString();
                            }
                            if (node1.Name == "Level")
                            {
                                node1.InnerText = player.Level.ToString();
                            }
                            if (node1.Name == "EXP")
                            {
                                node1.InnerText = player.EXP.ToString();
                            }
                            if (node1.Name == "Health")
                            {
                                node1.InnerText = player.Health.ToString();
                            }
                            if (node1.Name == "Energy")
                            {
                                node1.InnerText = player.Energy.ToString();
                            }
                            if (node1.Name == "StatPoints")
                            {
                                node1.InnerText = player.StatPoints.ToString();
                            }
                        }
                        SaveFile.Save("Load/Gameplay/SaveFile.xml");
                    }
                }
            }
            if (menu == 8)
            {

            }
        }
        public void LoadContent(Player player)
        {
            PlayerImage.LoadContent();
            Level.Text = player.Level.ToString();
            Level.LoadContent();
            charName.Text = player.Name;
            charName.LoadContent();
            foreach (Image image in MenuItems)
            {
                image.LoadContent();
                image.Position.X = (ScreenManager.instance.Dimensions.X - image.texture.Width) / 2;
                image.Position.Y = (ScreenManager.instance.Dimensions.Y - image.texture.Height) / 2;
            }
            PlayerImage.Position.X = MenuItems[0].Position.X + 118;
            PlayerImage.Position.Y = MenuItems[0].Position.Y + 100;
            charName.Position.X = MenuItems[0].Position.X + (300 - charName.Font.MeasureString(charName.Text).X) / 2;
            charName.Position.Y = MenuItems[0].Position.Y;
        }
        public void UnloadContent()
        {
            PlayerImage.UnloadContent();
            Level.UnloadContent();
            foreach (Image image in MenuItems)
            {
                image.UnloadContent();
            }
        }
        public void Update(GameTime gameTime, Player player)
        {
            if (paused == true)
            {
                if (InputManager.Instance.KeyPressed(Keys.Escape))
                {
                    paused = false;
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
                Level = new Image();
                Level.Text = player.Level.ToString();
                Level.Position.X = PlayerImage.Position.X + 25;
                Level.Position.Y = PlayerImage.Position.Y + 126;
                Level.LoadContent();
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
                if (InMenu == true)
                {
                    if (InputManager.Instance.KeyPressed(Keys.X))
                    {
                        InMenu = false;
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
                LoadMenu(MenuNumber, player);
                foreach (Image image in MenuItems)
                {
                    image.Update(gameTime);
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (paused == true)
            {
                foreach (Image image in MenuItems)
                {
                    image.Draw(spriteBatch);
                }
                Level.Draw(spriteBatch);
                PlayerImage.Draw(spriteBatch);
                charName.Draw(spriteBatch);
            }
        }
    }
}
