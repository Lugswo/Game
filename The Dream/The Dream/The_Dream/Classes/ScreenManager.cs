﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes
{
    public class ScreenManager
    {
        public static ScreenManager instance;
        [XmlIgnore]
        public Vector2 Dimensions { private set; get; }
        [XmlIgnore]
        public Vector2 realDimensions { private set; get; }
        [XmlIgnore]
        public ContentManager Content { private set; get; }
        XmlManager<GameScreen> xmlGameScreenManager;

        public GameScreen currentScreen, newScreen;
        [XmlIgnore]
        public GraphicsDevice GraphicsDevice;
        [XmlIgnore]
        public SpriteBatch SpriteBatch;
        public Image image;
        [XmlIgnore]
        public bool IsTransitioning { get; private set; }
        
        public static ScreenManager Instance
        {
            get
            {
                if (instance == null)
                {
                    XmlManager<ScreenManager> xml = new XmlManager<ScreenManager>();
                    instance = xml.Load("Load/ScreenManager.xml");
                }
                return instance;
            }
        }
        public void ChangeScreens(string screenName)
        {
            newScreen = (GameScreen)Activator.CreateInstance(Type.GetType("The_Dream.Classes." + screenName));
            image.IsActive = true;
            image.fadeEffect.Increase = true;
            image.Alpha = 0.0f;
            IsTransitioning = true;
        }
        void Transition(GameTime gameTime)
        {
            if (IsTransitioning)
            {
                image.Update(gameTime);
                if (image.Alpha == 1.0f)
                {
                    currentScreen.UnloadContent();
                    currentScreen = newScreen;
                    xmlGameScreenManager.type = currentScreen.type;
                    if (File.Exists(currentScreen.XmlPath))
                    {
                        currentScreen = xmlGameScreenManager.Load(currentScreen.XmlPath);
                    }
                    currentScreen.LoadContent();
                }
                else if (image.Alpha == 0.0f)
                {
                    image.IsActive = false;
                    IsTransitioning = false;
                }
            }
        }
        public ScreenManager()
        {
            Dimensions = new Vector2(
                1920, 1080);
            realDimensions = new Vector2(960, 540);
            currentScreen = new SplashScreen();
            xmlGameScreenManager = new XmlManager<GameScreen>();
            xmlGameScreenManager.type = currentScreen.type;
            currentScreen = xmlGameScreenManager.Load("Load/SplashScreen.xml");
        }
        public void LoadContent(ContentManager Content)
        {
            this.Content = new ContentManager(Content.ServiceProvider, "Content");
            currentScreen.LoadContent();
            image.LoadContent();
        }
        public void UnloadContent()
        {
            currentScreen.UnloadContent();
            image.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
            Transition(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            currentScreen.Draw(spriteBatch);
            if (IsTransitioning)
            {
                image.Draw(spriteBatch);
            }
        }
    }
}
