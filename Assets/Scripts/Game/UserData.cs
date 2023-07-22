using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData {
    public string username;
    public string userId;

    public int catfoodCount;
    public int level;
    public int currXP;
    public int maxXP;
    public int currHappiness;
    public long prevExitTime;

    public int alarmId;
    public Dictionary<int, List<int>> alarmDict;

    public UserData(string name, string userId) {
        username = name;
        this.userId = userId;

        catfoodCount = 0;
        level = 1;
        currXP = 0;
        maxXP = 5;
        currHappiness = 0;

    }

}
