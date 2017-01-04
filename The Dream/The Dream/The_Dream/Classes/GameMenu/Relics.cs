using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes.GameMenu
{
    public class Relics : MenuTab
    {
        public Relics()
        {
            image.Path = "Gameplay/GUI/Menu/Relics";
            inImage.Path = "Gameplay/GUI/Menu/InRelics";
        }
        public override void LoadContent()
        {
            base.LoadContent();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime, Player player, bool inMenu)
        {
            base.Update(gameTime, player, inMenu);
        }
        public override void Draw(SpriteBatch spriteBatch, bool inMenu)
        {
            base.Draw(spriteBatch, inMenu);
        }
    }
}
