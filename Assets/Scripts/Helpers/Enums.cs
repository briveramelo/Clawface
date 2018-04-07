public enum ModSpot {
    Default = -1,
    ArmL = 0,
    ArmR = 1
}

public enum CharacterStatType {
    Attack=0,    
    MoveSpeed = 2,
    Health = 3,
    MaxHealth = 5,
    EXP = 6
}

public enum CodexType {
    Journal=0
}

public enum SFXType {
    BloodExplosion,
    BlasterProjectileImpact,
    BlasterShoot,
    Dash,
    Boomerang_Throw,
    Boomerang_Impact,
    PlayerTakeDamage,
    PlayerDeath,
    UI_Click,
    UI_Hover,
    UI_Back,
    SpreadshotShoot,
    LightningGunShoot,
    GeyserShoot,
    ClawExtend,
    ClawGrab,
    PlayerEat,
    AnnounceDeath,
    AnnounceLevelStart,
    AnnounceTitle,
    MortarShoot,
    MortarExplosion,
    LandmarkBlast,
    GuardAttack,
    ZombieAttack,
    BouncerAttack,
    KamikazeAttack,
    KamikazePulse,
    LandmarkBlastShort,
    PLEPlaceObject,
    PLEHoverUI,
    PLEClickUI,
    BoomerangWallImpact,
    BouncerLand,
    BouncerVocalize,
    EnemyFootstep,
    KamikazeFootstep,
    ZombieVocalize,
    None,
    AcidZombieVocalize,
    TileLift,
    Score,
    KamikazeVocalize,
    KamikazeWarn,
    GuardVocalize,
    GuardShotgunAttack,
    GuardPrepare
}

public enum MusicType
{
    MainMenu_Intro,
    MainMenu_Loop,
    Hathos_Lo,
    Hathos_Med,
    Hathos_Hi
}

public enum DamagerType {
    SegwayPush=0,
    BlasterBullet=1,
    TankTreads=3,
    GrapplingHook=4,
    Boomerang=5,
    Geyser=6,
    FireTrap=8,
    GrapplingBotExplosion=9,
    Dice=10,
    SpreadGun = 12,
    Kamikaze = 13, 
}

public enum DamagedType {
    MallCop=0,
    Milo=1,
    Zombie=2,
    Bouncer=3,
    Kamikaze = 4,
    RedBouncer = 5,
    GreenBouncer = 6,
}

public enum ModType {
    SpreadGun = 0,
    Blaster = 1,
    LightningGun = 2,
    Boomerang = 3,
    Geyser = 4,
    Missile = 5,
    None = 6
}

public enum ModCategory
{
    None = 0,
    Melee = 1,
    Ranged = 2
}

public enum CharacterType {
    ModMan=0,
    MallCop=1,
    Butcher=2
}

public enum Layers {
    ModMan=8,
    Enemy=9,
    MiniMap=10,
    Ground=11,
    Blood=12,
    Hologram=13,
    Globe_Text=14,
    PlayerDetector=15,
    DiceBlocks=17,
    Obstacle=19
}

public enum PoolObjectType {
    
    //Misc
	GeyserShield = 19,
    GeyserGushLine = 21,
    GeyserBase = 23,
    WorldScoreCanvas = 26,
    GeyserFissure = 29,	
    BlasterImpactEffect = 31,
    KamikazeExplosionSphere = 48,
    KamikazePulseGenerator = 54,
    AcidTrigger = 59,
    AcidTrail = 72,

    //VFX
    VFXBlasterImpactEffect = 3,
    VFXBloodDecal = 6,
    VFXSegwayBlaster = 9,
    VFXMallCopExplosion = 10,
    VFXBlasterShoot = 17,
    VFXDiceBlockExplosion=22,
    VFXSkinningEffect = 24,
    VFXHealthGain = 25,
    VFXBoomerangShoot = 38,
    VFXBoomerangImpact = 39,
    VFXGeyserShoot = 41,
    VFXGeyserImpact = 42,
    VFXKamikazeExplosion = 43,
    VFXLightningGunImpact = 44,
    VFXLightningGunShoot = 45,
    VFXPlayerDeath = 49,
    VFXEnemySpawn=50,
    VFXSpreadshotShoot=51,
    VFXSpreadshotImpact=52,
    VFXEnemyProjectileImpact=53,
    VFXMortarExplode=61,
    VFXFish=62,
    VFXMortarShell=63,
    VFXMortarShoot=64,
    VFXKamikazeExplosionWarning=65,
    VFXEnemyChargeBlaster=66,
    VFXBoomerangProjectileDie=68,
    VFXBloodSpurt=69,
    VFXKamikazePulse=70,
    VFXPLEPlaceObject=71,

