using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseSkillScript
{
    [Header("Skill UI settings")]
    public string uniqueName;
    public KeyCode hotkey;

    // Specify object of skill:
    public GameObject skillObject;

    // or:
    public string imagePath;
    public string readableName;
    public string readableDescription;
    public float costFood = 0.0f;
    public float costGold = 0.0f;
    public float costWood = 0.0f;

    public List<string> GetCostInformation()
    {
        List<string> statistics = new List<string>();
        if (costFood > 0)
            statistics.Add(String.Format("Food: {0:F0}", costFood));
        if (costGold > 0)
            statistics.Add(String.Format("Gold: {0:F0}", costGold));
        if (costWood > 0)
            statistics.Add(String.Format("Wood: {0:F0}", costWood));
        return statistics;
    }

    public List<string> GetStatistics()
    {
        List<string> statistics = new List<string>();
        statistics.Add("Cool skill, yo!");
        return statistics;
    }
}
