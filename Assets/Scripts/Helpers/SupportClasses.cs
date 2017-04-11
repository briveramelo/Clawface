using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CapsuleBounds {

    public Transform start, end;
    public float radius;
    public Vector3 Start{ get{ return start.position;} }
    public Vector3 End{ get{ return end.position;} }
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
    public int shooterInstanceID { get { return projectileProperties.shooterInstanceID; } }
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

