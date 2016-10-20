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
        public bool levelUp { get; set; }
        public bool zPressed;
        public int Combo { get; set; }
        public bool aUp, aDown, aLeft, aRight, Up, Down, Left, Right;
        public int pX;
        public int pY;
        bool ChangedFrames;
        int AttackCounter;
        public List<MapSprite> Blanks;
        public Rectangle DeadZone;
        public Rectangle HitBox;
        public Rectangle upAttackHitBox, downAttackHitBox, leftAttackHitBox, rightAttackHitBox;
        public bool newArea = false;
        public int prevDir;
        public int HitTimer;
        [XmlIgnore]
        public NetConnection Connection { get; set; }
        public Player()
        {
            PlayerImage = new Image();
            Blanks = new List<MapSprite>();
            DeadZone = new Rectangle();
            Attacking = NextAttack = zPressed = false;
            AttackCounter = 0;
            HitTimer = 0;
        }
        public void UpdateHitTimer(GameTime gameTime)
        {
            if (Attacking == true)
            {
                HitTimer += gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                HitTimer = 0;
            }
        }
        public void UpdateHitBoxes()
        {
            upAttackHitBox = new Rectangle(X, Y - 64, 54, 64);
            downAttackHitBox = new Rectangle(X, Y + 64, 54, 64);
            leftAttackHitBox = new Rectangle(X - 54, Y, 54, 64);
            rightAttackHitBox = new Rectangle(X + 54, Y, 54, 64);
        }
        public void LoadContent()
        {
            PlayerImage.Path = "Gameplay/Characters/Sprites/Player/Player";
            PlayerImage.Effects = "SpriteSheetEffect";
            PlayerImage.spriteSheetEffect.AmountOfFrames = new Vector2(6, 16);
            PlayerImage.LoadContent();
            ChangedFrames = false;
            NextLevel = 100 + Level * Level * Level;
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
                if (ChangedFrames == false)
                {
                    prevDir = (int)PlayerImage.spriteSheetEffect.CurrentFrame.Y;
                    PlayerImage.spriteSheetEffect.CurrentFrame.Y += 8;
                    PlayerImage.spriteSheetEffect.CurrentFrame.X = 0;
                    ChangedFrames = true;
                }
                if (AttackCounter == 0)
                {
                    if (prevDir == 0)
                    {
                        aDown = true;
                    }
                    else if (prevDir == 1)
                    {
                        aUp = true;
                    }
                    else if (prevDir == 2)
                    {
                        aRight = true;
                    }
                    else if (prevDir == 3)
                    {
                        aLeft = true;
                    }
                }
                if (AttackCounter > 0)
                {
                    aUp = aDown = aLeft = aRight = false;
                }
                AttackCounter += gameTime.ElapsedGameTime.Milliseconds;
                if (AttackCounter > PlayerImage.spriteSheetEffect.AmountOfFrames.X * 100)
                {
                    if (NextAttack == true)
                    {
                        PlayerImage.spriteSheetEffect.CurrentFrame.Y = prevDir + 8 + Combo * 4;
                        PlayerImage.spriteSheetEffect.CurrentFrame.X = 0;
                        AttackCounter = 0;
                    }
                    else
                    {
                        Attacking = false;
                        PlayerImage.spriteSheetEffect.CurrentFrame.Y = prevDir;
                        PlayerImage.spriteSheetEffect.CurrentFrame.X = 0;
                    }
                    NextAttack = false;
                }
            }
            else
            {
                AttackCounter = 0;
                Combo = 0;
                ChangedFrames = false;
                aUp = aDown = aLeft = aRight = false;
                if (PlayerImage.spriteSheetEffect.CurrentFrame.Y >= 8)
                {
                    PlayerImage.spriteSheetEffect.CurrentFrame.Y = prevDir;
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
            if (EXP >= NextLevel)
            {
                levelUp = true;
                Level++;
                NextLevel = NextLevel = 100 + Level * Level * Level;
            }
            PlayerImage.Update(gameTime);
        }
    }
}