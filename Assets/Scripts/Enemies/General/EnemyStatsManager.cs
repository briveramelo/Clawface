using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatsManager : Singleton<EnemyStatsManager>
{
    //Blaster Variants
    public BlasterStats blasterStats;
    public BlasterShotgunStats blasterShotgunStats;
    public BlasterReanimatorStats blasterReanimatorStats;

    //Zombie Variants
    public ZombieStats zombieStats;
    public ZombieBeserkerStats zombieBeserkerStats;
    public ZombieAciderStats zombieAciderStats;

    //Kamikaze Variants
    public KamikazeStats kamikazeStats;
    public KamikazePulserStats kamikazePulserStats;
    public KamikazeMommyStats kamikazeMommyStats;

    //Bouncer Variants
    public BouncerStats bouncerStats;
    public BouncerStats redBouncerStats;
    public BouncerStats greenBouncerStats;

}

[System.Serializable]
public class SharedEnemyStats
{
    public float health;
    public float maxHealth;
    public float skinnableHealth;
    public float attack;

    //Nav agent modifiers
    public float speed;
    public float angularSpeed;
    public float acceleration;
    public float stoppingDistance;

    public int scoreValue;
    public int eatHealth;
    public float stunnedTime;
}

[System.Serializable]
public class BlasterStats : SharedEnemyStats
{
    public float closeEnoughToFireDistance;
    public float maxToleranceTime;

    //Blaster Mod
    [Range(0.1f, 10f)] public float bulletLiveTime;
    [Range(1f, 100f)] public float bulletSpeed;

    [Range(1.0f, 10.0f)] public float animationShootSpeed;
}

[System.Serializable]
public class BlasterShotgunStats : SharedEnemyStats
{
    public float closeEnoughToFireDistance;
    public float maxToleranceTime;

    //Bullet Hell Pattern Controller
    [Range(-360.0f, 360.0f)] public float separationFromForwardVector;
    [Range(0.0f, 100.0f)] public float bulletSpeed;
    [Range(0.0f, 100.0f)] public float bulletDamage;
    [Range(0.0f, 10.0f)] public float rateOfFire;
    [Range(0.0f, 10.0f)] public float bulletOffsetFromOrigin;
    [Range(0.0f, 30.0f)] public int bulletStrands;
    [Range(0.0f, 360.0f)] public float separationAngleBetweenStrands;
    public BulletHellPatternController.RotateDirection rotationDirection;
    [Range(0.0f, 10.0f)] public float rotationSpeedBulletHellController;
    public float bulletLiveTime = 1.0f;
    public bool animationDriven;
    [Range(1.0f, 10.0f)] public float animationShootSpeed;
}

[System.Serializable]
public class BlasterReanimatorStats : SharedEnemyStats
{
    public float closeEnoughToReanimateDistance;
    [Range(1.0f, 10.0f)] public float animationShootSpeed;
}

[System.Serializable]
public class BouncerStats : SharedEnemyStats
{
    [Range(1, 10)] public int maxBounces;
    [Range(1, 10)] public int minBounces;
    [Range(1, 100)] public int maxShots;
    [Range(1, 100)] public int minShots;
    [Range(0f, 100f)] public float rotationSpeed;
    public bool rotate;

    //Bullet Hell Pattern Controller
    [Range(-360.0f, 360.0f)] public float separationFromForwardVector;
    [Range(0.0f, 100.0f)] public float bulletSpeed;
    [Range(0.0f, 100.0f)] public float bulletDamage;
    [Range(0.0f, 10.0f)] public float rateOfFire;
    [Range(0.0f, 10.0f)] public float bulletOffsetFromOrigin;
    [Range(0.0f, 30.0f)] public int bulletStrands;
    [Range(0.0f, 360.0f)] public float separationAngleBetweenStrands;
    public BulletHellPatternController.RotateDirection rotationDirection;
    [Range(0.0f, 10.0f)] public float rotationSpeedBulletHellController;
    public float bulletLiveTime = 1.0f;
    public bool animationDriven;
    [Range(1.0f, 10.0f)] public float animationShootSpeed;

}

[System.Serializable]
public class KamikazeStats : SharedEnemyStats
{
    public float closeEnoughToAttackDistance;
    [Range(0.1f, 5f)] public float selfDestructTime;
    [Range(1f, 100f)] public float blastRadius;
}

[System.Serializable]
public class KamikazePulserStats : SharedEnemyStats
{
    public float closeEnoughToAttackDistance;
    [Range(1, 10)] public int maxPulses;
    [Range(0f, 10f)] public float pulseRate;

    //Pulse stats
    [Range(0f, 10f)] public float scaleRate;
    [Range(0.1f, 10f)] public float maxScale;
}

[System.Serializable]
public class KamikazeMommyStats : SharedEnemyStats
{
    public float closeEnoughToAttackDistance;
    [Range(0f, 1f)] public float kamikazeSpawnProbability;
    [Range(0f, 1f)] public float kamikazePulserSpawnProbability;
}

[System.Serializable]
public class ZombieStats : SharedEnemyStats
{
    public float closeEnoughToAttackDistance;
    [Range(1.0f, 10.0f)] public float animationAttackSpeed;
}

[System.Serializable]
public class ZombieBeserkerStats : SharedEnemyStats
{
    public float closeEnoughToAttackDistance;
    [Range(1.0f, 10.0f)] public float animationAttackSpeed;
}

[System.Serializable]
public class ZombieAciderStats : SharedEnemyStats
{
    public float closeEnoughToAttackDistance;
    [Range(1.0f, 10.0f)] public float animationAttackSpeed;
    [Range(1.0f, 10.0f)] public float trailRendererTime;
    [Range(1.0f, 10.0f)] public float trailRendererWidth;
    [Range(0.0f, 10.0f)] public float colliderGenerationTime;
    [Range(1.0f, 10.0f)] public float acidTriggerLife;
}