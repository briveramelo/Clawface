public class Strings
{
    public const string BLOCKINGOBJECT = "BLOCKING OBJECT";
    public const string ANIMATIONSTATE = "AnimationState";
    public const string FEETSTATE = "FeetState";
    public const string RESPAWN_POINT = "RespawnPoint";
    public const string CLONE = "(Clone)";
    public const string PREVIEW = "Preview";
    public const string GHOST_BLOCK = "GhostBlock";
    public const string REAL_BLOCK = "RealBlock";

    public class Paths
    {
        public static string PLAYER_PREFAB_NAME = "Keira_GroupV";
        public static string PLAYER_PREFAB_RESOURCES_PATH = "Player/";

        public static string PLAYER_UI_PREFAB_NAME = "PlayerHUDV";
        public static string PLAYER_UI_PREFAB_RESOURCES_PATH = "PlayerUI/";

        //plaer editor
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
        public class ScenePaths {
            public const string ScenesDirectory = "Scenes/Build Scenes/";

            public const string MainMenu = ScenesDirectory + SceneNames.MainMenu;
            public const string Arena = ScenesDirectory + SceneNames.Arena;
            public const string Editor = ScenesDirectory + SceneNames.Editor;
            public const string PlayerLevels = ScenesDirectory + SceneNames.PlayerLevels;
        }

        public class SceneNames {
            public const string MainMenu = "MainMenu";
            public const string Arena = "80s shit";
            public const string Editor = "Editor";
            public const string PlayerLevels = "PlayerLevels";
        }
    }    

    public class Layers
    {
        public const string UI = "UI";
        public const string MODMAN = "ModMan";
        public const string ENEMY = "Enemy";
        public const string MINI_MAP = "MiniMap";
        public const string GROUND = "Ground";
        public const string BLOOD = "Blood";
        public const string HOLOGRAM = "Hologram";
        public const string GLOBE_TEXT = "Globe_Text";
        public const string PLAYER_DETECTOR = "PlayerDetector";
        public const string INVISIBLE_ENEMY_FENCE = "InvisibleEnemyFence";
        public const string DICE_BLOCKS = "DiceBlocks";
        public const string OUTLINE = "EnemyProjectile";
        public const string OBSTACLE = "Obstacle";
        public const string ENEMY_BODY = "EnemyBody";
        public const string ENEMY_PROJECTILE = "EnemyProjectile";
        public const string CLAW = "Claw";
        public const string SPAWN = "Spawn";
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
        public class LevelEditor {
            public const string ADD_PROPS_PLE = "PropsMenu";
            public const string ADD_SPAWNS_PLE = "SpawnMenu";
            public const string SET_DYNLEVEL_PLE = "FloorMenu";
            public const string MAIN_PLE_MENU = "MainPLEMenu";
            public const string INIT_PLE_MENU = "InitPLEMenu";
            public const string SAVE_PLE_MENU = "SaveMenu";
            public const string WAVE_PLE_MENU = "WaveMenu";
            public const string HELP_PLE_MENU = "HelpMenu";
            public const string TEST_PLE_MENU = "TestMenu";
            public const string LEVELSELECT_PLE_MENU = "PLELevelSelectMenu";
        }
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
        public const string FINISHED_EATING = "FinishedEating";

        public const string KILL_ENEMY = "KillEnemy";
        public const string BEAT_LEVEL1 = "BeatLevel1";
        public const string SKIN_ENEMY = "SkinEnemy";
        public const string DEATH_ENEMY = "DeathEnemy";
        public const string EAT_ENEMY = "EatEnemy";

        public const string EARN_ACHIEVEMENT = "EarnAchievement";
        public const string PROGRESS_ACHIEVEMENT = "ProgressAchievement";
        public const string UPDATE_ACHIEVEMENTS = "UpdateAchievements";

        public const string LOCK_SPAWNERS = "LockSpawners";
        public const string CALL_NEXT_WAVE = "CallNextWave";
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
        public const string WEAPONS_SELECT_FROM_STAGE_OVER = "WEAPONS_SELECT_FROM_STAGE_OVER";

        public const string PLAYER_DAMAGED = "PlayerDamaged";
        public const string PLAYER_KILLED = "PlayerKilled";

        public const string SHOW_TUTORIAL_TEXT = "ShowTutorialText";
        public const string HIDE_TUTORIAL_TEXT = "HideTutorialText";


        public const string PLE_TEST_WAVE_  = "PLE_TEST_WAVE_";
        public const string PLE_TEST_WAVE_0 = "PLE_TEST_WAVE_0";
        public const string PLE_TEST_WAVE_1 = "PLE_TEST_WAVE_1";
        public const string PLE_TEST_WAVE_2 = "PLE_TEST_WAVE_2";
        public const string PLE_CHANGEWAVE = "PLE_ChangeWave";
        public const string PLE_ADD_WAVE    = "PLE_ADD_WAVE";
        public const string PLE_TEST_END = "PleTestEnd";

        public const string PLE_DELETE_CURRENTWAVE = "PLE_DELETE_CURRENTWAVE";
        public const string PLE_UPDATE_LEVELSTATE  = "PLE_UPDATE_LEVELSTATE";
        public const string PLE_RESET_LEVELSTATE   = "PLE_RESET_LEVELSTATE";

        public const string ENEMY_INVINCIBLE = "ENEMY_INVINCIBLE";
        public const string GAME_CAN_PAUSE = "GAME_CAN_PAUSE";

        public const string INIT_EDITOR = "INIT_EDITOR";

        public const string ACTIVATE_MOD = "ACTIVATE_MOD";
        public const string DEACTIVATE_MOD = "DEACTIVATE_MOD";

        public const string SCENE_LOADED = "SCENE LOADED";        
    }

    public class Editor
    {
        //folders
        public const string RESOURCE_PATH = "PlayerLevelEditorObjects/";
        public const string PLAYER_NAME = "Keira_GroupV1.5(Clone)";

        //others
        public const string LEVEL_OBJECT = "LEVEL";

        //objects
        public const string BASIC_LE_BLOCK = "LECube";
        public const string BASIC_LVL_BLOCK = "PLEBlockUnit_Default";
        public const string CHERLIN_LVL_BLOCK = "PLEBlockUnit_Cherlin";
        public const string ENV_OBJECTS_PATH = "PlayerLevelEditorObjects/EnvProps/";
        public const string SPAWN_OBJECTS_PATH = "PlayerLevelEditorObjects/SpawnProps/";

        public const string IMAGE_PREVIEW_PATH = "PlayerLevelEditorObjects/png/";

        public const string Tiles = "Tiles";
        public const string Props = "Props";
        public const string Spawns = "Spawns";
        public const string Wave = "Wave";

        public const string PLAYER_SPAWN_TAG = "PLE_PlayerSpawn";
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
