using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData {
    public string username;
    public string email;

    public int catfoodCount;
    public int level;
    public int currXP;
    public int currHappiness;
    public long prevExitTime;

    public Dictionary<int, List<int>> alarmDict;

    public UserData(string name, string email) {
        username = name;
        this.email = email;
    }

}
