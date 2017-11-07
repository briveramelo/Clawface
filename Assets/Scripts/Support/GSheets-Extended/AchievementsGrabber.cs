using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class AchievementsGrabber : DataGrabber<Achievement, AchievementParser> {


}
[System.Serializable]
public class Achievement : GSheetData {
    public string name;
    public string description;
    public int count;
    public int requiredCount;
    public bool IsEarned { get { return count >= requiredCount; } }
    public Achievement(string name, string description, int requiredCount) {
        this.name = name;
        this.description = description;
        this.requiredCount = requiredCount;
    }
}
[System.Serializable]
public class AchievementParser : GSheetsJSONParser<Achievement> {
    public AchievementParser() { }
    public override Achievement ParseValueElement(JSONArray array) {
        return new Achievement(array[0].Value, array[1].Value, array[2].AsInt);
    }
}
