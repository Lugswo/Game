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
    [Serializable]
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
        public int xSpawn { get; set; }
        public int ySpawn { get; set; }
        public int areaXSpawn { get; set; }
        public int areaYSpawn { get; set; }
        [XmlIgnore]
        public int X { get; set; }
        [XmlIgnore]
        public int Y { get; set; }
        [XmlIgnore]
        public int AreaX { get; set; }
        [XmlIgnore]
        public int AreaY { get; set; }
        [XmlIgnore]
        public int VelocityX { get; set; }
        [XmlIgnore]
        public int VelocityY { get; set; }
        [XmlIgnore]
        public int PositionX { get; set; }
        [XmlIgnore]
        public int PositionY { get; set; }
        [XmlIgnore]
        public bool Attacking { get; set; }
        [XmlIgnore]
        public bool NextAttack { get; set;}
        [XmlIgnore]
        public bool levelUp { get; set; }
        public int HealthRegen { get; set; }
        [XmlIgnore]
        public double OneSecond;
        [XmlIgnore]
        public double combatTimer;
        [XmlIgnore]
        public bool zPressed;
        [XmlIgnore]
        public bool inCombat;
        [XmlIgnore]
        public int Combo { get; set; }
        [XmlIgnore]
        public bool aUp, aDown, aLeft, aRight, Up, Down, Left, Right, cUp, cDown, cLeft, cRight;
        [XmlIgnore]
        public int pX;
        [XmlIgnore]
        public int pY;
        public int moveSpeed;
        [XmlIgnore]
        bool ChangedFrames;
        [XmlIgnore]
        public int AttackCounter;
        [XmlIgnore]
        public List<MapSprite> Blanks;
        [XmlIgnore]
        public Rectangle DeadZone;
        [XmlIgnore]
        public Rectangle HitBox;
        [XmlIgnore]
        public Rectangle upAttackHitBox, downAttackHitBox, leftAttackHitBox, rightAttackHitBox, facingHitBox;
        [XmlIgnore]
        public bool newArea = false;
        [XmlIgnore]
        public int prevDir;
        [XmlIgnore]
        public int HitTimer;
        [XmlIgnore]
        public int maxHealth;
        public int skillPoints;
        [XmlIgnore]
        public Image skillPointsImage;
        public List<Item> inventory;
        [XmlIgnore]
        public List<Skills.Skill> skills;
        [XmlIgnore]
        public List<Skills.Skill> binds;
        [XmlIgnore]
        public NetConnection Connection { get; set; }
        [XmlIgnore]
        public Image levelUpImage;
        public Image hair;
        public Image eyes;
        public Keys skill1Bind;
        public List<int> skillIds, bindIds;
        [XmlIgnore]
        public Globals globals;
        public Player()
        {
            PlayerImage = new Image();
            hair = new Image();
            eyes = new Image();
            Blanks = new List<MapSprite>();
            DeadZone = new Rectangle();
            Attacking = NextAttack = zPressed = false;
            AttackCounter = 0;
            HitTimer = 0;
            levelUpImage = new Image();
            levelUpImage.Path = "Gameplay/Effects/Level Up";
            levelUpImage.Effects = "SpriteSheetEffect";
            levelUpImage.spriteSheetEffect.AmountOfFrames.X = 15;
            levelUpImage.spriteSheetEffect.AmountOfFrames.Y = 1;
            levelUpImage.IsActive = false;
            levelUpImage.LoadContent();
            facingHitBox = new Rectangle();
            inCombat = false;
            combatTimer = 0;
            moveSpeed = 10;
            inventory = new List<Item>();
            skills = new List<Skills.Skill>();
            skillPointsImage = new Image();
            skillPointsImage.Layer = .97f;
            binds = new List<Skills.Skill>();
            skillIds = new List<int>();
            bindIds = new List<int>();
            globals = new Globals();
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
            upAttackHitBox = new Rectangle(X, Y - 60, 60, 60);
            downAttackHitBox = new Rectangle(X, Y + 60, 60, 60);
            leftAttackHitBox = new Rectangle(X - 60, Y, 60, 60);
            rightAttackHitBox = new Rectangle(X + 60, Y, 60, 60);
        }
        public int Direction()
        {
            return (int)PlayerImage.spriteSheetEffect.CurrentFrame.Y;
        }
        public void LoadContent()
        {
            maxHealth = Health;
            PlayerImage.Path = "Gameplay/Characters/Player/Player";
            PlayerImage.Effects = "SpriteSheetEffect";
            PlayerImage.spriteSheetEffect.AmountOfFrames.X = 6;
            PlayerImage.spriteSheetEffect.AmountOfFrames.Y = 16;
            PlayerImage.LoadContent();
            ChangedFrames = false;
            NextLevel = 100 + Level * Level * Level;
            skillPointsImage.Position.Y = 125;
            skillPointsImage.Position.X = 125;
            skillPointsImage.LoadContent();
            hair.LoadContent();
            eyes.LoadContent();
            globals.LoadContent();
            foreach (int id in skillIds)
            {
                Skills.Skill temp = new Skills.Skill();
                temp = (Classes.Skills.Skill)Activator.CreateInstance(globals.dSkills[id].GetType());
                skills.Add(temp);
            }
        }
        public void UnloadContent()
        {
            PlayerImage.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            hair.Position = PlayerImage.Position;
            eyes.Position = PlayerImage.Position;
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
                if (AttackCounter > PlayerImage.spriteSheetEffect.AmountOfFrames.X * 50)
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
                facingHitBox = downAttackHitBox;
            }
            if (VelocityY < 0 && Attacking == false)
            {
                PlayerImage.spriteSheetEffect.CurrentFrame.Y = 1;
                facingHitBox = upAttackHitBox;
            }
            if (VelocityX > 0 && Attacking == false)
            {
                PlayerImage.spriteSheetEffect.CurrentFrame.Y = 2;
                facingHitBox = rightAttackHitBox;
            }
            if (VelocityX < 0 && Attacking == false)
            {
                PlayerImage.spriteSheetEffect.CurrentFrame.Y = 3;
                facingHitBox = leftAttackHitBox;
            }
            if (EXP >= NextLevel)
            {
                Health = maxHealth;
                levelUp = true;
                Level++;
                skillPoints++;
                if (levelUpImage.IsActive == true)
                {
                    Level--;
                    skillPoints--;
                }
                NextLevel = 100 + Level * Level * Level;
                EXP = 0;
            }
            PlayerImage.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerImage.Draw(spriteBatch);
            hair.Draw(spriteBatch);
            eyes.Draw(spriteBatch);
        }
    }
}