    //BulletTypes
    TurretBullet = 28,
    SpreadGunBullet = 30,
    BoomerangProjectile = 20,
    DiceBlock=13,
    GeyserProjectile = 12,
    BlasterBullet=2,
    LightningProjectile = 27,
    EnemyBulletSmall = 35,
    EnemyBulletMedium = 36,
    EnemyBulletLarge = 37,
    MissileProjectile = 40,

    //Enemy Types
    GrapplingBot=11,
    MallCopBlaster=8,
    Zombie = 32,
    Bouncer = 33,
    Kamikaze = 34,
    RedBouncer = 46,
    GreenBouncer = 47,
    KamikazePulser = 55,
    KamikazeMommy = 56,
    ZombieBeserker = 57,
    ZombieAcider = 58,
    BlasterShotgun = 60,
    BlasterReanimator = 67,
}

public enum MovementMode
{
    PRECISE = 0,
    ICE = 1
}

public enum PlayerAnimationStates
{
    Idle = 0,
    RunningForward = 1,
    Dash = 2,
    OpenFace = 3,
    CloseFace = 4,
    SideStrafeRight = 5,
    SideStrafeLeft = 6,
    RunningBackward = 7
}

public enum PlayerAnimationLayers
{
    BaseLayer = 0,
    SkinningLayer = 1
}

public enum ButtonMode
{
    UP = 0, // just went up
    HELD = 1, // is held
    DOWN = 2, // just went down
    IDLE= 3 // is released (not used)
}


public enum VibrationTargets
{
    LEFT = 0,
    RIGHT = 1,
    BOTH = 2
}

public enum AnimationStates {
    Idle = 0,
    Walk = 1,

    Attack1 = 2,
    Attack2 = 21,
    Attack3 = 22,
    Attack4 = 33,
    Attack5 = 34,
    Attack6 = 35,

    Fire1 = 3,
    Fire2 = 31,
    Fire3 = 32,

    ReadyFire = 301,
    EndFire = 302,

    Stunned = 4,

    StartingJump = 5,
    Jumping = 51,
    EndJump = 52,

    TurnLeft = 6,
    TurnRight = 61,
    
    Celebrate1 = 7,
    Celebrate2 = 71,
    Celebrate3 = 72,
    Celebrate4 = 73,
    Celebrate5 = 74,

    HitReaction1 = 8,
    HitReaction2 = 81,
    HitReaction3 = 82,

    GetUp = 9
}

public enum EAIState
{
    Chase = 0,
    Attack = 1,
    Fire = 3,
    Death = 4,
    Stun = 5,
    Celebrate = 6,
    GetUp = 7
}


public enum EGrapplingBotState {
    Patrol = 0,
    Grapple = 1,
    Explode = 2,
    Approach = 3,
    Twitch = 4,
}

public enum SpawnType {
    Blaster = 0,
    Zombie = 1,
    Bouncer = 2,
    Kamikaze = 3,
    RedBouncer = 4,
    GreenBouncer = 5,
    KamikazePulser = 6,
    KamikazeMommy = 7,
    ZombieBeserker = 8,
    ZombieAcider = 9,
    BlasterShotgun = 10,
    BlasterReanimator = 11,
    Keira=12,

}

public enum ActionType
{
    Skin=0
}

public enum EditorMenu
{
    MAIN_EDITOR_MENU = 0,
    FLOOR_MENU = 1,
    PROPS_MENU = 2,
    SPAWN_MENU = 3,
    SAVE_MENU = 4,
    HELP_MENU = 5

}

public enum FireMode
{
    AIM_TO_SHOOT = 0,
    AUTOFIRE,
    MANUAL,
    SINGLE_TRIGGER,
    COUNT
}

public enum MouseAimMode
{
    AUTOMATIC,
    ALWAYS_ON,
    ALWAYS_OFF,
    COUNT
}

public enum Difficulty
{
    EASY,
    NORMAL,
    HARD,
    VERY_EASY,
    INSANE,
    COUNT
}

public enum PLEMenuType
{
    INIT,
    MAIN,
    PROPS,
    FLOOR,
    SPAWN,
    SAVE,
    HELP,
    WAVE,
    TEST,
    LEVELSELECT,
    STEAM,
    EXIT,
    NONE
}