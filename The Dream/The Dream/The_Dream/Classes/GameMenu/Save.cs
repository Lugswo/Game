using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace The_Dream.Classes.GameMenu
{
    [XmlInclude(typeof(Classes.Skills.Skill))]
    public class Save : MenuTab
    {
        Globals globals;
        public Save()
        {
            image.Path = "Gameplay/GUI/Menu/Save";
            inImage.Path = "Gameplay/GUI/Menu/InSave";
            globals = new Globals();
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
            if (inMenu == true)
            {
                if (InputManager.Instance.KeyPressed(Keys.Z))
                {
                    globals.Save(player);
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch, bool inMenu)
        {
            base.Draw(spriteBatch, inMenu);
        }
    }
}
