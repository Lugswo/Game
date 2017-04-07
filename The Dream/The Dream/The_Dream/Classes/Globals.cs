using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace The_Dream.Classes
{
    public class Globals
    {
        public Dictionary<int, Skills.Skill> dSkills;
        public Skills.TestSkill testSkill;
        public void SetSkill<T>(ref T skill, int ID)
        {
            skill = (T)Activator.CreateInstance(typeof(T));
            dSkills.Add(ID, (skill as Skills.Skill));
        }
        public Globals()
        {
            dSkills = new Dictionary<int, Skills.Skill>();
            testSkill = new Skills.TestSkill();
        }
        public void LoadContent()
        {
            SetSkill<Skills.TestSkill>(ref testSkill, testSkill.SkillID);
        }
    }
}
