using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes.Items
{
    public class TestItem : Item
    {
        public TestItem()
        {
            dropChance = 1000000;
            ItemID = 1;
            category = Subcategory.ITEM;
            Name = "TestItem";
        }
        public override void LoadContent()
        {
            image.Path = "Gameplay/Items/TestItem/Drop";
            inventoryImage.Path = "Gameplay/Items/TestItem/Inventory";
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
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
