using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using System.Text;

namespace The_Dream.Classes
{
    public class SoundManager
    {
        [XmlElement("Sound")]
        public List<SoundItem> soundEffectList;
        [XmlElement("Music")]
        public List<SoundItem> musicList;
        [XmlIgnore]
        public Dictionary<string, SoundItem> soundEffects;
        [XmlIgnore]
        public Dictionary<string, SoundItem> Music;
        void SetSound(SoundItem sound)
        {
            if (sound == null)
            {
                sound = (SoundItem)Activator.CreateInstance(typeof(SoundItem));
            }
            soundEffects.Add(sound.Name, (sound as SoundItem));
        }
        void SetMusic(SoundItem music)
        {
            if (music == null)
            {
                music = (SoundItem)Activator.CreateInstance(typeof(SoundItem));
            }
            Music.Add(music.Name, (music as SoundItem));
        }
        public SoundManager()
        {
            soundEffects = new Dictionary<string, SoundItem>();
            Music = new Dictionary<string, SoundItem>();
        }
        public void LoadContent()
        {
            foreach (SoundItem sound in soundEffectList)
            {
                SetSound(sound);
            }
            foreach (SoundItem music in musicList)
            {
                SetMusic(music);
            }
            foreach (var sound in soundEffects)
            {
                sound.Value.LoadContent();
            }
            foreach (var music in Music)
            {
                music.Value.LoadContent();
                music.Value.instance.IsLooped = true;
            }
        }
        public void UnloadContent()
        {
            foreach (var sound in soundEffects)
            {
                sound.Value.UnloadContent();
            }
        }
    }
}
