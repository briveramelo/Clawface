﻿public enum ModSpot {
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
    GrapplingGun_Shoot,
    TankTreads_Attack,
    TankTreads_Swing, 
    Dash,
    SegwayBlast_Standard,
    SegwayBlast,
    Boomerang_Throw, 
    DiceLauncher_Shoot,
    GeyserMod_Splash,
    GeyserMod_MiniSplash,
    Boomerang_Impact,
    PlayerTakeDamage,
    PlayerDeath,
    ModCooldown,
    MallCopHurt,
    UI_Click,
    UI_Hover,
    UI_Back
}

public enum MusicType
{
    MainMenu_Track
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
}

public enum DamagedType {
    MallCop=0,
    Milo=1,
    Zombie=2,
}

public enum ModType {
    ForceSegway = 0,
    ArmBlaster = 1,
    Grappler = 2,
    Boomerang = 3,
    Geyser = 4,
    Dice = 5,
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
    DiceBlocks=17        
}

public enum PoolObjectType {
    BlasterBullet=2,
    VFXBlasterImpactEffect=3,
    VFXBloodDecal=6,
    VFXBloodEmitter=7,
    MallCopBlaster=8,
    VFXSegwayBlaster=9,
    VFXMallCopExplosion=10,
    GrapplingBot=11,
    GeyserProjectile = 12,
    DiceBlock=13,
    VFXBlasterShoot=17,
	GeyserShield = 19,
    BoomerangProjectile = 20,
    GeyserGushLine = 21,
    VFXDiceBlockExplosion=22,
    GeyserBase = 23,
    VFXSkinningEffect = 24,
    VFXHealthGain = 25,
    WorldScoreCanvas = 26,
    GrapplingHook = 27,
    TurretBullet = 28,
    GeyserFissure = 29,	
    SpreadGunBullet = 30
}

public enum MovementMode
{
    PRECISE = 0,
    ICE = 1
}

public enum PlayerAnimationStates
{
    Idle = 0,
    Running = 1,    
    StunBaton = 2,
    Boomerang = 3,
    Dash = 4,
    TankTreads = 5
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

public enum MallCopAnimationStates {
    Idle = 0,
    Walk = 1,
    Swing = 2,
    HitReaction = 3,
    Stunned = 4,
    GettingUp = 5,
    DrawGun = 6,
    Run = 7,
    Fire = 8
}

public enum EMallCopState {
    Patrol = 0,
    Swing = 1,
    Fall = 3,
    Chase = 4,
    Twitch = 5,
    Fire = 6,
    Flee = 7
}

public enum EZombieState
{
    Patrol = 0,
    Chase = 1,
    Attack = 3,
    Fall = 4,
    Twitch = 5
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
    Grappler = 1,
}

public enum ActionType
{
    Skin=0
}