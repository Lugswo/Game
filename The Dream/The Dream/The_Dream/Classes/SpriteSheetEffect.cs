using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace The_Dream.Classes
{
    public class SpriteSheetEffect : ImageEffect
    {
        public int FrameCounter;
        public int SwitchFrame;
        public Vector2 CurrentFrame;
        public Vector2 AmountOfFrames;
        public int FrameWidth
        {
            get
            {
                if (image.texture != null)
                {
                    return image.texture.Width / (int)AmountOfFrames.X;
                }
                else return 0;
            }
        }
        public int FrameHeight
        {
            get
            {
                if (image.texture != null)
                {
                    return image.texture.Height / (int)AmountOfFrames.Y;
                }
                else
                {
                    return 0;
                }
            }
        }
        public SpriteSheetEffect()
        {
            CurrentFrame = new Vector2(1, 0);
            SwitchFrame = 100;
            FrameCounter = 0;
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
            if (image.IsActive)
            {
                FrameCounter += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (FrameCounter >= SwitchFrame)
                {
                    FrameCounter = 0;
                    CurrentFrame.X++;
                    if (CurrentFrame.X * FrameWidth >= image.texture.Width - 1)
                    {
                        CurrentFrame.X = 0;
                    }
                }
            }
            else
            {
                CurrentFrame.X = 1;
            }
            image.SourceRect = new Rectangle((int)CurrentFrame.X * FrameWidth,
                (int)CurrentFrame.Y * FrameHeight, FrameWidth, FrameHeight);
        }
    }
}
