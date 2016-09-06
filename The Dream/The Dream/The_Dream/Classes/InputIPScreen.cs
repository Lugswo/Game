using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace The_Dream.Classes
{
    public class InputIPScreen : GameScreen
    {
        Image IPImage;
        string Ip;
        bool Host = false;
        public InputIPScreen()
        {
            Ip = String.Empty;
            IPImage = new Image();
        }
        public void SaveIP(string ip, bool host)
        {
            XmlDocument IP = new XmlDocument();
            IP.Load("Load/ClientIPandHost.xml");
            XmlNode node;
            node = IP.DocumentElement;
            foreach (XmlNode node1 in node.ChildNodes)
            {
                if (node1.Name == "hostip")
                {
                    node1.InnerText = ip;
                }
                if (node1.Name == "host")
                {
                    node1.InnerText = "0";
                }
            }
            IP.Save("Load/ClientSavedIPandHost.xml");
            XmlDocument saveHost = new XmlDocument();
            saveHost.Load("Load/ServerIPandHost.xml");
            XmlNode hnode;
            hnode = saveHost.DocumentElement;
            foreach (XmlNode node1 in hnode.ChildNodes)
            {
                if (node1.Name == "hostip")
                {
                    node1.InnerText = ip;
                }
                if (node1.Name == "host")
                {
                    node1.InnerText = "0";
                }
            }
            saveHost.Save("Load/ServerSavedIPandHost.xml");
        }
        public override void LoadContent()
        {
            base.LoadContent();
            IPImage.LoadContent();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
            IPImage.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (InputManager.Instance.KeyPressed(Keys.D0))
            {
                Ip += 0;
            }
            if (InputManager.Instance.KeyPressed(Keys.D1))
            {
                Ip += 1;
            }
            if (InputManager.Instance.KeyPressed(Keys.D2))
            {
                Ip += 2;
            }
            if (InputManager.Instance.KeyPressed(Keys.D3))
            {
                Ip += 3;
            }
            if (InputManager.Instance.KeyPressed(Keys.D4))
            {
                Ip += 4;
            }
            if (InputManager.Instance.KeyPressed(Keys.D5))
            {
                Ip += 5;
            }
            if (InputManager.Instance.KeyPressed(Keys.D6))
            {
                Ip += 6;
            }
            if (InputManager.Instance.KeyPressed(Keys.D7))
            {
                Ip += 7;
            }
            if (InputManager.Instance.KeyPressed(Keys.D8))
            {
                Ip += 8;
            }
            if (InputManager.Instance.KeyPressed(Keys.D9))
            {
                Ip += 9;
            }
            if (InputManager.Instance.KeyPressed(Keys.OemPeriod))
            {
                Ip += ".";
            }
            if (InputManager.Instance.KeyPressed(Keys.Back))
            {
                if (Ip.Length > 0)
                {
                    Ip = Ip.Remove(Ip.Length - 1);
                }
            }
            if (InputManager.Instance.KeyPressed(Keys.Enter))
            {
                if (Ip.Length > 0)
                {
                    SaveIP(Ip, Host);
                    ScreenManager.Instance.ChangeScreens("GameplayScreen");
                }
            }
            IPImage = new Image();
            IPImage.Text = Ip;
            IPImage.LoadContent();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            IPImage.Draw(spriteBatch);
        }
    }
}
