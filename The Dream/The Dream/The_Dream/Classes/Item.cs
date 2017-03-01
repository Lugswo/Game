using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes
{
    public class Item : Sprite
    {
        public Image inventoryImage;
        public bool Dropped, pickedUp, itemAdded, selected;
        public int ItemID, dropChance, X, Y, numberOwned;
        public Subcategory category;
        public Image name, description, numberOwnedImage;
        public Image itemFrame;
        public enum Subcategory
        {
            ITEM,
            GEAR
        }
        public Item()
        {
            image = new Image();
            inventoryImage = new Image();
            Dropped = false;
            inventoryImage.Alpha = 0;
            image.Layer = .4f;
            itemAdded = false;
            itemFrame = new Image();
            selected = false;
            name = new Image();
            description = new Image();
            name.Position.X = 760;
            name.Position.Y = 746;
            description.Position.X = 760;
            description.Position.Y = 777;
            name.Layer = .94f;
            description.Layer = .94f;
            numberOwned = 1;
            numberOwnedImage = new Image();
            numberOwnedImage.Text = numberOwned.ToString() + "x";
            numberOwnedImage.Layer = .96f;
        }
        public void HorizontalMove(int mX)
        {
            image.Position.X = X - mX;
        }
        public void VerticalMove(int mY)
        {
            image.Position.Y = Y - mY;
        }
        public virtual void LoadContent()
        {
            image.LoadContent();
            inventoryImage.LoadContent();
            itemFrame.Path = "Gameplay/Items/ItemFrame";
            itemFrame.LoadContent();
            name.LoadContent();
            description.LoadContent();
            numberOwnedImage.LoadContent();
        }
        public virtual void UnloadContent()
        {
            image.UnloadContent();
            inventoryImage.UnloadContent();
        }
        public virtual void Update(GameTime gameTime, Player player)
        {
            HitBox = new Rectangle(X, Y, image.texture.Width, image.texture.Height);
            if (player.HitBox.Intersects(HitBox))
            {
                pickedUp = true;
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            image.Draw(spriteBatch);
        }
    }
}