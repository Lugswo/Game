using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes
{
    public class GameScreen
    {
        protected ContentManager Content;
        [XmlIgnore]
        public Type type;
        public string XmlPath;
        public GameScreen()
        {
            type = this.GetType();
            XmlPath = "Load/" + type.ToString().Replace("The_Dream.Classes.", "") + ".xml";
        }
        public virtual void LoadContent()
        {
            Content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
        }
        public virtual void UnloadContent()
        {
            Content.Unload();
        }
        public virtual void Update(GameTime gameTime)
        {
            InputManager.Instance.Update();
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
