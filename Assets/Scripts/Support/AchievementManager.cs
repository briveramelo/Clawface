//Brandon Rivera-Melo
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : Singleton<AchievementManager> {

    protected AchievementManager() { }

    [SerializeField] AchievementScriptable OG_AchievementScriptable;
    AchievementScriptable achievements;
    public AchievementScriptable Achievements {
        get {
            if (achievements==null) {
                achievements = Instantiate(OG_AchievementScriptable);
            }
            return achievements;
        }
    }
    public List<Achievement> AchievementsList { get { return Achievements.dataList; } }

    private void OnEnable() {
        EventSystem.Instance.RegisterEvent(Strings.Events.KILL_ENEMY, KillEnemy);
        EventSystem.Instance.RegisterEvent(Strings.Events.SKIN_ENEMY, SkinEnemy);
    }

    private void OnDisable() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.KILL_ENEMY, KillEnemy);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.SKIN_ENEMY, SkinEnemy);
        }
    }


    void KillEnemy(params object[] parameters) {        
        TryEarnAchievement(Strings.AchievementNames.Kill100);
    }    

    void SkinEnemy(params object[] parameters) {
        TryEarnAchievement(Strings.AchievementNames.Skin20Enemies);        
    }    









    void TryEarnAchievement(string achievementName) {
        Achievement chieve = Achievements.Find(achievementName);
        if (!chieve.IsEarned) {
            chieve.count++;
            if (!chieve.IsEarned) {
                EventSystem.Instance.TriggerEvent(Strings.Events.PROGRESS_ACHIEVEMENT, chieve);
            }
            else {
                EventSystem.Instance.TriggerEvent(Strings.Events.EARN_ACHIEVEMENT, chieve);
            }
        }
    }


}
