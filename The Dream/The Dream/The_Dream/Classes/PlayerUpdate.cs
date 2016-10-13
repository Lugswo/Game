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
        public Textures textures;
        public SoundManager soundManager;
        [XmlIgnore]
        public Rectangle HitBox;
        public Map map = new Map();
        public Vector2 Velocity, PlayerMoved, PrevPlayerMoved, OriginalPosition;
        public float MoveSpeed;
        public int playerHeight, playerWidth, Combo, Health, Energy, EXP, Level, NextLevel, Strength, Defense, Dexterity, Intelligence, Speed, Direction, StatPoints;
        public bool Attacking, SetZero, NextAttack, Rolling, IncreaseVelocity, AtHorizontalEdge, AtVerticalEdge, Leveled, addMonsters;
        public List<Rectangle> AttackHitboxes;
        public string Name;
        int Attacks, prevY;
        public int Height, Width;
        public PlayerUpdate()
        {
            Width = 54;
            Height = 64;
            addMonsters = false;
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
        public void NewArea(ref Player player, bool Up, bool Down, bool Left, bool Right)
        {
            if (Up == true)
            {
                player.Y = player.DeadZone.Bottom - Height;
                player.AreaY--;
            }
            else if (Down == true)
            {
                player.Y = player.DeadZone.Top;
                player.AreaY++;
            }
            else if (Left == true)
            {
                player.X = player.DeadZone.Right - Width;
                player.AreaX--;
            }
            else if (Right == true)
            {
                player.X = player.DeadZone.Left;
                player.AreaX++;
            }
        }
        public void Move(GameTime gameTime, ref Player player, bool Up, bool Down, bool Left, bool Right)
        {
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
            if (Attacking == true)
            {
                if (SetZero == true)
                {
                    Attacks += 8;
                    prevY = (int)image.spriteSheetEffect.CurrentFrame.Y;
                    image.spriteSheetEffect.CurrentFrame.Y += Attacks;
                    image.spriteSheetEffect.CurrentFrame.X = 0;
                    SetZero = false;
                    Combo = 0;
                    Direction = prevY;
                }
                if (NextAttack == true)
                {
                    if (image.spriteSheetEffect.CurrentFrame.X == image.spriteSheetEffect.AmountOfFrames.X - 1)
                    {
                        soundManager.soundEffects["Attack"].soundEffect.Play();
                        NextAttack = false;
                        Combo++;
                        image.spriteSheetEffect.CurrentFrame.Y += 4;
                        Attacks += 4;
                        image.spriteSheetEffect.CurrentFrame.X = 0;
                        if (Attacks >= image.spriteSheetEffect.AmountOfFrames.Y)
                        {
                            Attacks = 8;
                            Combo = 0;
                            image.spriteSheetEffect.CurrentFrame.Y = prevY + Attacks;
                        }
                    }
                }
                if (image.spriteSheetEffect.CurrentFrame.X == image.spriteSheetEffect.AmountOfFrames.X - 1)
                {
                    Attacking = false;
                    Attacks = 0;
                    image.spriteSheetEffect.CurrentFrame.Y = prevY;
                    SetZero = true;
                    Direction = 0;
                }
            }
            //foreach (MapSprite blank in player.Blanks)
            //{
            //    if (blank.HitBox.Left < player.PositionX + Width && blank.HitBox.Right > player.PositionX)
            //    {
            //        blank.Column = true;
            //    }
            //    else
            //    {
            //        blank.Column = false;
            //    }
            //    if (blank.HitBox.Top < player.PositionY + Height && blank.HitBox.Bottom > player.PositionY)
            //    {
            //        blank.Row = true;
            //    }
            //    else
            //    {
            //        blank.Row = false;
            //    }
            //    if (blank.Row == true)
            //    {
            //        if (blank.Left.Intersects(pHitBox))
            //        {
            //            if (player.VelocityX < 0)
            //            {
            //                player.VelocityX = -10;
            //            }
            //            else
            //            {
            //                player.VelocityX = 0;
            //            }
            //        }

            //        if (blank.Right.Intersects(pHitBox))
            //        {
            //            if (player.VelocityX > 0)
            //            {
            //                player.VelocityX = 10;
            //            }
            //            else
            //            {
            //                player.VelocityX = 0;
            //            }
            //        }
            //    }
            //    if (blank.Column == true)
            //    {
            //        if (blank.Down.Intersects(pHitBox))
            //        {
            //            if (player.VelocityY > 0)
            //            {
            //                player.VelocityY = 10;
            //            }
            //            else
            //            {
            //                player.VelocityY = 0;
            //            }
            //        }
            //        if (blank.Up.Intersects(pHitBox))
            //        {
            //            if (player.VelocityY < 0)
            //            {
            //                player.VelocityY = -10;
            //            }
            //            else
            //            {
            //                player.VelocityY = 0;
            //            }
            //        }
            //    }
            //}

            //foreach (Sprite sprite in textures.Sprites)
            //{
            //    if (sprite.HitBox.Left < player.PositionX + Width && sprite.HitBox.Right > player.PositionX)
            //    {
            //        sprite.Column = true;
            //    }
            //    else
            //    {
            //        sprite.Column = false;
            //    }
            //    if (sprite.HitBox.Top < player.PositionY + Height && sprite.HitBox.Bottom > player.PositionY)
            //    {
            //        sprite.Row = true;
            //    }
            //    else
            //    {
            //        sprite.Row = false;
            //    }
            //    if (sprite.Row == true)
            //    {
            //        if (sprite.Left.Intersects(pHitBox))
            //        {
            //            if (player.VelocityX < 0)
            //            {
            //                player.VelocityX = -10;
            //            }
            //            else
            //            {
            //                player.VelocityX = 0;
            //            }
            //        }

            //        if (sprite.Right.Intersects(pHitBox))
            //        {
            //            if (player.VelocityX > 0)
            //            {
            //                player.VelocityX = 10;
            //            }
            //            else
            //            {
            //                player.VelocityX = 0;
            //            }
            //        }
            //    }
            //    if (sprite.Column == true)
            //    {
            //        if (sprite.Down.Intersects(pHitBox))
            //        {
            //            if (player.VelocityY > 0)
            //            {
            //                player.VelocityY = 10;
            //            }
            //            else
            //            {
            //                player.VelocityY = 0;
            //            }
            //        }
            //        if (sprite.Up.Intersects(pHitBox))
            //        {
            //            if (player.VelocityY < 0)
            //            {
            //                player.VelocityY = -10;
            //            }
            //            else
            //            {
            //                player.VelocityY = 0;
            //            }
            //        }
            //    }
            //}

            player.X += player.VelocityX;
            player.Y += player.VelocityY;

            //if (map.Area[player.AreaX, player.AreaY + 1] != null)
            //{
            //    if (player.Y > map.DeadZone.Bottom - Height)
            //    {
            //        player.AreaY++;
            //        GetDeadZone(player.AreaX, player.AreaY);
            //        player.Y = map.DeadZone.Top;
            //        player.newArea = true;
            //    }
            //}
            //else
            //{
            //    if (player.Y > map.DeadZone.Bottom - Height)
            //    {
            //        player.Y = map.DeadZone.Bottom - Height;
            //        player.VelocityY = 0;
            //    }
            //}
            //if (map.Area[player.AreaX, player.AreaY - 1] != null)
            //{
            //    if (player.Y < map.DeadZone.Top)
            //    {
            //        player.AreaY--;
            //        GetDeadZone(player.AreaX, player.AreaY);
            //        player.Y = map.DeadZone.Bottom - Height;
            //        player.newArea = true;
            //    }
            //}
            //else
            //{
            //    if (player.Y < map.DeadZone.Top)
            //    {
            //        player.Y = map.DeadZone.Top;
            //        player.VelocityY = 0;
            //    }
            //}
            //if (map.Area[player.AreaX + 1, player.AreaY] != null)
            //{
            //    if (player.X > map.DeadZone.Right - Width)
            //    {
            //        player.AreaX++;
            //        GetDeadZone(player.AreaX, player.AreaY);
            //        player.X = map.DeadZone.Left;
            //        player.newArea = true;
            //    }
            //}
            //else
            //{
            //    if (player.X > map.DeadZone.Right - Width)
            //    {
            //        player.X = map.DeadZone.Right - Width;
            //        player.VelocityX = 0;
            //    }
            //}
            //if (map.Area[player.AreaX - 1, player.AreaY] != null)
            //{
            //    if (player.X < map.DeadZone.Left)
            //    {
            //        player.AreaX--;
            //        GetDeadZone(player.AreaX, player.AreaY);
            //        player.X = map.DeadZone.Right - Width;
            //        player.newArea = true;
            //    }
            //}
            //else
            //{
            //    if (player.X < map.DeadZone.Left)
            //    {
            //        player.X = 0;
            //        player.VelocityX = 0;
            //    }
            //}
            //if (player.newArea == true)
            //{
            //    GetDeadZone(player.AreaY, player.AreaY);
            //}
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