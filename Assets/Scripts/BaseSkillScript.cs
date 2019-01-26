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
        public SkillInfo(string _name, string _readName, string _imagePath, string _readDescr, KeyCode _hotkey)
        {
            imagePath = _imagePath;
            uniqueName = _name;
            readableName = _readName;
            readableDescription = _readDescr;
            productionHotkey = _hotkey;
        }

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

    public enum SkillConditionType { None, TearCheck, OnlyOneAtAQueue, OnlyOne,  };
    [System.Serializable]
    public class SkillCondition
    {
        public SkillConditionType type = SkillConditionType.None;
        public string name = "";
        public int minValue = 0;
        public int maxValue = 0;
        public string readableName = "";
        public bool notDisplayWhenFalse = false;
    }
    interface ISkillInterface
    {
        SkillInfo skillInfo { get; set; }

        List<SkillCondition> skillConditions { get; set; }

        List<string> GetCostInformation();
        List<string> GetStatistics();
    }

    public class SkillErrorInfo {
        public string errorMessage = "";
        public bool isDisplayedAsSkill = true;
        public bool isCanBeUsedAsSkill = true;
    }

    public class BaseSkillScript : MonoBehaviour, ISkillInterface
    {
        public SkillInfo _skillInfo;
        public SkillInfo skillInfo { get { return _skillInfo; } set { _skillInfo = value; } }
        public List<SkillCondition> _skillConditions;
        public List<SkillCondition> skillConditions { get { return _skillConditions; } set { _skillConditions = value; } }
        
        public enum SkillActionType { None, UpgradeTear, };
        public SkillActionType skillActionType = SkillActionType.None;

        public virtual bool Activate(GameObject sender)
        {
            if (skillActionType == SkillActionType.UpgradeTear)
            {
                BaseBehavior baseBehaviorComponent = sender.GetComponent<BaseBehavior>();
                baseBehaviorComponent.tear += 1;
                baseBehaviorComponent.UpdateTearDisplay();
            }
            return true;
        }

        public static SkillErrorInfo GetSkillErrorInfo(GameObject sender, GameObject skillObject)
        {
            SkillErrorInfo skillErrorInfo = new SkillErrorInfo();
            BaseBehavior baseBehaviorComponent = sender.GetComponent<BaseBehavior>();

            List<SkillCondition> skillConditions = null;
            SkillInfo skillInfo = null;
            BaseSkillScript baseSkillScript = skillObject.GetComponent<BaseSkillScript>();
            if (baseSkillScript != null)
            {
                skillConditions = baseSkillScript.skillConditions;
                skillInfo = baseSkillScript.skillInfo;
            }
            BaseBehavior senderBaseBehaviorComponent = skillObject.GetComponent<BaseBehavior>();
            if (senderBaseBehaviorComponent != null)
            {
                skillConditions = senderBaseBehaviorComponent.skillConditions;
                skillInfo = senderBaseBehaviorComponent.skillInfo;
            }
            
            object[] errorInfo = BaseSkillScript.GetSkillErrors(
                condList: skillConditions, team: baseBehaviorComponent.team,
                skillName: skillInfo.uniqueName, skillObject: skillObject, skillSender: sender);
            SkillCondition skillCondition = (SkillCondition)errorInfo[0];
            string errorMessage = (string)errorInfo[1];
            
            skillErrorInfo.errorMessage = errorMessage;

            if (errorMessage != "")
                skillErrorInfo.isCanBeUsedAsSkill = false;
            
            if (errorMessage != "" && skillCondition.notDisplayWhenFalse)
                skillErrorInfo.isDisplayedAsSkill = false;
            
            return skillErrorInfo;
        }

        public static bool IsHasUnitWithTear(
            string unitName, int minTear, int maxTear, int TCTeam, GameObject skillSender = null, GameObject skillObject = null)
        {
            BaseBehavior baseBehaviorComponent = skillSender.GetComponent<BaseBehavior>();
            if (baseBehaviorComponent.tear >= minTear && (baseBehaviorComponent.tear <= maxTear || maxTear == -1))
                return true;
            return false;
        }
        
        public static bool IsQueueContain(GameObject skillSender, int team, string skillName)
        {
            BaseBehavior baseBehaviorComponent = skillSender.GetComponent<BaseBehavior>();
            if (baseBehaviorComponent != null)
                return baseBehaviorComponent.IsQueueContain(skillName);
            return false;
        }

        public static bool IsAnyQueueContain(int team, string skillName)
        {
            foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Building"))
            {
                BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
                if (baseBehaviorComponent != null)
                    return baseBehaviorComponent.IsQueueContain(skillName);   
            }
            return false;
        }

        public static object[] GetSkillErrors(
            List<SkillCondition> condList, GameObject skillObject = null, int team = -1, string skillName = "", GameObject skillSender = null)
        {
            object[] errorInfo = new object[] {null, ""};
            foreach (SkillCondition cond in condList)
            {
                if(cond.type == SkillConditionType.TearCheck)
                {
                    bool hasUnitWithTear = false;
                    if (skillObject.GetComponent<BaseBehavior>() != null)
                        hasUnitWithTear = BaseBehavior.IsHasUnitWithTear(cond.name, cond.minValue, cond.maxValue, team, skillSender);
                    if (skillObject.GetComponent<BaseSkillScript>() != null)
                        hasUnitWithTear = BaseSkillScript.IsHasUnitWithTear(cond.name, cond.minValue, cond.maxValue, team, skillSender, skillObject);

                    if (!hasUnitWithTear)
                    {
                        string[] TCErrors = new string[] { "first", "second", "third", "fourth" };
                        errorInfo[0] = cond;
                        errorInfo[1] = String.Format("You need to have at least one {1} with {0} upgrade", TCErrors[cond.maxValue], cond.readableName);
                        return errorInfo;
                    }
                }
                
                if(cond.type == SkillConditionType.OnlyOneAtAQueue && IsQueueContain(skillSender, team, skillName))
                {
                    errorInfo[0] = cond;
                    errorInfo[1] = "This upgrade can be done in a single copy in one building";
                    return errorInfo;
                }
                if(cond.type == SkillConditionType.OnlyOne && IsAnyQueueContain(team, skillName))
                {
                    errorInfo[0] = cond;
                    errorInfo[1] = "This upgrade can be done in a single copy";
                    return errorInfo;
                }
            }
            return errorInfo;
        }

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
