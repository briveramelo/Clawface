public class Strings  {

    public static string ANIMATIONSTATE = "AnimationState";

    public static class Tags {
        public static string MOD = "Mod";
        public static string ENEMY = "Enemy";
        public static string PLAYER = "Player";
        public static string CODEXENTRY = "CodexEntry";
        public static string UNLOCKABLE = "Unlockable";
        public static string UNTAGGED = "Untagged";
        public static string PLAYERDETECTOR = "PlayerDetector";
        public static string PROJECTILE = "Projectile";
        public static string WALL = "Wall";
        public static string FLOOR = "Floor";
    }

    public static class Scenes {
        public static string MainMenu = "Scenes/Build Scenes/MainMenu";
        //public static string Arena = "Scenes/Build Scenes/Gucci_V1.2";
        public static string Arena = "Scenes/Build Scenes/Arena 1";
    }

    public static class Layers
    {
        public static string GROUND = "Ground";

        public static string ENEMY = "Enemy";
    }

    public static class MenuStrings
    {
        public static string MAIN = "MainMenu";
        public static string CREDITS = "CreditsMenu";
        public static string LOAD = "LoadMenu";
        public static string PAUSE = "PauseMenu";
        public static string TUTORIAL = "TutorialMenu";
        public static string STAGE_OVER = "StageOverMenu";
        public static string SETTINGS = "Settings";
        public static string LEVEL_SELECT = "LevelSelect";
        public static string WEAPON_SELECT = "WeaponSelect";

        public static string GAME_OVER_TEXT = "Game Over";
        public static string STAGE_OVER_TEXT = "Stage Over";
    }

    public static class Input
    {
        public static class Axes
        {
            public const string MOVEMENT = "Move";
            public const string LOOK = "Look";
        }

        public static class Actions
        {
            public const string FIRE_RIGHT = "Fire Right";
            public const string FIRE_LEFT = "Fire Left";
            public const string PAUSE = "Pause";
            public const string EAT = "Eat";
            public const string DODGE = "Dodge";
        }

        public static class UI
        {
            public const string HORIZONTAL = "UIHorizontal";
            public const string VERTICAL = "UIVertical";
            public const string SUBMIT = "UISubmit";
            public const string CANCEL = "UICancel";
        }
    }

    public static class Events
    {
        public const string FACE_OPEN = "FaceOpen";
        public const string ARM_EXTENDED = "ArmExtended";
        public const string ARM_ANIMATION_COMPLETE = "ArmAnimationComplete";
        public const string UPDATE_HEALTH = "UpdateHealth";

        public const string KILL_ENEMY = "KillEnemy";
        public const string BEAT_LEVEL1 = "BeatLevel1";
        public const string SKIN_ENEMY = "SkinEnemy";

        public const string EARN_ACHIEVEMENT = "EarnAchievement";
        public const string PROGRESS_ACHIEVEMENT = "ProgressAchievement";
        public const string UPDATE_ACHIEVEMENTS = "UpdateAchievements";
        
        public const string LOCK_SPAWNERS = "LockSpawners";
        public const string CALL_NEXTWAVEENEMIES = "CallNextWaveEnemies";
		
        public const string UNLOCK_WEAPON = "UnlockWeapon";
        public const string UNLOCK_NEXT_LEVEL = "UnlockNextLevel";
        public const string SET_LEVEL_SCORE = "SetLevelScore";

        public const string SCORE_UPDATED = "ScoreUpdated";
        public const string COMBO_UPDATED = "ComboUpdated";
        public const string COMBO_TIMER_UPDATED = "ComboTimerUpdated";
        public const string PLAYER_HEALTH_MODIFIED = "PlayerHealthModified";


        public const string LEVEL_STARTED = "LevelStarted";
        public const string LEVEL_FAILED = "LevelFailed";
        public const string LEVEL_QUIT = "LevelQuit";
        public const string LEVEL_COMPLETED = "LevelCompleted";
        public const string LEVEL_RESTARTED = "LevelRestarted";
        public const string EXIT_GAME = "ExitGame";

        public const string PLAYER_DAMAGED = "PlayerDamaged";
        public const string PLAYER_KILLED = "PlayerKilled";
    }

    public class AchievementNames {
        public const string Kill100 = "Blood Bath";
        public const string BeatLevel1 = "Conquest";
        public const string Skin20Enemies = "Skinner";        
    }

    public class PlayerPrefStrings
    {
        public const string BLASTER_ENABLED = "BlasterEnabled";
        public const string LIGHTNING_GUN_ENABLED = "LightningGunEnabled";
        public const string SPREAD_GUN_ENABLED = "SpreadGunEnabled";
        public const string GEYSER_GUN_ENABLED = "GeyserGunEnabled";
        public const string DICE_GUN_ENABLED = "DiceGunEnabled";
        public const string BOOMERANG_ENABLED = "BoomerangEnabled";
        public const string LATEST_UNLOCKED_LEVEL = "LatestUnlockedLevel";
    }
}
