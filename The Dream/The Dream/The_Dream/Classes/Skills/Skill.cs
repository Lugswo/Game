using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes.Skills
{
    public class Skill
    {
        public Image image;
        public Image icon;
        public bool learned, despawn, up, down, left, right, inLag, usable;
        public int SkillID, X, Y, pX, pY, PositionX, PositionY, endLag, startLag, cooldown, level;
        public Rectangle hitBox;
        public int skillDamage;
        public int range, moved, projSpeed;
        public Skill()
        {
            image = new Image();
            icon = new Image();
            learned = false;
            despawn = false;
            moved = 0;
            up = down = left = right = usable = false;
            inLag = true;
            icon.Position.X = 150;
            icon.Position.Y = 500;
            level = 1;
        }
        public virtual void LoadContent(int fX, int fY)
        {
            image.LoadContent();
            icon.LoadContent();
        }
        public virtual void LoadIcon()
        {
            icon.LoadContent();
        }
        public virtual void UnloadContent()
        {

        }
        public virtual void Update(GameTime gameTime)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            image.Draw(spriteBatch);
        }
    }
}
