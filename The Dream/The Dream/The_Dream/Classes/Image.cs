using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace The_Dream.Classes
{
    public class Image
    {
        public float Alpha;
        public string Text, FontName;
        public string Path;
        public Vector2 Position, Scale;
        public Rectangle SourceRect;
        public bool IsActive;
        [XmlIgnore]
        public Texture2D texture;
        Vector2 origin;
        public Color color;
        ContentManager content;
        [XmlIgnore]
        RenderTarget2D renderTarget;
        [XmlIgnore]
        public SpriteFont Font;
        [XmlIgnore]
        public Dictionary<string, ImageEffect> effectList;
        public string Effects;
        public FadeEffect fadeEffect;
        public ExpandEffect expandEffect;
        public SpriteSheetEffect spriteSheetEffect = new SpriteSheetEffect();
        public ShowSpriteEffect showSpriteEffect;
        public Vector2 dimensions;
        void SetEffect<T>(ref T effect)
        {
            if (effect == null)
            {
                effect = (T)Activator.CreateInstance(typeof(T));
            }
            else
            {
                var obj = this;
                (effect as ImageEffect).LoadContent(ref obj);
            }
            effectList.Add(effect.GetType().ToString().Replace("The_Dream.Classes.", ""), (effect as ImageEffect));
        }
        public void ActivateEffect(string effect)
        {
            if (effectList.ContainsKey(effect))
            {
                effectList[effect].IsActive = true;
                var obj = this;
                effectList[effect].LoadContent(ref obj);
            }
        }
        public void DeactivateEffect(string effect)
        {
            if (effectList.ContainsKey(effect))
            {
                effectList[effect].IsActive = false;
                effectList[effect].UnloadContent();
            }
        }
        public void StoreEffects()
        {
            Effects = String.Empty;
            foreach (var effect in effectList)
            {
                if (effect.Value.IsActive)
                {
                    Effects += effect.Key + ":";
                }
            }
            if (Effects != String.Empty)
            {
                Effects.Remove(Effects.Length - 1);
            }
        }
        public void RestoreEffects()
        {
            foreach (var effect in effectList)
            {
                DeactivateEffect(effect.Key);
            }
            string[] split = Effects.Split(':');
            foreach (string s in split)
            {
                ActivateEffect(s);
            }
        }
        public Image()
        {
            Path = Text = Effects = String.Empty;
            FontName = "Fonts/Secret of Mana";
            Position = Vector2.Zero;
            Scale = Vector2.One;
            Alpha = 1.0f;
            SourceRect = Rectangle.Empty;
            effectList = new Dictionary<string, ImageEffect>();
        }
        public void LoadContent()
        {
            content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
            if (Path != String.Empty)
            {
                texture = this.content.Load<Texture2D>(Path);
            }
            Font = content.Load<SpriteFont>(FontName);
            dimensions = Vector2.One;
            if (texture != null)
            {
                dimensions.X += texture.Width;
            }
            dimensions.X += Font.MeasureString(Text).X;
            if (texture != null)
            {
                dimensions.Y = Math.Max(texture.Height, Font.MeasureString(Text).Y);
            }
            else
            {
                dimensions.Y += Font.MeasureString(Text).Y;
            }
            if (SourceRect == Rectangle.Empty)
            {
                SourceRect = new Rectangle(0, 0, (int)dimensions.X, (int)dimensions.Y);
            }
            renderTarget = new RenderTarget2D(
                ScreenManager.Instance.GraphicsDevice,
                (int)dimensions.X,
                (int)dimensions.Y);
            ScreenManager.Instance.GraphicsDevice.SetRenderTarget(renderTarget);
            ScreenManager.Instance.GraphicsDevice.Clear(Color.Transparent);
            ScreenManager.Instance.SpriteBatch.Begin();
            if (texture != null)
            {
                ScreenManager.Instance.SpriteBatch.Draw(texture, Vector2.Zero, Color.White);
            }
            if (color.A == 0 && color.R == 0 && color.G == 0 && color.B == 0)
            {
                ScreenManager.Instance.SpriteBatch.DrawString(Font, Text, Vector2.Zero, Color.White);
            }
            else
            {
                ScreenManager.Instance.SpriteBatch.DrawString(Font, Text, Vector2.Zero, color);
            }
            ScreenManager.Instance.SpriteBatch.End();
            texture = renderTarget;
            ScreenManager.Instance.GraphicsDevice.SetRenderTarget(null);
            SetEffect<FadeEffect>(ref fadeEffect);
            SetEffect<ExpandEffect>(ref expandEffect);
            SetEffect<SpriteSheetEffect>(ref spriteSheetEffect);
            SetEffect<ShowSpriteEffect>(ref showSpriteEffect);
            if (Effects != String.Empty)
            {
                string[] split = Effects.Split(':');
                foreach(string item in split)
                {
                    ActivateEffect(item);
                }
            }
        }
        public void UnloadContent()
        {
            content.Unload();
            foreach (var effect in effectList)
            {
                DeactivateEffect(effect.Key);
            }
        }
        public void Update(GameTime gameTime)
        {
            foreach(var effect in effectList)
            {
                if (effect.Value.IsActive)
                {
                    effect.Value.Update(gameTime);
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);
            spriteBatch.Draw(texture, Position + origin, SourceRect, Color.White * Alpha,
                0.0f, origin, Scale, SpriteEffects.None, 0.0f);
        }
    }
}
