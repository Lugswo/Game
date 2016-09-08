using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace The_Dream.Classes
{
    public class PlayerUpdate
    {
        public Image image;
        public Image LevelUp;
        public Image HavePoints;
        public SoundManager soundManager;
        [XmlIgnore]
        public Rectangle HitBox;
        public Map map = new Map();
        public Vector2 Velocity, PlayerMoved, PrevPlayerMoved, OriginalPosition;
        public float MoveSpeed;
        public int playerHeight, playerWidth, Combo, Health, Energy, EXP, Level, NextLevel, Strength, Defense, Dexterity, Intelligence, Speed, Direction, StatPoints;
        public bool Attacking, SetZero, NextAttack, Rolling, IncreaseVelocity, AtHorizontalEdge, AtVerticalEdge, Leveled;
        public List<Rectangle> AttackHitboxes;
        public string Name;
        int Attacks, prevY;
        public int Height, Width;
        public PlayerUpdate()
        {
            Width = 54;
            Height = 64;
        }
        public void GetReferences(Map GameMap)
        {
            map = GameMap;
        }
        public void LoadContent()
        {
            Velocity = Vector2.Zero;
            Attacking = false;
            SetZero = true;
            Rolling = false;
            LevelUp.Path = "Gameplay/Effects/Level Up";
            HavePoints.Path = "Gameplay/Effects/Level Up";
            AtHorizontalEdge = AtVerticalEdge = Leveled = false;
            IncreaseVelocity = true;
            image.LoadContent();
            HavePoints.LoadContent();
            LevelUp.LoadContent();
            HavePoints.Position.X = map.ScreenWidth - HavePoints.showSpriteEffect.SpriteWidth - 50;
            HavePoints.Position.Y = 0;
            image.Position.X = map.ScreenWidth / 2;
            image.Position.Y = map.ScreenHeight / 2;
            OriginalPosition = new Vector2(image.Position.X, image.Position.Y);
            HitBox = new Rectangle();
            AttackHitboxes.Add(new Rectangle((int)image.Position.X, (int)image.Position.Y + image.spriteSheetEffect.FrameHeight / 2, image.spriteSheetEffect.FrameWidth, image.spriteSheetEffect.FrameHeight));
            AttackHitboxes.Add(new Rectangle((int)image.Position.X, (int)image.Position.Y - image.spriteSheetEffect.FrameHeight / 2, image.spriteSheetEffect.FrameWidth, image.spriteSheetEffect.FrameHeight));
            AttackHitboxes.Add(new Rectangle((int)image.Position.X + image.spriteSheetEffect.FrameWidth / 2, (int)image.Position.Y, image.spriteSheetEffect.FrameWidth, image.spriteSheetEffect.FrameHeight));
            AttackHitboxes.Add(new Rectangle((int)image.Position.X - image.spriteSheetEffect.FrameWidth / 2, (int)image.Position.Y, image.spriteSheetEffect.FrameWidth, image.spriteSheetEffect.FrameHeight));
        }
        public void UnloadContent()
        {
            image.UnloadContent();
            LevelUp.UnloadContent();
            HavePoints.UnloadContent();
        }
        public void Move(GameTime gameTime, ref Player player, bool Up, bool Down, bool Left, bool Right)
        {
            //map.Update(gameTime, player);
            if (Down == true)
            {
                player.VelocityY = 10;
            }
            else if (Up == true)
            {
                player.VelocityY = -10;
            }
            if (Right == true)
            {
                player.VelocityX = 10;
            }
            else if (Left == true)
            {
                player.VelocityX = -10;
            }
            if (Up == false && Down == false)
            {
                player.VelocityY = 0;
            }
            if (Left == false && Right == false)
            {
                player.VelocityX = 0;
            }
            
            foreach (MapSprite blank in map.Blanks)
            {
                if (blank.HitBox)
            }

            player.X += player.VelocityX;
            player.Y += player.VelocityY;
            //if (map.Left == true || map.Right == true)
            //{
            //    player.PositionX += player.VelocityX;
            //}
            //if (map.Down == true || map.Up == true)
            //{
            //    player.PositionY += player.VelocityY;
            //}
            //if (map.Left == false && map.Right == false)
            //{
            //    map.HorizontalMove(player.VelocityX);
            //}
            //if (map.Up == false && map.Down == false)
            //{
            //    map.VerticalMove(player.VelocityY);
            //}

            if (map.Area[player.AreaX, player.AreaY + 1] != null)
            {
                if (player.Y > map.DeadZone.Bottom - Height)
                {
                    player.AreaY++;
                    player.Y = map.DeadZone.Top;
                    player.PositionY = map.DeadZone.Top;
                    player.newArea = true;
                }
            }
            else
            {
                if (player.Y > map.DeadZone.Bottom - Height)
                {
                    player.Y = map.DeadZone.Bottom - Height;
                    player.PositionY = map.DeadZone.Bottom - Height;
                    player.VelocityY = 0;
                }
            }
            if (map.Area[player.AreaX, player.AreaY - 1] != null)
            {
                if (player.Y < map.DeadZone.Top)
                {
                    player.AreaY--;
                    player.Y = map.DeadZone.Bottom - Height;
                    player.PositionY = map.DeadZone.Bottom - Height;
                    player.newArea = true;
                }
            }
            else
            {
                if (player.Y < map.DeadZone.Top)
                {
                    player.Y = map.DeadZone.Top;
                    player.PositionY = map.DeadZone.Top;
                    player.VelocityY = 0;
                }
            }
            if (map.Area[player.AreaX + 1, player.AreaY] != null)
            {
                if (player.X > map.DeadZone.Right - Width)
                {
                    player.AreaX++;
                    player.X = map.DeadZone.Left;
                    player.PositionX = map.DeadZone.Left;
                    player.newArea = true;
                }
            }
            else
            {
                if (player.X > map.DeadZone.Right - Width)
                {
                    player.X = map.DeadZone.Right - Width;
                    player.PositionX = map.DeadZone.Right - Width;
                    player.VelocityX = 0;
                }
            }
            if (map.Area[player.AreaX - 1, player.AreaY] != null)
            {
                if (player.X < map.DeadZone.Left)
                {
                    player.AreaX--;
                    player.X = map.DeadZone.Right - Width;
                    player.PositionX = map.DeadZone.Right - Width;
                    player.newArea = true;
                }
            }
            else
            {
                if (player.X < map.DeadZone.Left)
                {
                    player.X = 0;
                    player.PositionX = map.DeadZone.Right - Width;
                    player.VelocityX = 0;
                }
            }
        //    if (map.Pause == false)
        //    {
        //        LevelUp.Position.X = image.Position.X - ((LevelUp.spriteSheetEffect.FrameWidth - image.spriteSheetEffect.FrameWidth) / 2);
        //        LevelUp.Position.Y = image.Position.Y - image.spriteSheetEffect.FrameHeight;
        //        playerHeight = image.texture.Height / (int)image.spriteSheetEffect.AmountOfFrames.Y;
        //        playerWidth = image.texture.Width / (int)image.spriteSheetEffect.AmountOfFrames.X;
        //        HitBox.X = (int)image.Position.X;
        //        HitBox.Y = (int)image.Position.Y;
        //        HitBox.Width = playerWidth;
        //        HitBox.Height = playerHeight;
        //        AtHorizontalEdge = AtVerticalEdge = false;
        //        image.IsActive = true;
        //        AttackHitboxes[0] = new Rectangle((int)image.Position.X, (int)image.Position.Y + image.spriteSheetEffect.FrameHeight / 2, image.spriteSheetEffect.FrameWidth, image.spriteSheetEffect.FrameHeight);
        //        AttackHitboxes[1] = new Rectangle((int)image.Position.X, (int)image.Position.Y - image.spriteSheetEffect.FrameHeight / 2, image.spriteSheetEffect.FrameWidth, image.spriteSheetEffect.FrameHeight);
        //        AttackHitboxes[2] = new Rectangle((int)image.Position.X + image.spriteSheetEffect.FrameWidth / 2, (int)image.Position.Y, image.spriteSheetEffect.FrameWidth, image.spriteSheetEffect.FrameHeight);
        //        AttackHitboxes[3] = new Rectangle((int)image.Position.X - image.spriteSheetEffect.FrameWidth / 2, (int)image.Position.Y, image.spriteSheetEffect.FrameWidth, image.spriteSheetEffect.FrameHeight);
        //        if (EXP >= NextLevel)
        //        {
        //            EXP = 0;
        //            StatPoints += 5;
        //            LevelUp.IsActive = true;
        //            NextLevel = 100 + Level * Level * Level;
        //            Level++;
        //            Leveled = true;
        //        }
        //        if (HitBox.X < map.DeadZone.X)
        //        {
        //            AtHorizontalEdge = true;
        //            if (map.Area[map.AreaX - 1, map.AreaY] != null)
        //            {
        //                map.NewMap = true;
        //                map.ExitLeft = true;
        //            }
        //        }
        //        if (HitBox.X > (map.DeadZone.Right - playerWidth))
        //        {
        //            AtHorizontalEdge = true;
        //            if (map.Area[map.AreaX + 1, map.AreaY] != null)
        //            {
        //                map.NewMap = true;
        //                map.ExitRight = true;
        //            }
        //        }
        //        if (HitBox.Y < map.DeadZone.Y)
        //        {
        //            AtVerticalEdge = true;
        //            if (map.Area[map.AreaX, map.AreaY - 1] != null)
        //            {
        //                map.NewMap = true;
        //                map.ExitTop = true;
        //            }
        //        }
        //        if (HitBox.Y > (map.DeadZone.Bottom - playerHeight))
        //        {
        //            AtVerticalEdge = true;
        //            if (map.Area[map.AreaX, map.AreaY + 1] != null)
        //            {
        //                map.NewMap = true;
        //                map.ExitBottom = true;
        //            }
        //        }
        //        if (Attacking == true && Rolling == false)
        //        {
        //            Velocity = Vector2.Zero;
        //            map.Moved = map.prevMoved;
        //            if (SetZero == true)
        //            {
        //                Attacks += 8;
        //                prevY = (int)image.spriteSheetEffect.CurrentFrame.Y;
        //                image.spriteSheetEffect.CurrentFrame.Y += Attacks;
        //                image.spriteSheetEffect.CurrentFrame.X = 0;
        //                SetZero = false;
        //                Combo = 0;
        //                Direction = prevY;
        //            }
        //            if (NextAttack == true)
        //            {
        //                if (image.spriteSheetEffect.CurrentFrame.X == image.spriteSheetEffect.AmountOfFrames.X - 1)
        //                {
        //                    soundManager.soundEffects["Attack"].soundEffect.Play();
        //                    NextAttack = false;
        //                    Combo++;
        //                    image.spriteSheetEffect.CurrentFrame.Y += 4;
        //                    Attacks += 4;
        //                    image.spriteSheetEffect.CurrentFrame.X = 0;
        //                    if (Attacks >= image.spriteSheetEffect.AmountOfFrames.Y)
        //                    {
        //                        Attacks = 8;
        //                        Combo = 0;
        //                        image.spriteSheetEffect.CurrentFrame.Y = prevY + Attacks;
        //                    }
        //                }
        //            }
        //            if (image.spriteSheetEffect.CurrentFrame.X == image.spriteSheetEffect.AmountOfFrames.X - 1)
        //            {
        //                Attacking = false;
        //                Attacks = 0;
        //                image.spriteSheetEffect.CurrentFrame.Y = prevY;
        //                SetZero = true;
        //                Direction = 0;
        //            }
        //        }
        //        if (Attacking == false && Rolling == true)
        //        {
        //            if (IncreaseVelocity == true)
        //            {
        //                IncreaseVelocity = false;
        //                bool right, left, up, down;
        //                right = left = up = down = false;
        //                image.spriteSheetEffect.CurrentFrame.Y += 4;
        //                image.spriteSheetEffect.CurrentFrame.X = 0;
        //                if (Velocity.X < 0)
        //                {
        //                    left = true;
        //                }
        //                if (Velocity.X > 0)
        //                {
        //                    right = true;
        //                }
        //                if (Velocity.Y < 0)
        //                {
        //                    down = true;
        //                }
        //                if (Velocity.Y > 0)
        //                {
        //                    up = true;
        //                }
        //                if (Velocity.X == 0 && Velocity.Y == 0)
        //                {
        //                    image.IsActive = false;
        //                }
        //                if (left == true)
        //                {
        //                    Velocity.X = -10;
        //                }
        //                if (right == true)
        //                {
        //                    Velocity.X = 10;
        //                }
        //                if (down == true)
        //                {
        //                    Velocity.Y = -10;
        //                }
        //                if (up == true)
        //                {
        //                    Velocity.Y = 10;
        //                }
        //            }
        //            if (image.spriteSheetEffect.CurrentFrame.X >= image.spriteSheetEffect.AmountOfFrames.X - 2)
        //            {
        //                Rolling = false;
        //                image.spriteSheetEffect.CurrentFrame.Y -= 4;
        //                IncreaseVelocity = true;
        //                Velocity = Vector2.Zero;
        //            }
        //        }
        //        if (map.Horizontal == true && Attacking == false && AtHorizontalEdge == false && map.EdgeHorizontal == false)
        //        {
        //            image.Position.X = OriginalPosition.X - PlayerMoved.X;
        //            map.Moved.X = map.prevMoved.X;
        //            if (((map.ScreenWidth / 2) + 5 >= image.Position.X && image.Position.X >= (map.ScreenWidth / 2) - 5))
        //            {
        //                image.Position.X = map.ScreenWidth / 2;
        //                map.Horizontal = false;
        //            }
        //        }
        //        if (map.Vertical == true && Attacking == false && AtVerticalEdge == false && map.EdgeVertical == false)
        //        {
        //            image.Position.Y = OriginalPosition.Y - PlayerMoved.Y;
        //            map.Moved.Y = map.prevMoved.Y;
        //            if ((map.ScreenHeight / 2) + 5 >= image.Position.Y && image.Position.Y >= (map.ScreenHeight / 2) - 5)
        //            {
        //                image.Position.Y = map.ScreenHeight / 2;
        //                map.Vertical = false;
        //            }
        //        }
        //        if (Velocity.X == 0 && Velocity.Y == 0 && Attacking == false && Rolling == false)
        //        {
        //            image.IsActive = false;
        //        }
        //        image.Update(gameTime);
        //        LevelUp.Update(gameTime);
        //        HavePoints.Update(gameTime);
        //    }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            image.Draw(spriteBatch);
            if (Leveled == true)
            {
                if (LevelUp.spriteSheetEffect.CurrentFrame.X == 10)
                {
                    Leveled = false;
                    LevelUp.IsActive = false;
                }
                LevelUp.Draw(spriteBatch);
            }
            if (StatPoints > 0)
            {
                HavePoints.Draw(spriteBatch);
            }
        }
    }
}