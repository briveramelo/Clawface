using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class AchievementsGrabber : Grabber<Achievement, AchievementParser> {

	
}
[System.Serializable]
public class Achievement : GSheetData{
    public string name;
    public string description;
    public int count;
    public Achievement(string name, string description, int count) {
        this.name = name;
        this.description = description;
        this.count = count;
    }
}
[System.Serializable]
public class AchievementParser : GSheetsJSONParser<Achievement> {
    public AchievementParser() { }
    public override Achievement ParseValueElement(JSONArray array) {
        return new Achievement(array[0].Value, array[1].Value, array[2].AsInt);
    }
}