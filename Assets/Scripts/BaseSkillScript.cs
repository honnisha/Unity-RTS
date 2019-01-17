using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GangaGame
{
    public enum SkillType { None, Skill, Upgrade };

    [System.Serializable]
    public class SkillInfo
    {
        public string uniqueName;
        public string readableName;
        public string readableDescription;
        public string imagePath;
        public float timeToBuild = 0.0f;
        public float costFood = 0.0f;
        public float costGold = 0.0f;
        public float costWood = 0.0f;
        public KeyCode productionHotkey;
        public SkillType skillType = SkillType.Skill;
    }

    public enum SkillConditionType { None, TownCenterTear1, TownCenterTear2 };
    interface ISkillInterface
    {
        SkillInfo skillInfo { get; set; }

        SkillConditionType skillConditions { get; set; }

        List<string> GetCostInformation();
        List<string> GetStatistics();
        string ErrorMessage(GameObject sender);
        bool IsDisplayedAsSkill(GameObject sender);
        bool IsCanBeUsedAsSkill(GameObject sender);
    }

    public class BaseSkillScript : MonoBehaviour, ISkillInterface
    {
        public SkillInfo _skillInfo;
        public SkillInfo skillInfo { get { return _skillInfo; } set { _skillInfo = value; } }
        public SkillConditionType _skillConditions;
        public SkillConditionType skillConditions { get { return _skillConditions; } set { _skillConditions = value; } }

        public virtual bool Activate(GameObject sender) { return true; }
        public virtual bool IsDisplayedAsSkill(GameObject sender) { return true; }
        public virtual bool IsCanBeUsedAsSkill(GameObject sender) { return true; }
        public virtual string ErrorMessage(GameObject sender) { return ""; }

        public List<string> GetCostInformation()
        {
            List<string> statistics = new List<string>();
            if (skillInfo.timeToBuild > 0.0f)
                statistics.Add(String.Format("Time to create: {0:F0} sec", skillInfo.timeToBuild));
            if (skillInfo.costFood > 0)
                statistics.Add(String.Format("Food: {0:F0}", skillInfo.costFood));
            if (skillInfo.costGold > 0)
                statistics.Add(String.Format("Gold: {0:F0}", skillInfo.costGold));
            if (skillInfo.costWood > 0)
                statistics.Add(String.Format("Wood: {0:F0}", skillInfo.costWood));
            return statistics;
        }

        public List<string> GetStatistics()
        {
            List<string> statistics = new List<string>();
            // statistics.Add(String.Format("Stabbing resist: {0:F0}%", stabbingResist));
            return statistics;
        }
    }
}