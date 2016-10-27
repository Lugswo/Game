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
        public bool newArea, aUp, aDown, aLeft, aRight;
        public PlayerUpdate()
        {
            newArea = aUp = aDown = aLeft = aRight = false;
        }
        public void LoadContent()
        {

        }
        public void UnloadContent()
        {

        }
        public void Move(GameTime gameTime, ref Player player, Map map)
        {
            if (player.Down == true)
            {
                player.VelocityY = 10;
            }
            else if (player.Up == true)
            {
                player.VelocityY = -10;
            }
            if (player.Right == true)
            {
                player.VelocityX = 10;
            }
            else if (player.Left == true)
            {
                player.VelocityX = -10;
            }
            if (player.Up == false && player.Down == false)
            {
                player.VelocityY = 0;
            }
            if (player.Left == false && player.Right == false)
            {
                player.VelocityX = 0;
            }
            foreach (Sprite sprite in map.Sprites)
            {
                if (sprite.Up.Intersects(player.HitBox))
                {
                    if (player.Down == true)
                    {
                        player.VelocityY = 0;
                    }
                    else if (player.Up == true)
                    {
                        player.VelocityY = -10;
                    }
                }
                if (sprite.Down.Intersects(player.HitBox))
                {
                    if (player.Up == true)
                    {
                        player.VelocityY = 0;
                    }
                    else if (player.Down == true)
                    {
                        player.VelocityY = 10;
                    }
                }
                if (sprite.Left.Intersects(player.HitBox))
                {
                    if (player.Right == true)
                    {
                        player.VelocityX = 0;
                    }
                    else if (player.Left == true)
                    {
                        player.VelocityX = -10;
                    }
                }
                if (sprite.Right.Intersects(player.HitBox))
                {
                    if (player.Left == true)
                    {
                        player.VelocityX = 0;
                    }
                    else if (player.Right == true)
                    {
                        player.VelocityX = 10;
                    }
                }
            }
            if (newArea == false)
            {
                if (player.HitBox.Left <= map.DeadZone.Left)
                {
                    if (map.Area[player.AreaX - 1, player.AreaY] != null)
                    {
                        if (player.Left == true)
                        {
                            player.AreaX--;
                            newArea = true;
                            aLeft = true;
                        }
                    }
                    else
                    {
                        if (player.Left == true)
                        {
                            player.VelocityX = 0;
                        }
                        else if (player.Right == true)
                        {
                            player.VelocityX = 10;
                        }
                    }
                }
                if (player.HitBox.Right >= map.DeadZone.Right)
                {
                    if (map.Area[player.AreaX + 1, player.AreaY] != null)
                    {
                        if (player.Right == true)
                        {
                            player.AreaX++;
                            newArea = true;
                            aRight = true;
                        }
                    }
                    else
                    {
                        if (player.Right == true)
                        {
                            player.VelocityX = 0;
                        }
                        else if (player.Left == true)
                        {
                            player.VelocityX = -10;
                        }
                    }
                }
                if (player.HitBox.Top <= map.DeadZone.Top)
                {
                    if (map.Area[player.AreaY - 1, player.AreaY] != null)
                    {
                        if (player.Up == true)
                        {
                            player.AreaY--;
                            newArea = true;
                            aUp = true;
                        }
                    }
                    else
                    {
                        if (player.Up == true)
                        {
                            player.VelocityY = 0;
                        }
                        else if (player.Down == true)
                        {
                            player.VelocityY = 10;
                        }
                    }
                }
                if (player.HitBox.Bottom >= map.DeadZone.Bottom)
                {
                    if (map.Area[player.AreaY + 1, player.AreaY] != null)
                    {
                        if (player.Down == true)
                        {
                            player.AreaY++;
                            newArea = true;
                            aDown = true;
                        }
                    }
                    else
                    {
                        if (player.Down == true)
                        {
                            player.VelocityY = 0;
                        }
                        else if (player.Up == true)
                        {
                            player.VelocityY = -10;
                        }
                    }
                }
            }
            player.X += player.VelocityX;
            player.Y += player.VelocityY;
        }
    }
}