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
        class Bind
        {
            public Image name;
            public Image button;
            public Image skill;
            public Bind()
            {
                name = new Image();
                button = new Image();
            }
            public void LoadContent()
            {
                button.Path = "Gameplay/GUI/Menu/Skills/Skill Box";
                name.Text = "None";
                button.LoadContent();
                name.LoadContent();
                button.Layer = .941f;
                name.Layer = .941f;
            }
            public void UnloadContent()
            {
                button.UnloadContent();
                name.UnloadContent();
            }
            public void Update(GameTime gameTime)
            {
                button.Update(gameTime);
                name.Update(gameTime);
            }
            public void Draw(SpriteBatch spriteBatch)
            {
                button.Draw(spriteBatch);
                name.DrawString(spriteBatch);
            }
            public void DrawFaded(SpriteBatch spriteBatch)
            {
                button.DrawFaded(spriteBatch);
                name.DrawString(spriteBatch);
            }
        }
        public List<Classes.Skills.Skill> skillList;
        public Classes.Skills.TestSkill testSkill;
        public Classes.Skills.TestSkill2 testSkill2;
        Texture2D texture;
        Rectangle rect;
        Color color;
        Vector2 origin, toMove;
        Image cursor;
        Image selection;
        Image skillSelection;
        float mouseScaler;
        int category, skillSelect;
        List<Bind> binds;
        Textbox keyBindBox;
        bool keyBound, releaseZ, setIcons, settingSkill;
        public Skills()
        {
            image.Path = "Gameplay/GUI/Menu/Skills/Skills";
            inImage.Path = "Gameplay/GUI/Menu/Skills/InSkills";
            skillList = new List<Classes.Skills.Skill>();
            testSkill = new Classes.Skills.TestSkill();
            testSkill2 = new Classes.Skills.TestSkill2();
            origin = new Vector2(5000, 5000);
            cursor = new Image();
            mouseScaler = ScreenManager.instance.Dimensions.X / ScreenManager.instance.realDimensions.X;
            toMove = Vector2.Zero;
            subCat = true;
            category = 0;
            binds = new List<Bind>();
            for (int i = 0; i < 10; i++)
            {
                binds.Add(new Bind());
            }
            selection = new Image();
            keyBindBox = new Textbox("Press the key you would like to bind to");
            category = 1;
            keyBound = false;
            releaseZ = false;
            skillSelect = 0;
            setIcons = false;
            settingSkill = false;
            skillSelection = new Image();
        }
        public override void LoadOnPause(Player player)
        {
            base.LoadOnPause(player);
        }
        public override void LoadContent(Player player)
        {
            base.LoadContent(player);
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
            foreach (int i in player.skillIds)
            {
                skillList[i - 1].learned = true;
            }
            texture = new Texture2D(ScreenManager.Instance.GraphicsDevice, 1, 1);
            rect = new Rectangle(-5000, -5000, 10000, 10000);
            cursor.Path = "Gameplay/Skills/TestSkill/Sprite";
            cursor.LoadContent();
            texture.SetData(new Color[] { Color.Black });
            cursor.Layer = .999f;
            foreach (Bind b in binds)
            {
                b.LoadContent();
            }
            binds[0].button.Position.X = xOffset + 50;
            binds[0].button.Position.Y = yOffset + 100;
            for (int i = 1; i < binds.Count / 2; i++)
            {
                binds[i].button.Position.X = binds[i - 1].button.Position.X + binds[i - 1].button.texture.Width + 50;
                binds[i].button.Position.Y = binds[i - 1].button.Position.Y;
            }
            for (int i = binds.Count / 2; i < binds.Count; i++)
            {
                binds[i].button.Position.X = binds[i - (binds.Count / 2)].button.Position.X;
                binds[i].button.Position.Y = binds[0].button.Position.Y + binds[0].button.texture.Height + 100;
            }
            foreach (Bind b in binds)
            {
                b.name.Position.X = b.button.Position.X + ((b.button.texture.Width - b.name.Font.MeasureString("None").X) / 2);
                b.name.Position.Y = b.button.Position.Y - b.name.Font.MeasureString("None").Y;
            }
            selection.Path = "Gameplay/GUI/Menu/Skills/Select";
            selection.LoadContent();
            selection.Layer = .94f;
            keyBindBox.LoadContent();
            skillSelection.Path = "Gameplay/GUI/Menu/Skills/Skill Select";
            skillSelection.Layer = .95f;
            skillSelection.Position.X = 185;
            skillSelection.Position.Y = 100;
            skillSelection.LoadContent();
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
                if (inCategory == false)
                {
                    if (category > 1)
                    {
                        if (InputManager.Instance.KeyPressed(Keys.Left))
                        {
                            category--;
                        }
                    }
                    if (category < binds.Count + 1)
                    {
                        if (InputManager.Instance.KeyPressed(Keys.Right))
                        {
                            category++;
                        }
                    }
                    if (category <= binds.Count / 2)
                    {
                        if (InputManager.Instance.KeyPressed(Keys.Down))
                        {
                            category += 5;
                        }
                    }
                    else
                    {
                        if (InputManager.Instance.KeyPressed(Keys.Down))
                        {
                            category = binds.Count + 1;
                        }
                    }
                    if (category > binds.Count / 2)
                    {
                        if (InputManager.Instance.KeyPressed(Keys.Up))
                        {
                            category -= 5;
                        }
                    }
                }
                if (category == binds.Count + 1)
                {
                    //selection = go to skill tree pos
                }
                else
                {
                    selection.Position = binds[category - 1].button.Position;
                    selection.Position.X -= 5;
                    selection.Position.Y -= 5;
                }
                if (inCategory == true)
                {
                    if (category <= binds.Count)
                    {
                        if (player.skills.Count > 0)
                        {
                            if (settingSkill == false)
                            {
                                if (InputManager.Instance.KeyReleased(Keys.Z))
                                {
                                    releaseZ = true;
                                }
                            }
                            if (releaseZ == true && keyBound == false)
                            {
                                List<Keys> currentKeys;
                                currentKeys = new List<Keys>();
                                Keys[] keys = Keyboard.GetState().GetPressedKeys();
                                foreach (Keys key in keys)
                                {
                                    currentKeys.Add(key);
                                }
                                if (currentKeys.Count > 0)
                                {
                                    binds[category - 1].name.Text = currentKeys[0].ToString();
                                    keyBound = true;
                                    releaseZ = false;
                                    settingSkill = true;
                                    binds[category - 1].name.Position.X = binds[category - 1].button.Position.X + ((binds[category - 1].button.texture.Width - binds[category - 1].name.Font.MeasureString(binds[category - 1].name.Text).X) / 2);
                                    binds[category - 1].name.Position.Y = binds[category - 1].button.Position.Y - binds[category - 1].name.Font.MeasureString(binds[category - 1].name.Text).Y;
                                    player.skill1Bind = currentKeys[0];
                                }
                            }
                            else if (keyBound == true)
                            {
                                if (setIcons == false)
                                {
                                    int count = 0;
                                    int ycount = 0;
                                    foreach (Classes.Skills.Skill skill in player.skills)
                                    {
                                        if (count == 10)
                                        {
                                            ycount++;
                                            count = 0;
                                        }
                                        skill.icon.Layer = .99f;
                                        skill.icon.Position.X = 185 + (count * 150) + 50;
                                        skill.icon.Position.Y = 200 + 50 + (ycount * 100);
                                        count++;
                                    }
                                    setIcons = true;
                                }
                            }
                        }
                        else
                        {
                            //textbox saying you have no skills
                        }
                    }
                    else
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
                                    bool skillLearned = false;
                                    foreach (Classes.Skills.Skill s in player.skills)
                                    {
                                        if (s.SkillID == skill.SkillID)
                                        {
                                            skillLearned = true;
                                            break;
                                        }
                                    }
                                    if (skillLearned == false)
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
                                            int id = skill.SkillID;
                                            player.skillIds.Add(id);
                                            temp.learned = true;
                                            skill.learned = true;
                                            player.skillPoints--;
                                        }
                                    }
                                    else
                                    {
                                        skill.level++;
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
                }
            }
            base.Update(gameTime, player, inMenu);
        }
        public override void Draw(SpriteBatch spriteBatch, bool inMenu, Player player)
        {
            base.Draw(spriteBatch, inMenu, player);
            foreach (Bind b in binds)
            {
                b.DrawFaded(spriteBatch);
            }
            if (inMenu == true)
            {
                selection.Draw(spriteBatch);
                if (inCategory == true)
                {
                    if (category <= binds.Count)
                    {
                        if (keyBound == false)
                        {
                            keyBindBox.Draw(spriteBatch);
                        }
                        else
                        {
                            skillSelection.Draw(spriteBatch);
                            foreach (Classes.Skills.Skill skill in player.skills)
                            {
                                skill.DrawIcon(spriteBatch);
                            }
                        }
                    }
                    else if (category == 11)
                    {
                        spriteBatch.Draw(texture, rect, rect, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, .96f);
                        cursor.Draw(spriteBatch);
                        foreach (Classes.Skills.Skill skill in skillList)
                        {
                            if (skill.learned || skill.hovering == true)
                            {
                                skill.DrawIcon(spriteBatch);
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
                }
            }
            player.skillPointsImage.DrawString(spriteBatch);
        }
    }
}
