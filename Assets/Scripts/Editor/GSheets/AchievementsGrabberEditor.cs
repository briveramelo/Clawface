using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AchievementsGrabber)), CanEditMultipleObjects]
public class AchievementsGrabberEditor : GrabberEditor<AchievementsGrabber, Achievement, AchievementScriptable, AchievementParser> {

	
}
//T GSheet Data
//V GSHeet Scriptable
//U GSheetsJSONParser
//X Grabber<T,V,U>