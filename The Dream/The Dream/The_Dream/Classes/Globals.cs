using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace The_Dream.Classes
{
    public class Globals
    {
        public Dictionary<int, Items.Item> dItems;
        public Items.TestItem testItem;

        public Dictionary<int, Skills.Skill> dSkills;
        public Skills.TestSkill testSkill;
        public void SetItem<T>(ref T item, int ID)
        {
            if (item == null)
            {
                item = (T)Activator.CreateInstance(typeof(T));
            }
            dItems.Add(ID, (item as Items.Item));
        }
        public void SetSkill<T>(ref T skill, int ID)
        {
            skill = (T)Activator.CreateInstance(typeof(T));
            dSkills.Add(ID, (skill as Skills.Skill));
        }
        public void Save(Player player)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Player));
            StreamWriter writer = new StreamWriter("Load/Gameplay/Savefile.xml");
            ser.Serialize(writer, player);
            writer.Close();
        }
        public Globals()
        {
            dItems = new Dictionary<int, Items.Item>();
            testItem = new Items.TestItem();

            dSkills = new Dictionary<int, Skills.Skill>();
            testSkill = new Skills.TestSkill();
        }
        public void LoadContent()
        {
            SetItem<Items.TestItem>(ref testItem, testItem.ItemID);

            SetSkill<Skills.TestSkill>(ref testSkill, testSkill.SkillID);
        }
    }
}
