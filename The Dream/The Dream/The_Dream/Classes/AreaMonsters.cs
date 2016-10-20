using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace The_Dream.Classes
{
    public class AreaMonsters
    {
        public float SpawnTimer;
        public List<Monster> SpawnedMonsters, SpawnableMonsters;
        public List<Monster> AliveMonsters;
        public List<int> DeadMonsters;
        Random random;
        int WhichMonster, RandomX, RandomY;
        public int MaxMonsters, AreaX, AreaY;
        public bool MonsterAdded;
        public AreaMonsters()
        {
            SpawnedMonsters = new List<Monster>();
            AliveMonsters = new List<Monster>();
            SpawnableMonsters = new List<Monster>();
            DeadMonsters = new List<int>();
            random = new Random();
        }
        public void SpawnMonster(Monster monster)
        {
            //RandomX = random.Next(map.DeadZone.Left, map.DeadZone.Right - monster.image.texture.Width);
            //RandomY = random.Next(map.DeadZone.Top, map.DeadZone.Bottom - monster.image.texture.Height);
            RandomX = random.Next(0, 1000);
            RandomY = random.Next(0, 1000);
            Point temp = new Point(RandomX, RandomY);
            Point temp2 = new Point(RandomX + monster.image.texture.Width, RandomY + monster.image.texture.Height);
            //foreach (MapSprite blank in map.Blanks)
            //{
            //    if (blank.HitBox.Contains(temp) || blank.HitBox.Contains(temp2))
            //    {
            //        SpawnMonster(monster);
            //    }
            //}
            //foreach (Sprite sprite in textures.Sprites)
            //{
            //    if (sprite.HitBox.Contains(temp) || sprite.HitBox.Contains(temp2))
            //    {
            //        SpawnMonster(monster);
            //    }
            //}
            Type tempType = monster.GetType();
            Monster tempMonster = new Monster();
            tempMonster = (Monster)Activator.CreateInstance(monster.GetType());
            tempMonster.LoadContent();
            tempMonster.AreaX = AreaX;
            tempMonster.AreaY = AreaY;
            tempMonster.X = RandomX;
            tempMonster.Y = RandomY;
            tempMonster.Hitbox = new Rectangle(RandomX, RandomY, monster.image.spriteSheetEffect.FrameWidth, monster.image.spriteSheetEffect.FrameHeight);
            SpawnedMonsters.Add(tempMonster);
            MonsterAdded = true;
        }
        public void DespawnMonster(Monster monster, Player player)
        {
            player.EXP += monster.EXP;
            monster.IsAlive = false;
            DeadMonsters.Add(SpawnedMonsters.IndexOf(monster));
        }
        public void LoadContent()
        {

        }
        public void UnloadContent()
        {

        }
        public void Update(GameTime gameTime, Player player)
        {
            SpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (Monster m in SpawnedMonsters)
            {
                if (m.Health <= 0)
                {
                    DespawnMonster(m, player);
                }
            }
            AliveMonsters = new List<Monster>();
            foreach (Monster monster in SpawnedMonsters)
            {
                if (monster.IsAlive == true)
                {
                    AliveMonsters.Add(monster);
                }
            }
            SpawnedMonsters = new List<Monster>(AliveMonsters);
            if (SpawnTimer >= 1)
            {
                WhichMonster = random.Next(0, SpawnableMonsters.Count);
                if (SpawnableMonsters.Count > 0 && SpawnedMonsters.Count < MaxMonsters)
                {
                    SpawnMonster(SpawnableMonsters[WhichMonster]);
                }
                SpawnTimer = 0;
            }
        }
    }
}
