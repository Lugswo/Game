using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace The_Dream.Classes
{
    public class TextScrollEffect : ImageEffect
    {
        public int scrollSpeed;
        public char[] text;
        public string originalText;
        public int textNumber;
        public string currentText;
        int scrollTimer;
        bool toChar;
        public TextScrollEffect()
        {
            currentText = originalText = string.Empty;
            scrollSpeed = 10;
            textNumber = 0;
            scrollTimer = 0;
            toChar = true;
        }
        public override void LoadContent(ref Image image)
        {
            base.LoadContent(ref image);
            text = image.Text.ToCharArray();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (image.IsActive == true)
            {
                if (toChar == true)
                {
                    originalText = image.Text;
                    text = image.Text.ToCharArray();
                    toChar = false;
                }
                scrollTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (textNumber < text.Length)
                {
                    if (scrollTimer >= scrollSpeed)
                    {
                        scrollTimer = 0;
                        currentText = currentText + text[textNumber];
                        image.Text = currentText;
                        textNumber++;
                    }
                }
                else
                {
                    image.IsActive = false;
                }
            }
            else
            {
                textNumber = 0;
                currentText = string.Empty;
                scrollTimer = 0;
                toChar = true;
                image.Text = originalText;
            }
        }
    }
}