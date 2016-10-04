using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dream.Classes
{
    public class UpdateMonsters
    {
        Map map;
        Textures textures;
        public Dictionary<string, Monster> MonsterList;
        public List<Monster> SpawnedMonsters, SpawnableMonsters, AliveMonsters;
        public string SpawnableMonsterKeys;
        public Monsters.TestMonster testMonster;
        public Monsters.TestMonster2 testMonster2;
        public bool Entered;
        public bool MonsterAdded;
        int WhichMonster, RandomX, RandomY, MaxMonsters;
        float SpawnTimer;
        Random random;
        List<Vector2> AreasAdded;
        bool AreaAdded;
        public void GetMap(Map realMap)
        {
            map = realMap;
        }
        void SetMonster<T>(ref T monster)
        {
            if (monster == null)
            {
                monster = (T)Activator.CreateInstance(typeof(T));
            }
            MonsterList.Add(monster.GetType().ToString().Replace("The_Dream.Classes.Monsters.", ""), (monster as Monster));
        }
        public void CanMonsterSpawn(string monster)
        {
            if (MonsterList.ContainsKey(monster))
            {
                MonsterList[monster].CanSpawn = true;
            }
            else
            {
                MonsterList[monster].CanSpawn = false;
            }
        }
        public void SpawnMonster(Monster monster, int AreaX, int AreaY)
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
        public void DespawnMonster(Monster monster)
        {
            //map.player.EXP += monster.EXP;
            monster.IsAlive = false;
            monster.UnloadContent();
        }
        public UpdateMonsters()
        {
            MonsterList = new Dictionary<string, Monster>();
            SpawnedMonsters = new List<Monster>();
            SpawnableMonsters = new List<Monster>();
            AliveMonsters = new List<Monster>();
            Entered = true;
            SpawnTimer = 0f;
            random = new Random();
            MonsterAdded = false;
            AreasAdded = new List<Vector2>();
            MaxMonsters = 0;
            AreaAdded = false;
        }
        public void LoadContent()
        {
            SetMonster<Monsters.TestMonster>(ref testMonster);
            SetMonster<Monsters.TestMonster2>(ref testMonster2);
            foreach (var monster in MonsterList)
            {
                monster.Value.LoadContent();
            }
        }
        public void UnloadContent()
        {
            foreach (Monster monster in SpawnedMonsters)
            {
                monster.UnloadContent();
            }
        }
        public void Update(GameTime gameTime, int AreaX, int AreaY)
        {
            SpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //if (map.NewMap == true)
            //{
            //    SpawnTimer = 0;
            //    Entered = true;
            //    foreach (Monster monster in SpawnedMonsters)
            //    {
            //        monster.UnloadContent();
            //    }
            //    foreach (var monster in MonsterList)
            //    {
            //        monster.Value.CanSpawn = false;
            //    }
            //}
            if (Entered == true)
            {
                SpawnableMonsters.Clear();
                foreach (var monster in MonsterList)
                {
                    if (monster.Value.XSpawn == AreaX && monster.Value.YSpawn == AreaY)
                    {
                        monster.Value.CanSpawn = true;
                        SpawnableMonsters.Add(monster.Value);
                    }
                //    if (monster.Value.CanSpawn)
                //    {
                //        SpawnableMonsterKeys += monster.Key + ":";
                //    }
                //    if (SpawnableMonsterKeys == null)
                //    {

                //    }
                //    else if (SpawnableMonsterKeys != String.Empty)
                //    {
                //        SpawnableMonsterKeys.Remove(SpawnableMonsterKeys.Length - 1);
                //    }
                //}
                //if (SpawnableMonsterKeys == null)
                //{

                //}
                //else if (SpawnableMonsterKeys != String.Empty)
                //{
                //    string[] split = SpawnableMonsterKeys.Split(':');
                //    foreach (string item in split)
                //    {
                //        if (item != "")
                //        {
                //            CanMonsterSpawn(item);
                //        }
                //    }
                }
                Vector2 Area = new Vector2(AreaX, AreaY);
                if (SpawnableMonsters.Count > 0)
                {
                    if (AreasAdded.Count == 0)
                    {
                        AreasAdded.Add(Area);
                        MaxMonsters += 5;
                    }
                    foreach (Vector2 v in AreasAdded)
                    {
                        if (v == Area)
                        {
                            AreaAdded = true;
                        }
                    }
                    if (AreaAdded == false)
                    {
                        AreasAdded.Add(Area);
                        MaxMonsters += 5;
                    }
                }
                AreaAdded = false;
                Entered = false;
            }
            //if (map.player.Attacking == true)
            //{
            //    foreach (Monster monster in SpawnedMonsters)
            //    {
            //        if (map.player.AttackHitboxes[map.player.Direction].Intersects(monster.Hitbox))
            //        {
            //            monster.Health -= map.player.Strength;
            //        }
            //    }
            //}
            foreach (Monster monster in SpawnedMonsters)
            {
                if (monster.Health <= 0)
                {
                    DespawnMonster(monster);
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
                    SpawnMonster(SpawnableMonsters[WhichMonster], AreaX, AreaY);
                }
                SpawnTimer = 0;
            }
        }
    }
}
