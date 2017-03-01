using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes
{
    public class CharacterCreationScreen : GameScreen
    {
        Image NameImage;
        Image characterCreationGUI;
        Image arrow;
        Image characterSprite;
        Image coverImage;
        Image backgroundImage;
        Image selectedCategory;
        Image select;
        Image cursor;
        Image currentHair;
        Image currentEyes;
        string charName;
        List<Keys> currentKeys;
        [XmlElement("list")]
        [XmlElementAttribute(Type = typeof(List<Image>))]
        public List<List<Image>> categories;
        public XmlManager<CharacterCreationScreen> characterCreate;
        bool moving, inCategory, closeCategory, typingName, initialMove, pick;
        int moved, toMove, selectedCategoryNumber, selected, initialToMove, prevToMove;
        public void CreateCharacterSave(string charName)
        {
            XmlDocument SaveFile = new XmlDocument();
            SaveFile.Load("Load/Gameplay/Player.xml");
            XmlNode node;
            node = SaveFile.DocumentElement;
            foreach (XmlNode node1 in node.ChildNodes)
            {
                if (node1.Name == "Name")
                {
                    node1.InnerText = charName;
                }
                if (node1.Name == "hair")
                {
                    foreach (XmlNode node2 in node1.ChildNodes)
                    {
                        if (node2.Name == "Path")
                        {
                            node2.InnerText = currentHair.Path;
                        }
                    }
                }
                if (node1.Name == "eyes")
                {
                    foreach (XmlNode node2 in node1.ChildNodes)
                    {
                        if (node2.Name == "Path")
                        {
                            node2.InnerText = currentEyes.Path;
                        }
                    }
                }
            }
            SaveFile.Save("Load/Gameplay/SaveFile.xml");
            //using (XmlWriter writer = XmlWriter.Create("Save.xml"))
            //{
            //    writer.WriteStartDocument();
            //    writer.WriteStartElement("Player");
            //    writer.WriteElementString("Name", charName);
            //    writer.WriteEndElement();
            //    writer.WriteEndDocument();
            //}
        }
        public CharacterCreationScreen()
        {
            charName = String.Empty;
            NameImage = new Image();
            characterCreate = new XmlManager<CharacterCreationScreen>();
            characterCreationGUI = new Image();
            arrow = new Image();
            arrow.Path = "GUI/Arrow";
            characterCreationGUI.Path = "GUI/Character Creation/Character Creation";
            characterSprite = new Image();
            characterSprite.Layer = .91f;
            currentKeys = new List<Keys>();
            categories = new List<List<Image>>();
            coverImage = new Image();
            backgroundImage = new Image();
            selectedCategory = new Image();
            select = new Image();
            cursor = new Image();
            currentHair = new Image();
            currentEyes = new Image();
            moving = false;
            moved = 0;
            toMove = 0;
            selectedCategoryNumber = 0;
            selected = 0;
            inCategory = false;
            closeCategory = false;
            initialMove = false;
            pick = false;
        }
        public override void LoadContent()
        {
            base.LoadContent();
            characterCreationGUI.LoadContent();
            characterSprite.Effects = "ShowSpriteEffect";
            characterSprite.Path = "Gameplay/Characters/Player/Player";
            characterSprite.LoadContent();
            characterSprite.showSpriteEffect.AmountOfSprites = new Vector2(6, 16);
            characterSprite.showSpriteEffect.Sprite = new Vector2(1, 0);
            characterSprite.Position.X = ScreenManager.instance.Dimensions.X / 2;
            characterSprite.Position.Y = ScreenManager.instance.Dimensions.Y / 2;
            coverImage.rect.X = 1623;
            coverImage.rect.Y = 35;
            coverImage.rect.Width = 300;
            coverImage.rect.Height = 1080;
            coverImage.Layer = .92f;
            coverImage.Path = "GUI/Character Creation/Cover";
            coverImage.LoadContent();
            backgroundImage.rect.X = 1623;
            backgroundImage.rect.Y = 0;
            backgroundImage.rect.Width = 300;
            backgroundImage.rect.Height = 1080;
            backgroundImage.Path = "GUI/Character Creation/Background";
            backgroundImage.LoadContent();
            selectedCategory.Path = "GUI/Character Creation/Selected Category";
            selectedCategory.Layer = .94f;
            selectedCategory.Alpha = 0.0f;
            selectedCategory.LoadContent();
            select.Path = "GUI/Character Creation/Selected";
            select.Layer = .93f;
            select.Alpha = 0.0f;
            select.LoadContent();
            cursor.Path = "GUI/Cursor";
            cursor.Layer = .99f;
            cursor.LoadContent();
            currentHair.Path = "Gameplay/Characters/Player/Hair/Hair1";
            currentHair.Layer = .91f;
            currentHair.Position = characterSprite.Position;
            currentHair.LoadContent();
            currentEyes.Path = "Gameplay/Characters/Player/Eyes/Eyes1";
            currentEyes.Layer = .91f;
            currentEyes.Position = characterSprite.Position;
            currentEyes.LoadContent();
            NameImage.Text = "Name: ";
            NameImage.LoadContent();
            NameImage.Position.X = characterSprite.Position.X - 300;
            NameImage.Position.Y = characterSprite.Position.Y - NameImage.dimensions.Y - 5;
            NameImage.Layer = .91f;
            NameImage.rect.X = (int)NameImage.Position.X;
            NameImage.rect.Y = (int)NameImage.Position.Y;
            NameImage.rect.Width = 500;
            NameImage.rect.Height = (int)NameImage.dimensions.Y;
            foreach (List<Image> list in categories)
            {
                foreach (Image im in list)
                {
                    im.LoadContent();
                }
            }
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
            NameImage.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Keys[] keys = Keyboard.GetState().GetPressedKeys();
            currentKeys.Clear();
            foreach (Keys key in keys)
            {
                if (key != Keys.LeftShift)
                {
                    currentKeys.Add(key);
                }
            }
            cursor.Position.X = InputManager.Instance.currentMousePosition.X;
            cursor.Position.Y = InputManager.Instance.currentMousePosition.Y;
            if (InputManager.Instance.LeftClickArea(NameImage.rect))
            {
                typingName = true;
            }
            if (typingName == true)
            {
                foreach (Keys currentKey in currentKeys)
                {
                    if (InputManager.Instance.KeyPressed(currentKey) && currentKey != Keys.Back && currentKey != Keys.Space && currentKey != Keys.Enter)
                    {
                        if (InputManager.Instance.KeyDown(Keys.LeftShift) || InputManager.Instance.KeyDown(Keys.RightShift))
                        {
                            if (currentKey.ToString().Length == 1)
                            {
                                charName += currentKey.ToString();
                            }
                        }
                        else
                        {
                            if (currentKey.ToString().Length == 1)
                            {
                                charName += currentKey.ToString().ToLower();
                            }
                        }
                    }
                    else if (InputManager.Instance.KeyPressed(Keys.Back))
                    {
                        if (charName.Length > 0)
                        {
                            charName = charName.Remove(charName.Length - 1);
                        }
                    }
                    else if (InputManager.Instance.KeyPressed(Keys.Space))
                    {
                        charName += " ";
                    }
                }
                int temp = (int)NameImage.Font.MeasureString(charName).X;
                if (InputManager.Instance.KeyPressed(Keys.Enter))
                {
                    CreateCharacterSave(charName);
                    ScreenManager.Instance.ChangeScreens("SetHostScreen");
                }
                NameImage.Text = "Name: " + charName;
            }
            characterSprite.Update(gameTime);
            foreach (List<Image> list in categories)
            {
                if (InputManager.Instance.MouseInArea(list[1].rect))
                {
                    selectedCategory.Position = list[1].Position;
                    selectedCategory.Alpha = 1.0f;
                    break;
                }
                selectedCategory.Alpha = 0.0f;
            }
            if (moving == false)
            {
                if (inCategory == true)
                {
                    if (pick == false)
                    {
                        foreach (Image im in categories[selectedCategoryNumber])
                        {
                            if (categories[selectedCategoryNumber].IndexOf(im) != 0 && categories[selectedCategoryNumber].IndexOf(im) != 1)
                            {
                                if (InputManager.Instance.LeftClickArea(im.rect))
                                {
                                    pick = true;
                                    if (selectedCategoryNumber == 0)
                                    {
                                        currentHair = new Image();
                                        currentHair.Path = "Gameplay/Characters/Player/" + im.Path.Remove(0, 23);
                                        currentHair.Position = characterSprite.Position;
                                        currentHair.Layer = .92f;
                                        currentHair.LoadContent();
                                    }
                                    if (selectedCategoryNumber == 1)
                                    {
                                        currentEyes = new Image();
                                        currentEyes.Path = "Gameplay/Characters/Player/" + im.Path.Remove(0, 23);
                                        currentEyes.Position = characterSprite.Position;
                                        currentEyes.Layer = .92f;
                                        currentEyes.LoadContent();
                                    }
                                }
                                if (InputManager.Instance.MouseInArea(im.rect))
                                {
                                    select.Position = im.Position;
                                    select.Alpha = 1.0f;
                                    break;
                                }
                                select.Alpha = 0.0f;
                            }
                        }
                    }
                    else
                    {
                        closeCategory = true;
                        inCategory = false;
                        moving = true;
                    }
                }
                else
                {
                    foreach (List<Image> list in categories)
                    {
                        if (InputManager.Instance.LeftClickArea(list[1].rect))
                        {
                            inCategory = true;
                            moving = true;
                            selectedCategoryNumber = categories.IndexOf(list);
                            foreach (List<Image> list2 in categories)
                            {
                                if (categories.IndexOf(list2) < selectedCategoryNumber)
                                {
                                    toMove += list2[1].texture.Height;
                                }
                                if (categories.IndexOf(list2) > selectedCategoryNumber)
                                {
                                    list2[0].Alpha = 0.0f;
                                    list2[1].Alpha = 0.0f;
                                }
                            }
                            foreach (Image im in categories[selectedCategoryNumber])
                            {
                                im.Alpha = 1.0f;
                            }
                        }
                    }
                }
            }
            else
            {
                if (closeCategory == true)
                {
                    toMove = prevToMove;
                    if (initialMove == false)
                    {
                        if (moved < toMove)
                        {
                            coverImage.rect.Y -= 10;
                            moved += 10;
                        }
                        if (moved >= toMove)
                        {
                            foreach (Image im in categories[selectedCategoryNumber])
                            {
                                if (categories[selectedCategoryNumber].IndexOf(im) != 0 && categories[selectedCategoryNumber].IndexOf(im) != 1)
                                im.Alpha = 0.0f;
                            }
                            moved = 0;
                            initialMove = true;
                        }
                    }
                    else
                    {
                        toMove = initialToMove;
                        if (moved < toMove)
                        {
                            foreach (List<Image> list in categories)
                            {
                                list[0].Position.Y += 10;
                                list[1].Position.Y += 10;
                                list[1].rect.Y += 10;
                            }
                            moved += 10;
                        }
                        else
                        {
                            moving = false;
                            moved = 0;
                            toMove = 0;
                            initialMove = false;
                            closeCategory = false;
                            pick = false;
                            foreach (List<Image> list in categories)
                            {
                                list[0].Alpha = 1.0f;
                                list[1].Alpha = 1.0f;
                            }
                        }
                    }
                }
                else
                {
                    if (initialMove == false)
                    {
                        if (moved < toMove)
                        {
                            foreach (List<Image> list in categories)
                            {
                                list[0].Position.Y -= 10;
                                list[1].Position.Y -= 10;
                                list[1].rect.Y -= 10;
                            }
                            moved += 10;
                        }
                        if (moved >= toMove)
                        {
                            initialToMove = moved;
                            moved = 0;
                            toMove = 0;
                            int count = 2;
                            foreach (Image im in categories[selectedCategoryNumber])
                            {
                                if (categories[selectedCategoryNumber].IndexOf(im) != 0 && categories[selectedCategoryNumber].IndexOf(im) != 1)
                                {
                                    count++;
                                    if (count == 3)
                                    {
                                        toMove += im.texture.Height;
                                        count = 0;
                                    }
                                }
                            }
                            initialMove = true;
                        }
                    }
                    else
                    {
                        if (moved < toMove)
                        {
                            coverImage.rect.Y += 10;
                            moved += 10;
                        }
                        else
                        {
                            prevToMove = moved;
                            moving = false;
                            moved = 0;
                            toMove = 0;
                            initialMove = false;
                        }
                    }
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            characterCreationGUI.Draw(spriteBatch);
            NameImage.DrawString(spriteBatch);
            characterSprite.Draw(spriteBatch);
            coverImage.DrawToRectangle(spriteBatch);
            backgroundImage.DrawToRectangle(spriteBatch);
            if (inCategory == false)
            {
                selectedCategory.Draw(spriteBatch);
            }
            cursor.Draw(spriteBatch);
            if (inCategory == true)
            {
                select.Draw(spriteBatch);
            }
            currentHair.Draw(spriteBatch);
            currentEyes.Draw(spriteBatch);
            foreach (List<Image> list in categories)
            {
                list[0].DrawString(spriteBatch);
                foreach (Image im in list)
                {
                    im.Draw(spriteBatch);
                }
            }
        }
    }
}
