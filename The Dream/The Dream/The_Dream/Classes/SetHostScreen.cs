using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes
{
    public class SetHostScreen : GameScreen
    {
        public void SetHost()
        {
            XmlDocument IP = new XmlDocument();
            IP.Load("Load/ServerIPandHost.xml");
            XmlNode node;
            node = IP.DocumentElement;
            foreach (XmlNode node1 in node.ChildNodes)
            {
                if (node1.Name == "host")
                {
                    node1.InnerText = "1";
                }
                if (node1.Name == "hostip")
                {
                    node1.InnerText = "localhost";
                }
            }
            IP.Save("Load/ServerSavedIPandHost.xml");
            XmlDocument Client = new XmlDocument();
            Client.Load("Load/ClientIPandHost.xml");
            XmlNode node2;
            node2 = Client.DocumentElement;
            foreach (XmlNode node3 in node2.ChildNodes)
            {
                if (node3.Name == "host")
                {
                    node3.InnerText = "1";
                }
                if (node3.Name == "hostip")
                {
                    node3.InnerText = "localhost";
                }
            }
            Client.Save("Load/ClientSavedIPandHost.xml");
        }
        public override void LoadContent()
        {
            base.LoadContent();
            SetHost();
            ScreenManager.Instance.ChangeScreens("GameplayScreen");
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
