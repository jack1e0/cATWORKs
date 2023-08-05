using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UserData
{
    public string username;
    public string userId;

    public int growth; // 0 to 2

    public int catfoodCount;
    public int level;
    public int currXP;
    public int maxXP;
    public int currHappiness;
    public DateTime prevExitTime;

    public int alarmId;
    public Dictionary<int, List<int>> alarmDict;

    public string equippedAccessory;

    // contains a mapping of unlocked accessories, and  0 / 1  to show whether they are bought or not.
    public Dictionary<string, int> unlockedAccessoryDict;

    public bool firstTime;

    public UserData(string name, string userId)
    {
        username = name;
        this.userId = userId;
        this.growth = 0;
        catfoodCount = 0;
        level = 1;
        currXP = 0;
        maxXP = 2;
        currHappiness = 0;
        equippedAccessory = string.Empty;
        unlockedAccessoryDict = new Dictionary<string, int> {
            {"CAP", 0},
            {"CLIP", 1}
        };
        firstTime = true;
    }

}
