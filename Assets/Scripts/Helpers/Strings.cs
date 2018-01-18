public class Strings
{

    public const string ANIMATIONSTATE = "AnimationState";
    public const string FEETSTATE = "FeetState";

    public class Paths
    {
        public static string PLAYER_PREFAB_NAME = "Keira_GroupV";
        public static string PLAYER_PREFAB_RESOURCES_PATH = "Player/";

        public static string PLAYER_UI_PREFAB_NAME = "PlayerHUDV";
        public static string PLAYER_UI_PREFAB_RESOURCES_PATH = "PlayerUI/";
    }

    public class Tags
    {
        public const string MOD = "Mod";
        public const string ENEMY = "Enemy";
        public const string PLAYER = "Player";
        public const string CODEXENTRY = "CodexEntry";
        public const string UNLOCKABLE = "Unlockable";
        public const string UNTAGGED = "Untagged";
        public const string PLAYERDETECTOR = "PlayerDetector";
        public const string PROJECTILE = "Projectile";
        public const string WALL = "Wall";
        public const string FLOOR = "Floor";
    }

    public class Scenes
    {
        public const string MainMenu = "Scenes/Build Scenes/MainMenu";

        //public const string Arena = "Scenes/Build Scenes/Gucci_V1.2";
        public const string Arena = "Scenes/Build Scenes/80s shit";

        public const string Editor = "Scenes/Build Scenes/Editor";
    }

    public class Layers
    {
        public const string GROUND = "Ground";
        public const string MODMAN = "ModMan";
        public const string OBSTACLE = "Obstacle";
        public const string ENEMY = "Enemy";
        public const string ENEMY_BODY = "EnemyBody";
        public const string ENEMY_PROJECTILE = "EnemyProjectile";
    }

    public class TextStrings
    {
        public const string GAME_OVER_TEXT = "Game Over";
        public const string STAGE_OVER_TEXT = "Stage Over";

        public static readonly string[] FLAVOR_TEXT = 
        {
            "EAT. EVERYTHING.",
            "KILL. EVERYTHING.",
            "EXECUTE",
            "GET READY TO DESTROY",
            "KILL THEM ALL",
            "SEEK AND DESTROY",
            "OH HECK NO",
            "NO MERCY",
            "DEATH HAS ARRIVED",
            "PREPARE FOR KILLING",
            "ACTIVATE DESTROY.EXE",
            "END ALL LIFE",
            "EAT THEM ALIVE",
            "ACTIVATE CLAWFACE.EXE",
            "RELEASE THE RAGE",
            "RELEASE CLAWFACE",


        };
    }

    public class MenuStrings
    {
        public const string MAIN = "MainMenu";
        public const string CREDITS = "CreditsMenu";
        public const string LOAD = "LoadMenu";
        public const string PAUSE = "PauseMenu";
        public const string TUTORIAL = "TutorialMenu";
        public const string STAGE_OVER = "StageOverMenu";
        public const string CONTROLS = "Settings";
        public const string LEVEL_SELECT = "LevelSelect";
        public const string WEAPON_SELECT = "WeaponSelect";

        //levelEditor
        public const string ADD_PROPS_PLE = "AddPropsMenu";
        public const string ADD_SPAWNS_PLE = "AddSpawnsMenu";
        public const string SET_DYNLEVEL_PLE = "SetDynamicLevelMenu";
        public const string MAIN_PLE_MENU = "MainPLEMenu";
        public const string INIT_PLE_MENU = "InitPLEMenu";

    }

    public class Input
    {
        public class Axes
        {
            public const string MOVEMENT = "Move";
            public const string LOOK = "Look";
        }

        public class Actions
        {
            public const string FIRE_RIGHT = "Fire Right";
            public const string FIRE_LEFT = "Fire Left";
            public const string PAUSE = "Pause";
            public const string EAT = "Eat";
            public const string DODGE = "Dodge";
        }

        public class UI
        {
            public const string NAVIGATION = "UINavigation";
            public const string SUBMIT = "UISubmit";
            public const string CANCEL = "UICancel";
        }
    }

    public class Events
    {
        public const string FACE_OPEN = "FaceOpen";
        public const string CAPTURE_ENEMY = "CaptureEnemy";
        public const string ARM_ANIMATION_COMPLETE = "ArmAnimationComplete";
        public const string UPDATE_HEALTH = "UpdateHealth";

        public const string KILL_ENEMY = "KillEnemy";
        public const string BEAT_LEVEL1 = "BeatLevel1";
        public const string SKIN_ENEMY = "SkinEnemy";
        public const string DEATH_ENEMY = "DeathEnemy";
        public const string EAT_ENEMY = "EatEnemy";

        public const string EARN_ACHIEVEMENT = "EarnAchievement";
        public const string PROGRESS_ACHIEVEMENT = "ProgressAchievement";
        public const string UPDATE_ACHIEVEMENTS = "UpdateAchievements";

        public const string LOCK_SPAWNERS = "LockSpawners";
        public const string CALL_NEXTWAVEENEMIES = "CallNextWaveEnemies";
        public const string ENEMY_SPAWNED = "EnemySpawned";
        public const string WAVE_COMPLETE = "WaveComplete";

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
        public const string WEAPONSSELECT_FROM_STAGEOVER = "WEAPONSSELECT_FROM_STAGEOVER";

        public const string PLAYER_DAMAGED = "PlayerDamaged";
        public const string PLAYER_KILLED = "PlayerKilled";

        public const string SHOW_TUTORIAL_TEXT = "ShowTutorialText";
        public const string HIDE_TUTORIAL_TEXT = "HideTutorialText";

        public const string PLE_TEST_WAVE_0 = "PLE_TEST_WAVE_0";
        public const string PLE_TEST_WAVE_1 = "PLE_TEST_WAVE_1";
        public const string PLE_TEST_WAVE_2 = "PLE_TEST_WAVE_2";

        public const string ENEMY_INVINCIBLE = "ENEMY_INVINCIBLE";
        public const string GAME_CAN_PAUSE = "GAME_CAN_PAUSE";

        public const string INIT_EDITOR = "INIT_EDITOR";
    }

    public class Editor
    {
        public const string RESOURCE_PATH = "PlayerLevelEditorObjects/";
        public const string PLAYER_NAME = "Keira_GroupV1.5(Clone)";
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
        public const string MISSILE_GUN_ENABLED = "MissileGunEnabled";
        public const string BOOMERANG_ENABLED = "BoomerangEnabled";
        public const string LATEST_UNLOCKED_LEVEL = "LatestUnlockedLevel";
    }
}
