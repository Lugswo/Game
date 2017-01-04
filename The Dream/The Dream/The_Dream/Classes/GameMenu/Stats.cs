using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes.GameMenu
{
    public class Stats : MenuTab
    {
        public Image stats;
        public Stats()
        {
            image.Path = "Gameplay/GUI/Menu/Stats";
            inImage.Path = "Gameplay/GUI/Menu/InStats";
            stats = new Image();
        }
        public override void LoadContent(Player player)
        {
            base.LoadContent(player);
            stats.Text = "Health: " + player.maxHealth + "\nAttack: " + player.Strength + "\nDefense: " + player.Defense;
            stats.LoadContent();
            stats.color = Color.Black;
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime, Player player, bool inMenu)
        {
            base.Update(gameTime, player, inMenu);
        }
        public override void Draw(SpriteBatch spriteBatch, bool inMenu)
        {
            base.Draw(spriteBatch, inMenu);
            stats.DrawString(spriteBatch);
        }
    }
}
