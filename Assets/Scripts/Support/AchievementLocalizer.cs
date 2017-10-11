//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementLocalizer : MonoBehaviour{

    public GamePlatform ActivePlatform { get { return PlatformManager.Instance.ActivePlatform; } }

    void OnEnable() {
        EventSystem.Instance.RegisterEvent(Strings.Events.EARN_ACHIEVEMENT, OnEarnAchievement);
        EventSystem.Instance.RegisterEvent(Strings.Events.PROGRESS_ACHIEVEMENT, OnProgressAchievement);
        EventSystem.Instance.RegisterEvent(Strings.Events.UPDATE_ACHIEVEMENTS, OnUpdateAchievements);
    }

    void OnDisable() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.EARN_ACHIEVEMENT, OnEarnAchievement);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PROGRESS_ACHIEVEMENT, OnProgressAchievement);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.UPDATE_ACHIEVEMENTS, OnUpdateAchievements);
        }
    }

    void OnEarnAchievement(params object[] parameters) {
        Achievement achievement = parameters[0] as Achievement;
        ActivePlatform.EarnAchievement(achievement);
    }

    void OnProgressAchievement(params object[] parameters) {
        Achievement achievement = parameters[0] as Achievement;
        ActivePlatform.ProgressAchievement(achievement);
    }

    void OnUpdateAchievements(params object[] parameters) {
        List<Achievement> achievements = parameters[0] as List<Achievement>;
        ActivePlatform.UpdateAchievements(achievements);
    }

}