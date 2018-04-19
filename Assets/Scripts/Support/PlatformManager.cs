using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : Singleton<PlatformManager> {

    protected PlatformManager() { }

    Dictionary<RuntimePlatform, GamePlatform> gamePlatforms;
    Dictionary<RuntimePlatform, GamePlatform> GamePlatforms {
        get {
            if (gamePlatforms==null) {
                gamePlatforms = new Dictionary<RuntimePlatform, GamePlatform>() {
                    {RuntimePlatform.WindowsPlayer,  new Windows() },
                    {RuntimePlatform.WindowsEditor,  new Windows() },
                    {RuntimePlatform.LinuxEditor,  new LinuxPlatform() },
                    { RuntimePlatform.LinuxPlayer,  new LinuxPlatform() },                    
                    {RuntimePlatform.PS4,  new PS4() },
                    {RuntimePlatform.Switch,  new Switch() },
                    {RuntimePlatform.XboxOne,  new XboxOne() },
                };
            }
            return gamePlatforms;
        }
    }
    
    public GamePlatform ActivePlatform {
        get {
            if (GamePlatforms.ContainsKey(Application.platform)) {
                return GamePlatforms[Application.platform];
            }
            //Debug.LogWarning(string.Format("UNEXPECTED GAME PLATFORM -- {0} -- DETECTED. DEFAULTING TO WINDOWS", Application.platform));
            return GamePlatforms[RuntimePlatform.WindowsPlayer];
        }
    }

    protected override void Start() {
        base.Start();
        GamePlatform plat = ActivePlatform;//test it finds expected platform
    }

}

public interface IPlatform {
    void EarnAchievement(Achievement achievement);
    void ProgressAchievement(Achievement achievement);
    void UpdateAchievements(List<Achievement> achievements);
}

public abstract class GamePlatform : IPlatform {
    public abstract void EarnAchievement(Achievement achievement);
    public abstract void ProgressAchievement(Achievement achievement);
    public abstract void UpdateAchievements(List<Achievement> achievements);
}

public class LinuxPlatform : GamePlatform {
    public override void EarnAchievement(Achievement achievement) { }
    public override void ProgressAchievement(Achievement achievement) { }
    public override void UpdateAchievements(List<Achievement> achievements) { }
}

public class PS4 : GamePlatform {
    public override void EarnAchievement(Achievement achievement) { }
    public override void ProgressAchievement(Achievement achievement) { }
    public override void UpdateAchievements(List<Achievement> achievements) { }
}

public class XboxOne : GamePlatform {
    public override void EarnAchievement(Achievement achievement) { }
    public override void ProgressAchievement(Achievement achievement) { }
    public override void UpdateAchievements(List<Achievement> achievements) { }
}

public class Switch : GamePlatform {
    public override void EarnAchievement(Achievement achievement) { }
    public override void ProgressAchievement(Achievement achievement) { }
    public override void UpdateAchievements(List<Achievement> achievements) { }
}

public class Windows : GamePlatform {
    public override void EarnAchievement(Achievement achievement) { }
    public override void ProgressAchievement(Achievement achievement) { }
    public override void UpdateAchievements(List<Achievement> achievements) { }
}