using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        public int VelocityX { get; set; }
        public int VelocityY { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public bool Attacking { get; set; }
        public bool NextAttack { get; set;}
        public bool zPressed;
        public int Combo { get; set; }
        public bool Up, Down, Left, Right;
        public int pX;
        public int pY;
        public List<MapSprite> Blanks;
        public Rectangle DeadZone;
        public Rectangle HitBox;
        public Rectangle upAttackHitBox, downAttackHitBox, leftAttackHitBox, rightAttackHitBox;
        public bool newArea = false;
        int prevDir;
        [XmlIgnore]
        public NetConnection Connection { get; set; }
        public Player()
        {
            PlayerImage = new Image();
            Blanks = new List<MapSprite>();
            DeadZone = new Rectangle();
            Attacking = NextAttack = zPressed = false;
        }
        public void UpdateHitBoxes()
        {
            upAttackHitBox = new Rectangle(X, Y - 64, 54, 64);
            downAttackHitBox = new Rectangle(X, Y + 64, 54, 64);
            leftAttackHitBox = new Rectangle(X - 54, Y, 54, 64);
            rightAttackHitBox = new Rectangle(X + 54, Y + 64, 54, 64);
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
        public void Update(GameTime gameTime)
        {
            PlayerImage.IsActive = true;
            if (VelocityX == 0 && VelocityY == 0 && Attacking == false)
            {
                PlayerImage.IsActive = false;
            }
            if (Attacking == true)
            {
                if (zPressed == true)
                {
                    prevDir = (int)PlayerImage.spriteSheetEffect.CurrentFrame.Y;
                    PlayerImage.spriteSheetEffect.CurrentFrame.Y += 8;
                    PlayerImage.spriteSheetEffect.CurrentFrame.X = 0;
                    zPressed = false;
                }
                if (PlayerImage.spriteSheetEffect.CurrentFrame.X == PlayerImage.spriteSheetEffect.AmountOfFrames.X - 1)
                {
                    if (NextAttack == true)
                    {
                        PlayerImage.spriteSheetEffect.CurrentFrame.Y = prevDir + (Combo * 4) + 8;
                        PlayerImage.spriteSheetEffect.CurrentFrame.X = 0;
                        NextAttack = false;
                    }
                    else
                    {
                        Attacking = false;
                        PlayerImage.spriteSheetEffect.CurrentFrame.Y = prevDir;
                    }
                }
            }
            if (VelocityY > 0 && Attacking == false)
            {
                PlayerImage.spriteSheetEffect.CurrentFrame.Y = 0;
            }
            if (VelocityY < 0 && Attacking == false)
            {
                PlayerImage.spriteSheetEffect.CurrentFrame.Y = 1;
            }
            if (VelocityX > 0 && Attacking == false)
            {
                PlayerImage.spriteSheetEffect.CurrentFrame.Y = 2;
            }
            if (VelocityX < 0 && Attacking == false)
            {
                PlayerImage.spriteSheetEffect.CurrentFrame.Y = 3;
            }
            PlayerImage.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerImage.Draw(spriteBatch);
        }
    }
}