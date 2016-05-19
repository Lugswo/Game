using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;

namespace The_Dream.Classes
{
    public class Player
    {
        public Image PlayerImage;
        public int Level { get; set; }
        public int Strength { get; set; }
        public int Defense { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int Speed { get; set; }
        public int Health { get; set; }
        public int Energy { get; set; }
        public int EXP { get; set; }
        public int NextLevel { get; set; }
        public int StatPoints { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int AreaX { get; set; }
        public int AreaY { get; set; }
        public int VelocityX;
        public int VelocityY;
        [XmlIgnore]
        public NetConnection Connection { get; set; }
        public Player()
        {
            PlayerImage = new Image();
        }
        public void LoadContent()
        {
            PlayerImage.Path = "Gameplay/Characters/Sprites/Player/Player";
            PlayerImage.Effects = "SpriteSheetEffect";
            PlayerImage.spriteSheetEffect.AmountOfFrames = new Vector2(6, 16);
            PlayerImage.LoadContent();
        }
        public void UnloadContent()
        {
            PlayerImage.UnloadContent();
        }
    }
}