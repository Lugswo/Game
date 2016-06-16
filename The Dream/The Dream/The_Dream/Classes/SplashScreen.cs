using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace The_Dream.Classes
{
    public class SplashScreen : GameScreen
    {
        public Image image;
        public override void LoadContent()
        {
            base.LoadContent();
            image.LoadContent();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
            image.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            image.Update(gameTime);
            if (InputManager.Instance.KeyPressed(Keys.Enter, Keys.Z))
            {
                ScreenManager.Instance.ChangeScreens("TitleScreen");
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            image.Draw(spriteBatch);
        }
    }
}
