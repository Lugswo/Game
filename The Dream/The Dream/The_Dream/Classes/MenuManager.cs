using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace The_Dream.Classes
{
    public class MenuManager
    {
        Menu menu;
        bool isTransitioning;
        void Transition(GameTime gameTime)
        {
            if (isTransitioning)
            {
                for (int i = 0; i < menu.Items.Count; i++)
                {
                    menu.Items[i].image.Update(gameTime);
                    float first = menu.Items[0].image.Alpha;
                    float last = menu.Items[menu.Items.Count - 1].image.Alpha;
                    if (first == 0.0f && last == 0.0f)
                    {
                        menu.ID = menu.Items[menu.ItemNumber].LinkID;
                    }
                    else if (first == 1.0f && last == 1.0f)
                    {
                        isTransitioning = false;
                        foreach (MenuItem item in menu.Items)
                        {
                            item.image.RestoreEffects();
                        }
                    }
                }
            }
        }
        public MenuManager()
        {
            menu = new Menu();
            menu.OnMenuChange += menu_OnMenuChange;
        }

        void menu_OnMenuChange(object sender, EventArgs e)
        {
            XmlManager<Menu> xmlMenuManager = new XmlManager<Menu>();
            menu.UnloadContent();
            menu = xmlMenuManager.Load(menu.ID);
            menu.LoadContent();
            menu.OnMenuChange += menu_OnMenuChange;
            menu.Transition(0.0f);
            foreach (MenuItem item in menu.Items)
            {
                item.image.StoreEffects();
                item.image.ActivateEffect("FadeEffect");
            }
        }
        public void LoadContent(string menuPath)
        {
            if (menuPath != String.Empty)
            {
                menu.ID = menuPath;
            }
        }
        public void UnloadContent()
        {
            menu.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            if (!isTransitioning)
            {
                menu.Update(gameTime);
            }
            if ((InputManager.Instance.KeyPressed(Keys.Enter) || InputManager.Instance.KeyPressed(Keys.Z)) && !isTransitioning)
            {
                if (menu.Items[menu.ItemNumber].LinkType == "Screen")
                {
                    if (menu.ItemNumber == 0)
                    {
                        if (File.Exists("Load/Gameplay/SaveFile.xml"))
                        {
                            ScreenManager.Instance.ChangeScreens(menu.Items[menu.ItemNumber].LinkID);
                        }
                    }
                    else
                    {
                        ScreenManager.Instance.ChangeScreens(menu.Items[menu.ItemNumber].LinkID);
                    }
                }
                else
                {
                    isTransitioning = true;
                    menu.Transition(1.0f);
                    foreach(MenuItem item in menu.Items)
                    {
                        item.image.StoreEffects();
                        item.image.ActivateEffect("FadeEffect");
                    }
                }
            }
            Transition(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            menu.Draw(spriteBatch);
        }
    }
}