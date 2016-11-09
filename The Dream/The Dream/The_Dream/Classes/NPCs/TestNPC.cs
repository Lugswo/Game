using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace The_Dream.Classes.NPCs
{
    public class TestNPC : NPC
    {
        public TestNPC()
        {

        }
        public override void LoadContent()
        {
            image.Path = "Gameplay/Characters/NPCs/TestNPC/Sprite";
            image.spriteSheetEffect.AmountOfFrames.X = 2;
            image.spriteSheetEffect.AmountOfFrames.Y = 1;
            portrait.Path = "Gameplay/Characters/NPCs/TestNPC/Portrait";
            Name = "Test";
            Greetings.Add("Hello.");
            Greetings.Add("Hi.");
            Dialogue.Add("I'm Chrom.");
            Dialogue.Add("I'm friends with Robin.");
            Farewells.Add("Bye.");
            Farewells.Add("Goodbye.");
            base.LoadContent();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime, Player player)
        {
            base.Update(gameTime, player);
        }
    }
}
