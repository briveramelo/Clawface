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
    }

    public static class Scenes {
        //public static string Level1 = "Scenes/EAE_Level1.1";//Gucci_V1.2";
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
        public static string LOGO = "LogoMenu";
        public static string PAUSE = "PauseMenu";
        public static string FADE = "FadeMenu";
        public static string TUTORIAL = "TutorialMenu";
        public static string STAGE_OVER = "StageOverMenu";
        public static string SETTINGS = "Settings";
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
            public const string SKIN = "Skin";
            public const string DODGE = "Dodge";
        }
    }

    public static class Events
    {
        public const string FACE_OPEN = "FaceOpen";
        public const string ARM_EXTENDED = "ArmExtended";
        public const string UPDATE_HEALTH = "UpdateHealth";

        public const string KILL_ENEMY = "KillEnemy";
        public const string BEAT_LEVEL1 = "BeatLevel1";
        public const string SKIN_ENEMY = "SkinEnemy";

        public const string EARN_ACHIEVEMENT = "EarnAchievement";
        public const string PROGRESS_ACHIEVEMENT = "ProgressAchievement";
        public const string UPDATE_ACHIEVEMENTS = "UpdateAchievements";
    }

    public class AchievementNames {
        public const string Kill100 = "Blood Bath";
        public const string BeatLevel1 = "Conquest";
        public const string Skin20Enemies = "Skinner";        
    }
}
