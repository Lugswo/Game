using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace The_Dream.Classes
{
    public class ExpandEffect : ImageEffect
    {
        public float ExpandSpeed;
        public bool Increase;
        public ExpandEffect()
        {
            ExpandSpeed = 1.5f;
            Increase = true;
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
            if (image.IsActive == true)
            {
                if (!Increase)
                {
                    image.Scale.X -= ExpandSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    image.Scale.Y -= ExpandSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    image.Scale.X += ExpandSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    image.Scale.Y += ExpandSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (image.Scale.X < 1.0f)
                {
                    Increase = true;
                    image.Scale.X = 1.0f;
                    image.Scale.Y = 1.0f;
                }
                else if (image.Scale.X > 1.5f)
                {
                    image.Scale.X = 1.5f;
                    image.Scale.Y = 1.5f;
                }
            }
            else
            {
                image.Scale.X = 1.0f;
                image.Scale.Y = 1.0f;
            }
        }
    }
}
