using GangaGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TearUpgrade : BaseSkillScript
{
    public int maxTear = 2;

    public SkillInfo skillInfo
    {
        get { return _skillInfo; }
        set { _skillInfo = value; }
    }

    public override bool Activate(GameObject sender)
    {
        BaseBehavior senderBaseBehaviorComponent = sender.GetComponent<BaseBehavior>();
        senderBaseBehaviorComponent.tear += 1;
        senderBaseBehaviorComponent.UpdateTearDisplay();
        return true;
    }

    public override bool IsDisplayedAsSkill(GameObject sender)
    {
        BaseBehavior senderBaseBehaviorComponent = sender.GetComponent<BaseBehavior>();
        if (senderBaseBehaviorComponent.tear >= maxTear)
            return false;
        return true;
    }

    public override bool IsCanBeUsedAsSkill(GameObject sender)
    {
        BaseBehavior senderBaseBehaviorComponent = sender.GetComponent<BaseBehavior>();
        if (senderBaseBehaviorComponent.tear >= maxTear)
            return false;
        return true;
    }

    public override string ErrorMessage(GameObject sender)
    {
        return "";
    }
}
