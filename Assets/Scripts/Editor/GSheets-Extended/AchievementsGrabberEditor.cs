using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AchievementsGrabber))]
public class AchievementsGrabberEditor : DataGrabberEditor<DataGrabber<Achievement, AchievementParser>, Achievement, AchievementScriptable, AchievementParser> {

	
}
