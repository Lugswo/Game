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
        public Dictionary<int, Monster> MonsterList;
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
        public List<AreaMonsters> AreaList;
        bool AreaAdded;
        public void GetMap(Map realMap)
        {
            map = realMap;
        }
        void SetMonster<T>(ref T monster, int ID)
        {
            if (monster == null)
            {
                monster = (T)Activator.CreateInstance(typeof(T));
            }
            MonsterList.Add(ID, (monster as Monster));
        }
        public void NewArea(int AreaX, int AreaY, Map map)
        {
            Vector2 Area = new Vector2(AreaX, AreaY);
            if (AreasAdded.Count == 0)
            {
                AreasAdded.Add(Area);
                AreaMonsters temp = new AreaMonsters();
                temp.AreaX = AreaX;
                temp.AreaY = AreaY;
                temp.LoadContent(map);
                AreaList.Add(temp);
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
                AreaMonsters temp = new AreaMonsters();
                temp.AreaX = AreaX;
                temp.AreaY = AreaY;
                temp.LoadContent(map);
                AreaList.Add(temp);
            }
            foreach (AreaMonsters area in AreaList)
            {
                if (area.AreaX != AreaX || area.AreaY != AreaY)
                {
                    continue;
                }
                area.SpawnableMonsters.Clear();
                foreach (var monster in MonsterList)
                {
                    if (monster.Value.XSpawn == AreaX && monster.Value.YSpawn == AreaY)
                    {
                        monster.Value.CanSpawn = true;
                        area.SpawnableMonsters.Add(monster.Value);
                    }
                }
                AreaAdded = false;
                Entered = false;
                break;
            }
        }
        public UpdateMonsters()
        {
            MonsterList = new Dictionary<int, Monster>();
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
            AreaList = new List<AreaMonsters>();
            testMonster = new Monsters.TestMonster();
            testMonster2 = new Monsters.TestMonster2();
        }
        public void LoadContent()
        {
            SetMonster<Monsters.TestMonster>(ref testMonster, testMonster.MonsterID);
            SetMonster<Monsters.TestMonster2>(ref testMonster2, testMonster2.MonsterID);
            foreach (var monster in MonsterList)
            {
                monster.Value.LoadContent();
            }
        }
        public void UnloadContent()
        {
            foreach (AreaMonsters area in AreaList)
            {
                foreach (Monster m in area.SpawnedMonsters)
                {
                    m.UnloadContent();
                }
            }
        }
        public void Update(GameTime gameTime, Player player, int AreaX, int AreaY)
        {
            foreach (AreaMonsters area in AreaList)
            {
                area.Update(gameTime, player);
            }
        }
    }
}
