using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class BlasterBullet : MonoBehaviour {

    private VFXHandler vfxHandler;
    private ShooterProperties shooterProperties=new ShooterProperties();

	// Use this for initialization
	void Start () {        
        vfxHandler = new VFXHandler(transform);
    }

    void OnEnable()
    {        
        Timing.RunCoroutine(DestroyAfter());
    }

    private IEnumerator<float> DestroyAfter() {
        yield return Timing.WaitForSeconds(3f);
        if (gameObject.activeSelf){
            vfxHandler.EmitForBulletCollision();
            gameObject.SetActive(false);
        }
    }

    void Update () {
        transform.Translate(Vector3.forward * shooterProperties.speed * Time.deltaTime);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetInstanceID()!=shooterProperties.shooterInstanceID)
        {
            bool isEnemy = other.gameObject.CompareTag(Strings.Tags.ENEMY);
            bool isPlayer = other.gameObject.CompareTag(Strings.Tags.PLAYER);
            if (isEnemy || isPlayer)
            {
                Damage(other.gameObject.GetComponent<IDamageable>());
                Push(other.gameObject.GetComponent<IMovable>());
                if (isEnemy) {
                    EmitBlood();                                              
                }
            }
            if (isEnemy || isPlayer || other.gameObject.layer==(int)Layers.Ground) {
                vfxHandler.EmitForBulletCollision();
                gameObject.SetActive(false);
            }
        }
    }

    public void SetShooterProperties(ShooterProperties shooterProperties) {
        this.shooterProperties = shooterProperties;
    }

    private void Damage(IDamageable damageable) {        
        if (damageable != null) {
            damageable.TakeDamage(shooterProperties.damage);
        }
    }

    private void Push(IMovable movable) {
        Vector3 forceDirection = transform.forward;        
        if (movable != null) {
            movable.AddDecayingForce(forceDirection.normalized * shooterProperties.pushForce);
        }
    }

    private void EmitBlood() {
        //TODO: Create Impact effect needs to take into account
        // the type of surface that it had hit.
        if (Mathf.Abs(transform.forward.y) < 0.5f) {
            vfxHandler.EmitBloodBilaterally();
        }
        else {
            vfxHandler.EmitBloodInDirection(Quaternion.Euler(Vector3.right * 90f), transform.position);
        }
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