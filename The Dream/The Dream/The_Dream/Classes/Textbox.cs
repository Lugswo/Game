using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes
{
    public class Textbox
    {
        public Image box;
        public Image text;
        public bool yCenter;
        public int y;
        public Textbox(string t)
        {
            box = new Image();
            text = new Image();
            text.Text = t;
            yCenter = true;
        }
        public void LoadContent()
        {
            text.LoadContent();
            text.Position.X = 960 - (text.Font.MeasureString(text.Text).X / 2);
            if (yCenter == false)
            {
                text.Position.Y = y;
            }
            else
            {
                text.Position.Y = 540 - (text.Font.MeasureString(text.Text).Y / 2);
            }
            text.Layer = .99f;
            box.Layer = .98f;
            box.Path = "GUI/Textbox";
            box.rect = new Rectangle((int)text.Position.X - 50, (int)text.Position.Y - 50,
                (int)text.Font.MeasureString(text.Text).X + 100, (int)text.Font.MeasureString(text.Text).Y + 100);
            box.LoadContent();
        }
        public void UnloadContent()
        {
            text.UnloadContent();
            box.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            text.Update(gameTime);
            box.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            text.DrawString(spriteBatch);
            box.DrawToRectangle(spriteBatch);
        }
    }
}
