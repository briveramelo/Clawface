public enum ModSpot {
    Default = -1,
    ArmL = 0,
    ArmR = 1,
    Legs = 2
}

public enum StatType {
    Attack=0,
    Defense = 1,
    MoveSpeed = 2,
    Health = 3,
    RangedAccuracy= 4
}

public enum CodexType {
    Journal=0
}

public enum SFXType {
    ArmBlasterFire = 0,
    ArmBlasterExplode=1,
    TargetBreak=2,
    StunBatonSwing=3,
    StunBatonLayMine=4,
    StunBatonExplodeMine=5,
    ForceSegwayPush=6,
    FingerprintUnlock=7,
    ModPickup=8,
    ModDrop=9,
    ModSwap=10,
    ModSwapSetup=11,
    StunBatonHit=12
}

public enum ModType {
    ForceSegway=0,
    ArmBlaster=1,
    FingerPrint=2,
    StunBaton=3,
    TankTreads = 4,
    Grappler = 5
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
    Ground=11
}

public enum PoolObjectType {
    Mine=0,
    MineExplosionEffect=1,
    BlasterBullet=2,
    BlasterImpactEffect=3,
    MallCopSwinger=4,
    TargetExplosionEffect=5,
    BloodDecal=6,
    BloodEmitter=7,
    MallCopBlaster=8,
    VFXSegwayBlaster=9,
    MallCopExplosion=10
}

public enum WeaponType {
    Blaster=0,
    Baton=1        
}

public enum VictimType {
    MallCop=0,
    WallOrGround=1
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
    Shoot = 2,
    Float = 3,
    MeleeRight = 4,
    RunMeleeRight = 5,
    MeleeLeft = 6,
    RunMeleeLeft = 7
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

public enum MallCopType {
    Swinger=0,
    Blaster=1
}