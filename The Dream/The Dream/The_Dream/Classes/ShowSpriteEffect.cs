using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes
{
    public class ShowSpriteEffect : ImageEffect
    {
        public Vector2 Sprite;
        public Vector2 AmountOfSprites;
        public int SpriteWidth
        {
            get
            {
                if (image.texture != null)
                {
                    return image.texture.Width / (int)AmountOfSprites.X;
                }
                else return 0;
            }
        }
        public int SpriteHeight
        {
            get
            {
                if (image.texture != null)
                {
                    return image.texture.Height / (int)AmountOfSprites.Y;
                }
                else
                {
                    return 0;
                }
            }
        }
        public ShowSpriteEffect()
        {

        }
        public override void LoadContent(ref Image image)
        {
            base.LoadContent(ref image);
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            image.SourceRect = new Rectangle((int)Sprite.X * SpriteWidth, (int)Sprite.Y * SpriteHeight, SpriteWidth, SpriteHeight);
        }
    }
}
