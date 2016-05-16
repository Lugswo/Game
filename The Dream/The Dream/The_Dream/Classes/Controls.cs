using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace The_Dream.Classes
{
    public class Controls
    {
        public Map map;
        public PlayerUpdate player;
        public UpdateGameMenu gameMenu;
        public SoundManager soundManager;
        bool temp, temp2;
        public Controls()
        {

        }
        public void GetReferences(PlayerUpdate RealPlayer, Map RealMap, UpdateGameMenu RealMenu, SoundManager RealSounds)
        {
            map = RealMap;
            player = RealPlayer;
            gameMenu = RealMenu;
            soundManager = RealSounds;
        }
        public void LoadContent()
        {
            temp = false;
            temp2 = false;
        }
        public void UnloadContent()
        {

        }
        public void Update(GameTime gameTime)
        {
            if (map.Pause == false)
            {
                if (player.Attacking == true)
                {
                    if (InputManager.Instance.KeyPressed(Keys.Z))
                    {
                        if (player.image.spriteSheetEffect.CurrentFrame.X >= player.image.spriteSheetEffect.AmountOfFrames.X - 3)
                        {
                            player.NextAttack = true;
                        }
                    }
                }
                if (InputManager.Instance.KeyPressed(Keys.Z))
                {
                    if (player.Attacking == false)
                    {
                        soundManager.soundEffects["Attack"].soundEffect.Play();
                        player.Attacking = true;
                    }
                }
                if (InputManager.Instance.KeyPressed(Keys.LeftShift))
                {
                    if (player.Rolling == false && (player.Velocity.X != 0 || player.Velocity.Y != 0))
                    {
                        player.Rolling = true;
                    }
                }
                if (player.Attacking == false && player.Rolling == false)
                {
                    if (InputManager.Instance.KeyDown(Keys.Down) && InputManager.Instance.KeyDown(Keys.Up))
                    {
                        player.Velocity.Y = 0;
                    }
                    else if (InputManager.Instance.KeyDown(Keys.Down))
                    {
                        player.Velocity.Y = -player.MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        player.image.spriteSheetEffect.CurrentFrame.Y = 0;
                    }
                    else if (InputManager.Instance.KeyDown(Keys.Up))
                    {
                        player.Velocity.Y = player.MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        player.image.spriteSheetEffect.CurrentFrame.Y = 1;
                    }
                    else
                    {
                        player.Velocity.Y = 0;
                    }
                    if (InputManager.Instance.KeyDown(Keys.Right) && InputManager.Instance.KeyDown(Keys.Left))
                    {
                        player.Velocity.X = 0;
                    }
                    else if (InputManager.Instance.KeyDown(Keys.Right))
                    {
                        player.Velocity.X = -player.MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        player.image.spriteSheetEffect.CurrentFrame.Y = 2;
                    }
                    else if (InputManager.Instance.KeyDown(Keys.Left))
                    {
                        player.Velocity.X = player.MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        player.image.spriteSheetEffect.CurrentFrame.Y = 3;
                    }
                    else
                    {
                        player.Velocity.X = 0;
                    }
                }
                if (InputManager.Instance.KeyUp(Keys.Down) && InputManager.Instance.KeyUp(Keys.Up))
                {
                    player.Velocity.Y = 0;
                }
                if (InputManager.Instance.KeyUp(Keys.Left) && InputManager.Instance.KeyUp(Keys.Right))
                {
                    player.Velocity.X = 0;
                }
            }
            if (map.Pause == true)
            {
                if (gameMenu.InMenu == true)
                {
                    if (InputManager.Instance.KeyPressed(Keys.X) || InputManager.Instance.KeyPressed(Keys.Escape))
                    {
                        gameMenu.InMenu = false;
                    }
                }
                if (gameMenu.InMenu == false)
                {
                    if (InputManager.Instance.KeyPressed(Keys.Z))
                    {
                        gameMenu.InMenu = true;
                    }
                    if (InputManager.Instance.KeyPressed(Keys.Right))
                    {
                        gameMenu.MenuNumber++;
                    }
                    if (InputManager.Instance.KeyPressed(Keys.Left))
                    {
                        gameMenu.MenuNumber--;
                    }
                    if (InputManager.Instance.KeyUp(Keys.Escape))
                    {
                        temp = true;
                    }
                    if (temp == true)
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        {
                            map.Pause = false;
                            temp = false;
                        }
                    }
                }
            }
            if (map.Pause == false)
            {
                if (InputManager.Instance.KeyUp(Keys.Escape))
                {
                    temp2 = true;
                }
                if (temp2 == true)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        map.Pause = true;
                        temp2 = false;
                    }
                }
            }
        }
    }
}
