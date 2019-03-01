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
        public float costFavor = 0.0f;
        public KeyCode productionHotkey;
        public SkillType skillType = SkillType.Skill;
        public int givesLimit = 0;
        public int takesLimit = 0;
    }

    public enum SkillConditionType { None, TearCheck, OnlyOneAtAQueue, OnlyOne, GlobalUpgradeCheck, _LimitCheck };
    [System.Serializable]
    public class SkillCondition
    {
        public SkillCondition(SkillConditionType _type)
        {
            type = _type;
        }
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
        
        public enum UpgradeType { None, UpgradeTear, Food, Gold, Wood, Farm };
        public UpgradeType upgradeType = UpgradeType.None;

        public virtual bool Activate(GameObject sender)
        {
            if (upgradeType == UpgradeType.UpgradeTear)
            {
                BaseBehavior baseBehaviorComponent = sender.GetComponent<BaseBehavior>();
                baseBehaviorComponent.tear += 1;
                baseBehaviorComponent.UpdateTearDisplay();
            }
            if (upgradeType != UpgradeType.None)
            {
                CameraController cameraController = Camera.main.GetComponent<CameraController>();
                cameraController.AddUpgrade(cameraController.userId, upgradeType);
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
            
            object[] errorInfo = GetSkillErrors(
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
            string unitName, int minTear, int maxTear, GameObject skillSender = null, GameObject skillObject = null)
        {
            BaseBehavior baseBehaviorComponent = skillSender.GetComponent<BaseBehavior>();
            if (unitName != "")
            {
                foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Building"))
                {
                    BaseBehavior unitBehaviorComponent = unit.GetComponent<BaseBehavior>();
                    if (unitBehaviorComponent != null && unitBehaviorComponent.skillInfo.uniqueName == unitName && unitBehaviorComponent.ownerId == baseBehaviorComponent.ownerId)
                        if (unitBehaviorComponent.tear >= minTear && (unitBehaviorComponent.tear <= maxTear || maxTear == -1))
                            return true;
                }
            }
            else
            {
                if (baseBehaviorComponent.tear >= minTear && (baseBehaviorComponent.tear <= maxTear || maxTear == -1))
                    return true;
            }
            return false;
        }
        
        public static bool IsQueueContain(GameObject skillSender, string userId, string skillName)
        {
            BaseBehavior baseBehaviorComponent = skillSender.GetComponent<BaseBehavior>();
            if (baseBehaviorComponent != null && baseBehaviorComponent.ownerId == userId)
                return baseBehaviorComponent.IsQueueContain(skillName);
            return false;
        }

        public static bool IsAnyQueueContain(string userId, string skillName)
        {
            foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Building"))
            {
                BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
                if (baseBehaviorComponent != null && baseBehaviorComponent.ownerId == userId)
                    if (baseBehaviorComponent.IsQueueContain(skillName))
                        return true;
            }
            return false;
        }

        public static object[] GetSkillErrors(
            List<SkillCondition> condList, GameObject skillObject = null, int team = -1, string skillName = "", GameObject skillSender = null)
        {
            object[] errorInfo = new object[] {null, ""};

            SkillInfo skillInfo = null;
            if (skillObject.GetComponent<BaseBehavior>() != null)
                skillInfo = skillObject.GetComponent<BaseBehavior>().skillInfo;
            if (skillObject.GetComponent<BaseSkillScript>() != null)
                skillInfo = skillObject.GetComponent<BaseSkillScript>().skillInfo;
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            if (cameraController.limit + skillInfo.takesLimit > cameraController.maxLimit)
            {
                errorInfo[0] = new SkillCondition(SkillConditionType._LimitCheck);
                errorInfo[1] = "You do not have enough population limit!";
                return errorInfo;
            }
            foreach (SkillCondition cond in condList)
            {
                if(cond.type == SkillConditionType.TearCheck)
                {
                    bool hasUnitWithTear = false;
                    if (skillObject.GetComponent<BaseBehavior>() != null)
                        hasUnitWithTear = BaseBehavior.IsHasUnitWithTear(cond.name, cond.minValue, cond.maxValue, skillSender);
                    if (skillObject.GetComponent<BaseSkillScript>() != null)
                        hasUnitWithTear = BaseSkillScript.IsHasUnitWithTear(cond.name, cond.minValue, cond.maxValue, skillSender, skillObject);

                    if (!hasUnitWithTear)
                    {
                        errorInfo[0] = cond;
                        string[] TCErrors = new string[] { "first", "second", "third", "fourth" };
                        errorInfo[1] = String.Format("You need to have at least one {1} with {0} upgrade", TCErrors[cond.minValue], cond.readableName);
                        return errorInfo;
                    }
                }
                if (cond.type == SkillConditionType.GlobalUpgradeCheck)
                {
                    bool check = false;
                    //if (skillObject.GetComponent<BaseBehavior>() != null)
                    //    hasUnitWithTear = BaseBehavior.IsHasUnitWithTear(cond.name, cond.minValue, cond.maxValue, team, skillSender);
                    if (skillObject.GetComponent<BaseSkillScript>() != null)
                        check = CameraController.GlobalUpgradeCheck(
                            cond.name, cond.minValue, cond.maxValue, upgradeType: skillObject.GetComponent<BaseSkillScript>().upgradeType, ownerId: skillSender.GetComponent<BaseBehavior>().ownerId);

                    if (!check)
                    {
                        errorInfo[0] = cond;
                        errorInfo[1] = "You already has this upgrade";
                        return errorInfo;
                    }
                }
                
                if(cond.type == SkillConditionType.OnlyOneAtAQueue && IsQueueContain(skillSender, skillSender.GetComponent<BaseBehavior>().ownerId, skillName))
                {
                    errorInfo[0] = cond;
                    errorInfo[1] = "This upgrade can be done in a single copy in one building";
                    return errorInfo;
                }
                if(cond.type == SkillConditionType.OnlyOne && IsAnyQueueContain(skillSender.GetComponent<BaseBehavior>().ownerId, skillName))
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
