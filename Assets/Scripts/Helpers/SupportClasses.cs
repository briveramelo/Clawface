using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

[System.Serializable]
public class CapsuleBounds {

    public Transform start, end;
    public float radius;
    public Vector3 Start{ get{ return start.position;} }
    public Vector3 End{ get{ return end.position;} }
}

[System.Serializable]
public class CapsuleBoundsDirection {

    public Transform start, endingDirection;
    public float radius, length;
    public Vector3 Start { get { return start.position; } }
    public Vector3 End { get { return start.position + (endingDirection.position - start.position).normalized * length; } }
}

[System.Serializable]
public class BoxBounds {

    public Transform center, corner;
    public Vector3 Center{ get{ return center.position;} }
    public Vector3 Corner{ get{ return corner.position;} }
    public Vector3 Size{ get{ return 2*(Center-Corner);} }
}

public class TransformMemento {

    public Vector3 startPosition;
    public Vector3 startScale;
    public Quaternion startRotation;

    public void Initialize(Transform transform) {
        startPosition = transform.localPosition;
        startScale = transform.localScale;
        startRotation = transform.localRotation;
    }
    public void Reset(Transform transform) {
        transform.localPosition = startPosition;
        transform.localScale = startScale;
        transform.localRotation = startRotation;
    }
}

public class Will {
    public OnDeath onDeath;
    public bool willHasBeenWritten;
    public bool deathDocumented;
    public bool isDead;
    public void Reset() {
        willHasBeenWritten=false;
        deathDocumented=false;
        isDead=false;
        onDeath = null;
    }
}

public class ProjectileProperties {
    public int shooterInstanceID;
    public float damage;
    public ProjectileProperties() { }
    public ProjectileProperties(int shooterInstanceID, float damage) {
        this.shooterInstanceID = shooterInstanceID;
        this.damage = damage;
    }
    public void Initialize(int shooterInstanceID, float damage) {
        this.shooterInstanceID = shooterInstanceID;
        this.damage = damage;
    }
}

public class ShooterProperties {
    public int shooterInstanceID {
        get { return projectileProperties.shooterInstanceID; }
        set { projectileProperties.shooterInstanceID = value; }
    }
    public float damage { get { return projectileProperties.damage; } }
    public float speed;
    public float pushForce;
    private ProjectileProperties projectileProperties=new ProjectileProperties();
    public void Initialize(int shooterInstanceID, float damage, float speed, float pushForce)
    {
        projectileProperties.Initialize(shooterInstanceID, damage); 
        this.pushForce = pushForce;
        this.speed = speed;
    }
    public void Initialize(ProjectileProperties projectileProperties, float speed, float pushForce) {
        this.projectileProperties = projectileProperties;
        this.pushForce = pushForce;
        this.speed = speed;
    }
    public ShooterProperties() { }
}

public class Damager{
    public float damage;
    public DamagerType damagerType;
    public Vector3 impactDirection;
    public void Set(float damage, DamagerType damagerType, Vector3 impactDirection) {
        this.damage=damage;
        this.damagerType=damagerType;
        this.impactDirection=impactDirection;
    }
}

public class Damaged {
    public DamagedType damageType;
    public Transform owner;
    public void Set(DamagedType damageType, Transform owner) {
        this.damageType=damageType;
        this.owner=owner;
    }
}

public class DamagePack {
    public Damager damager;
    public Damaged damaged;
    public void Set(Damager damager, Damaged damaged) {
        this.damager=damager;
        this.damaged=damaged;
    }
}
