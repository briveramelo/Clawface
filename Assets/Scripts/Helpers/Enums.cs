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
    PlayerEat
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
    DiceBlocks=17        
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

    //VFX
    VFXBlasterImpactEffect = 3,
    VFXBloodDecal = 6,
    VFXBloodEmitter = 7,
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
    Dash = 2,
    OpenFace = 3,
    CloseFace = 4
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
    HitReaction3 = 82

}

public enum EAIState
{
    Chase = 0,
    Attack = 1,
    Fire = 3,
    Death = 4,
    Stun = 5,
    Celebrate = 6,
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
}

public enum ActionType
{
    Skin=0
}