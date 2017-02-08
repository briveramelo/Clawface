public enum ModSpot {
    Default = -1,
    Head = 0,
    ArmL = 1,
    ArmR = 2,
    Legs = 3
}

public enum StatType {
    Attack=0,
    Defense = 1,
    MoveSpeed = 2,
    Health = 3,
    MiniMapRange = 4,
    RangedAccuracy= 5
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
    ModSwapSetup=11
}

public enum ModType {
    ForceSegway=0,
    ArmBlaster=1,
    FingerPrint=2,
    StunBaton=3
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

enum MallCopState
{
    WALK = 0,
    ATTACK = 1,
    STUNNED = 2
}

public enum MallCopAnimationStates
{
    Idle = 0,
    Walk = 1,
    Swing = 2,
    HitReaction = 3,
    Stunned = 4,
    GettingUp = 5,
    DrawWeapon = 6,
    Run = 7,
    Shoot = 8
}

public enum MovementMode
{
    PRECISE = 0,
    ICE = 1
}
