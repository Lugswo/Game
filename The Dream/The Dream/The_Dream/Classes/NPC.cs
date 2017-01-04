using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes
{
    public class NPC
    {
        public Image image;
        public Image interactImage;
        public Rectangle HitBox;
        public Vector2 OriginalPosition;
        [XmlElement("Greeting")]
        public List<string> Greetings;
        [XmlElement("Dialogue")]
        public List<string> Dialogue;
        [XmlElement("Farewell")]
        public List<string> Farewells;
        public bool interactable;
        public bool Talking;
        public Random random;
        public Image text;
        public Image name;
        public Image portrait;
        public string Name;
        string[] splitText;
        string extraText;
        public bool initiated, broken, bGreeting, bDialogue, bFarewell, dialogueEnded;
        public NPC()
        {
            interactImage = new Image();
            interactImage.Path = "Gameplay/Characters/Emotes/Speech Bubble";
            interactable = false;
            random = new Random();
            text = new Image();
            name = new Image();
            initiated = false;
            extraText = string.Empty;
            dialogueEnded = false;
        }
        public string BrokenDialogue()
        {
            string brokenText = string.Empty;
            int dim = 0;
            int line = 830;
            if (extraText.Length > 0)
            {
                splitText = extraText.Split(' ');
                extraText = string.Empty;
            }
            foreach (string s in splitText)
            {
                dim += (int)name.Font.MeasureString(s).X;
                dim += (int)name.Font.MeasureString(" ").X;
                if (dim > ScreenManager.instance.Dimensions.X - 40)
                {
                    brokenText += "\n";
                    line += (int)name.Font.MeasureString(s).Y;
                    if (line > ScreenManager.instance.Dimensions.Y)
                    {
                        broken = true;
                        break;
                    }
                    else
                    {
                        broken = false;
                    }
                    dim = 0;
                    dim += (int)name.Font.MeasureString(s).X;
                    dim += (int)name.Font.MeasureString(" ").X;
                    brokenText += s + " ";
                }
                else
                {
                    brokenText += s + " ";
                }
            }
            if (broken == true)
            {
                bool addToExtra = false;
                dim = 0;
                line = 830;
                foreach (string s in splitText)
                {
                    dim += (int)name.Font.MeasureString(s).X;
                    dim += (int)name.Font.MeasureString(" ").X;
                    if (dim > ScreenManager.instance.Dimensions.X - 40)
                    {
                        dim = 0;
                        line += (int)name.Font.MeasureString(s).Y;
                        if (line > ScreenManager.instance.Dimensions.Y)
                        {
                            addToExtra = true;
                        }
                    }
                    if (addToExtra == true)
                    {
                        extraText += s + " ";
                    }
                }
            }
            if (extraText.Length > 0)
            {
                extraText = extraText.Remove(extraText.Length - 1);
            }
            return brokenText;
        }
        public void ContinueDialogue()
        {
            if (broken == true)
            {
                string brokenText = BrokenDialogue();
                text.Text = brokenText;
                text.IsActive = true;
            }
            else if (bGreeting == true && bDialogue == false)
            {
                int dialogue = random.Next(0, Dialogue.Count);
                text.Text = Dialogue[dialogue];
                text.IsActive = true;
                bDialogue = true;
            }
            else if (bDialogue == true && bFarewell == false)
            {
                int farewell = random.Next(0, Farewells.Count);
                text.Text = Farewells[farewell];
                text.IsActive = true;
                bFarewell = true;
            }
        }
        public virtual void LoadContent()
        {
            image.LoadContent();
            interactImage.LoadContent();
            OriginalPosition = new Vector2(image.Position.X, image.Position.Y);
            HitBox = new Rectangle((int)image.Position.X, (int)image.Position.Y, image.spriteSheetEffect.FrameWidth, image.spriteSheetEffect.FrameHeight);
            portrait.LoadContent();
            portrait.Position.X = ScreenManager.instance.Dimensions.X - portrait.texture.Width;
            portrait.Position.Y = 795 - portrait.texture.Height;
            portrait.Layer = .7f;
        }
        public virtual void UnloadContent()
        {
            image.UnloadContent();
            interactImage.UnloadContent();
        }
        public void UnloadText()
        {
            text.UnloadContent();
            name.UnloadContent();
        }
        public virtual void Update(GameTime gameTime, Player player)
        {
            image.Update(gameTime);
            HitBox = new Rectangle((int)image.Position.X, (int)image.Position.Y, image.spriteSheetEffect.FrameWidth, image.spriteSheetEffect.FrameHeight);
            if (player.facingHitBox.Intersects(HitBox))
            {
                interactable = true;
                interactImage.Position.X = image.Position.X + image.spriteSheetEffect.FrameWidth;
                interactImage.Position.Y = image.Position.Y - interactImage.texture.Height;
            }
            else
            {
                interactable = false;
            }
            if (Talking == true)
            {
                if (initiated == false)
                {
                    name = new Image();
                    name.color = new Color(0, 0, 0, 255);
                    name.Text = Name;
                    name.Position.X = 20;
                    name.Position.Y = 750;
                    name.Layer = .7f;
                    name.LoadContent();

                    int greeting = random.Next(0, Greetings.Count);
                    splitText = Greetings[greeting].Split(' ');
                    string brokenText = BrokenDialogue();
                    text = new Image();
                    text.IsActive = true;
                    text.Effects = "TextScrollEffect";
                    text.color = new Color(0, 0, 0, 255);
                    text.Text = brokenText;
                    text.Position.X = 20;
                    text.Position.Y = 830;
                    text.Layer = .7f;
                    text.LoadContent();
                    initiated = true;
                    bGreeting = true;
                }
                text.Update(gameTime);
            }
            else
            {
                text.IsActive = false;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            image.Draw(spriteBatch);
            if (interactable == true)
            {
                interactImage.Draw(spriteBatch);
            }
            if (Talking == true)
            {
                text.DrawString(spriteBatch);
                name.DrawString(spriteBatch);
                portrait.Draw(spriteBatch);
            }
        }
    }
}
