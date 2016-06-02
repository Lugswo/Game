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
            IP.Load("Load/IPandHost.xml");
            XmlNode node;
            node = IP.DocumentElement;
            foreach (XmlNode node1 in node.ChildNodes)
            {
                if (node1.Name == "host")
                {
                    node1.InnerText = "1";
                }
            }
            IP.Save("Load/SavedIPandHost.xml");
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
