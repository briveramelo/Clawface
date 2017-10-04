using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AchievementScriptable : GSheetScriptable<Achievement> {
    public Achievement Find(string name) {
        return dataList.Find(chieve =>chieve.name==name);
    }
}