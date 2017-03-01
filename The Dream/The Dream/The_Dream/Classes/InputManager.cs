using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace The_Dream.Classes
{
    public class InputManager
    {
        KeyboardState currentKeyState, prevKeyState;
        public MouseState currentMouseState, prevMouseState;
        public Point currentMousePosition, prevMousePosition;
        private static InputManager instance;
        public static InputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InputManager();
                }
                return instance;
            }
        }
        public void Update()
        {
            prevKeyState = currentKeyState;
            prevMouseState = currentMouseState;
            prevMousePosition.X = currentMousePosition.X;
            prevMousePosition.Y = currentMousePosition.Y;
            if (!ScreenManager.Instance.IsTransitioning)
            {
                currentKeyState = Keyboard.GetState();
                currentMouseState = Mouse.GetState();
                currentMousePosition = new Point(currentMouseState.X * 2, currentMouseState.Y * 2);
            }
        }
        public bool KeyPressed(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (currentKeyState.IsKeyDown(key) && prevKeyState.IsKeyUp(key))
                {
                    return true;
                }
            }
            return false;
        }
        public bool KeyReleased(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (currentKeyState.IsKeyUp(key) && prevKeyState.IsKeyDown(key))
                {
                    return true;
                }
            }
            return false;
        }
        public bool KeyDown(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (currentKeyState.IsKeyDown(key))
                {
                    return true;
                }
            }
            return false;
        }
        public bool KeyUp(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (currentKeyState.IsKeyUp(key))
                {
                    return true;
                }
            }
            return false;
        }
        public bool LeftClick()
        {
            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }
        public bool MouseInArea(Rectangle rect)
        {
            Rectangle hitBox = rect;
            if (rect.Contains(currentMousePosition))
            {
                return true;
            }
            return false;
        }
        public bool LeftClickArea(Rectangle rect)
        {
            if (MouseInArea(rect))
            {
                if (currentMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
                {
                    return true;
                }
            }
            return false;
        }
        public bool HoldingLeftClick()
        {
            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }
    }
}
