using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace The_Dream.Classes
{
    public class SoundItem
    {
        [XmlIgnore]
        public SoundEffect soundEffect;
        public string Path, Name;
        public SoundEffectInstance instance;
        ContentManager content;
        public SoundItem()
        {
            Path = String.Empty;
        }
        public void LoadContent()
        {
            content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
            soundEffect = content.Load<SoundEffect>(Path);
            instance = soundEffect.CreateInstance();
        }
        public void UnloadContent()
        {
            content.Unload();
        }
        public void Update(GameTime gameTime)
        {

        }
    }
}
