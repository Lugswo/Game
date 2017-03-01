using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace The_Dream.Classes.GameMenu
{
    public class Skills : MenuTab
    {
        public List<Classes.Skills.Skill> skillList;
        public Classes.Skills.TestSkill testSkill;
        public Classes.Skills.TestSkill2 testSkill2;
        Texture2D texture;
        Rectangle rect;
        Color color;
        Vector2 origin, toMove;
        Image cursor;
        float mouseScaler;
        public Skills()
        {
            image.Path = "Gameplay/GUI/Menu/Skills";
            inImage.Path = "Gameplay/GUI/Menu/InSkills";
            skillList = new List<Classes.Skills.Skill>();
            testSkill = new Classes.Skills.TestSkill();
            testSkill2 = new Classes.Skills.TestSkill2();
            origin = new Vector2(5000, 5000);
            cursor = new Image();
            mouseScaler = ScreenManager.instance.Dimensions.X / ScreenManager.instance.realDimensions.X;
            toMove = Vector2.Zero;
        }
        public override void LoadOnPause(Player player)
        {
            base.LoadOnPause(player);
        }
        public override void LoadContent()
        {
            base.LoadContent();
            skillList.Add(testSkill);
            skillList.Add(testSkill2);
            foreach (Classes.Skills.Skill skill in skillList)
            {
                if (skill.SkillID != 1)
                {
                    Classes.Skills.Skill temp = new Classes.Skills.Skill();
                    temp = skillList.Find(x => x.SkillID == skill.parentSkill.SkillID);
                    skill.LoadIcon(temp);
                }
                else
                {
                    skill.icon.LoadContent();
                    skill.icon.Layer = .97f;
                }
                if (skill.requiredSkills.Count > 0)
                {
                    foreach (Classes.Skills.Skill skill2 in skill.requiredSkills)
                    {
                        Image temp = new Image();
                        temp.Path = "Gameplay/Skills/Skill Line";
                        temp.LoadContent();
                        skill.connectingLines.Add(temp);
                        temp.rect.X = (int)skill.icon.Position.X + (skill.icon.texture.Width / 2) - 1;
                        temp.rect.Y = (int)skill.icon.Position.Y + (skill.icon.texture.Height) - 1;
                        temp.rect.Width = ((int)skill2.icon.Position.X + (skill.icon.texture.Width / 2) + 1) - temp.rect.X;
                        temp.rect.Height = ((int)skill2.icon.Position.Y + (skill.icon.texture.Height / 2) + 1) - temp.rect.Y;
                        temp.Layer = .961f;
                    }
                }
            }
            texture = new Texture2D(ScreenManager.Instance.GraphicsDevice, 1, 1);
            rect = new Rectangle(-5000, -5000, 10000, 10000);
            texture.SetData(new Color[] { Color.Black });
            cursor.Path = "Gameplay/Skills/TestSkill/Sprite";
            cursor.LoadContent();
            cursor.Layer = .99f;
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime, Player player, bool inMenu)
        {
            foreach (Classes.Skills.Skill skill in skillList)
            {
                skill.Update(gameTime);
            }
            cursor.Position.X = InputManager.Instance.currentMousePosition.X;
            cursor.Position.Y = InputManager.Instance.currentMousePosition.Y;
            player.skillPointsImage.color = new Color(255, 255, 255, 255);
            player.skillPointsImage.Text = "Skill Points: " + player.skillPoints.ToString();
            if (inMenu == true)
            {
                foreach (Classes.Skills.Skill skill in skillList)
                {
                    if (InputManager.Instance.MouseInArea(skill.iconHitBox))
                    {
                        skill.hovering = true;
                    }
                    else
                    {
                        skill.hovering = false;
                    }
                    if (InputManager.Instance.LeftClickArea(skill.iconHitBox))
                    {
                        if (player.skillPoints > 0)
                        {
                            bool canLearn = false;
                            if (skill.SkillID == 1)
                            {
                                canLearn = true;
                            }
                            foreach (Classes.Skills.Skill skill2 in skill.requiredSkills)
                            {
                                Classes.Skills.Skill temp3 = new Classes.Skills.Skill();
                                temp3 = skillList.Find(x => x.SkillID == skill2.SkillID);
                                if (temp3.learned == true)
                                {
                                    canLearn = true;
                                    break;
                                }
                            }
                            if (canLearn == true)
                            {
                                Classes.Skills.Skill temp = new Classes.Skills.Skill();
                                temp = (Classes.Skills.Skill)Activator.CreateInstance(skill.GetType());
                                temp.icon.LoadContent();
                                player.skills.Add(temp);
                                temp.learned = true;
                                Classes.Skills.Skill temp2 = new Classes.Skills.Skill();
                                temp2 = skillList.Find(x => x.SkillID == skill.SkillID);
                                temp2.learned = true;
                                player.skillPoints--;
                            }
                        }
                    }
                }
                if (InputManager.Instance.HoldingLeftClick())
                {
                    toMove = new Vector2(InputManager.Instance.currentMousePosition.X - InputManager.Instance.prevMousePosition.X, InputManager.Instance.currentMousePosition.Y - InputManager.Instance.prevMousePosition.Y);
                    foreach (Classes.Skills.Skill skill in skillList)
                    {
                        skill.icon.Position += toMove;
                        foreach (Image im in skill.connectingLines)
                        {
                            im.rect.X += (int)toMove.X;
                            im.rect.Y += (int)toMove.Y;
                        }
                    }
                }
            }
            base.Update(gameTime, player, inMenu);
        }
        public override void Draw(SpriteBatch spriteBatch, bool inMenu, Player player)
        {
            base.Draw(spriteBatch, inMenu, player);
            if (inMenu == true)
            {
                spriteBatch.Draw(texture, rect, rect, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, .96f);
                cursor.Draw(spriteBatch);
                foreach (Classes.Skills.Skill skill in skillList)
                {
                    if (skill.learned || skill.hovering == true)
                    {
                        skill.icon.Draw(spriteBatch);
                    }
                    else
                    {
                        skill.icon.DrawFaded(spriteBatch);
                    }
                    foreach (Image im in skill.connectingLines)
                    {
                        im.DrawToRectangle(spriteBatch);
                    }
                }
            }
            player.skillPointsImage.DrawString(spriteBatch);
        }
    }
}
