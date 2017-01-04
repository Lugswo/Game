using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes
{
    public class CharacterCreationScreen : GameScreen
    {
        Image NameImage;
        string charName;
        Keys currentKey;
        public XmlManager<CharacterCreationScreen> characterCreate;
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
                foreach (XmlNode node2 in node1.ChildNodes)
                {

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
        }
        public override void LoadContent()
        {
            base.LoadContent();
            NameImage.LoadContent();
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
            foreach (Keys key in keys)
            {
                if (key != Keys.LeftShift)
                {
                    currentKey = key;
                }
            }
            if (InputManager.Instance.KeyPressed(currentKey) && currentKey != Keys.Back && currentKey != Keys.Space && currentKey != Keys.Enter)
            {
                if (InputManager.Instance.KeyDown(Keys.LeftShift))
                {
                    charName += currentKey.ToString();
                }
                else
                {
                    charName += currentKey.ToString().ToLower();
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
            int temp = (int)NameImage.Font.MeasureString(charName).X;
            NameImage = new Image();
            if (temp > 281)
            {
                charName = charName.Remove(charName.Length - 1);
            }
            NameImage.Text = charName;
            NameImage.LoadContent();
            if (InputManager.Instance.KeyPressed(Keys.Enter))
            {
                CreateCharacterSave(charName);
                ScreenManager.Instance.ChangeScreens("SetHostScreen");
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            NameImage.Draw(spriteBatch);
        }
    }
}
