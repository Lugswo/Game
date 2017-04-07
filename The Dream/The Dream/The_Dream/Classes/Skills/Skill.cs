using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes.Skills
{
    [XmlInclude(typeof(TestSkill))]
    [XmlInclude(typeof(TestSkill2))]
    [Serializable]
    public class Skill
    {
        public const int halfIcon = 50;
        public Image image;
        public Image icon;
        public bool learned, despawn, up, down, left, right, inLag, usable, hovering;
        public int SkillID, X, Y, pX, pY, PositionX, PositionY, endLag, startLag, cooldown, level, areaX, areaY;
        public Rectangle hitBox;
        public Rectangle iconHitBox;
        public int skillDamage;
        public int range, moved, projSpeed;
        public string description;
        [XmlIgnore]
        public Skill parentSkill;
        public List<Skill> requiredSkills;
        public Vector2 offset;
        public List<Image> connectingLines;
        public Skill()
        {
            image = new Image();
            icon = new Image();
            learned = false;
            despawn = false;
            moved = 0;
            up = down = left = right = usable = false;
            inLag = true;
            level = 1;
            hovering = false;
            iconHitBox = new Rectangle();
            requiredSkills = new List<Skill>();
            connectingLines = new List<Image>();
        }
        public virtual void LoadContent(int fX, int fY)
        {
            image.LoadContent();
            icon.LoadContent();
        }
        public virtual void LoadIcon(Skill skill)
        {
            icon.LoadContent();
            icon.Layer = .97f;
            icon.Position = skill.icon.Position + offset;
        }
        public virtual void UnloadContent()
        {

        }
        public virtual void Update(GameTime gameTime)
        {
            iconHitBox.X = (int)icon.Position.X;
            iconHitBox.Y = (int)icon.Position.Y;
            iconHitBox.Width = icon.texture.Width;
            iconHitBox.Height = icon.texture.Height;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            image.Draw(spriteBatch);
        }
        public virtual void DrawIcon(SpriteBatch spriteBatch)
        {
            icon.Draw(spriteBatch);
        }
    }
}
