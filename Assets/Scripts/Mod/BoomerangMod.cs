using MovementEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangMod : Mod {


    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private float chargedMaxDistance;
    [SerializeField] private float standardMaxDistance;
    [SerializeField] private float ellipseHeightWidthRatio;
    [SerializeField] private SphereCollider damageCollider;
    [SerializeField] private float boomerangSpeed;
    [SerializeField] private float chargedScale;    
    [SerializeField] private float maxRotationRate;
    [SerializeField] private float minRotationRate;
    [SerializeField]
    private int chargeFeetBoomerangCount;
    [SerializeField]
    private float chargeFeetDelayBetweenBoomerangs;
    #endregion

    #region Private Fields
    private Vector3 initialPosition;
    private Transform modSocket;
    private bool returning;
    private Matrix4x4 TRMatrix;
    private float enemyDistance;
    private float majorAxisRadius;
    private Vector3 initialScale;
    private Vector3 pickUpScale;
    private GameObject boomerangProjectile;
    private List<GameObject> boomerangProjectiles;
    private float angle;
    private float rotation;
    private float rightHandStartAngle = -80f;
    private float rightHandEndAngle = 250f;
    private float leftHandStartAngle = 260f;
    private float leftHandEndAngle = -80f;
    #endregion

    #region Unity Lifecycle

    // Use this for initialization
    void Start () {
        type = ModType.Boomerang;
        category = ModCategory.Ranged;
        damageCollider.enabled = false;
        enemyDistance = Mathf.Infinity;
        initialScale = transform.localScale;
        boomerangProjectiles = new List<GameObject>();
        majorAxisRadius = standardMaxDistance/2.0f;
    }

    private void FixedUpdate() {
        if (energySettings.isActive)
        {
            if (getModSpot() == ModSpot.ArmR)
            {
                UpdateBoomerangPosition();
            }
            else if (getModSpot() == ModSpot.ArmL)
            {
                UpdateBoomerangPosition(true);
            }
        }
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();
       
	}    

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetInstanceID()!=GetWielderInstanceID() && energySettings.isActive){
            if(other.gameObject.CompareTag(Strings.Tags.ENEMY) || other.gameObject.CompareTag(Strings.Tags.PLAYER)){
                IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
                if (damageable!=null){
                    damager.Set(Attack, getDamageType(), wielderMovable.GetForward()); 
                    damageable.TakeDamage(damager);
                }
            }            
        }
    }
    #endregion

    #region Public Methods
    public bool IsActive() { return energySettings.isActive; }

    public override void Activate(Action onCompleteCoolDown=null, Action onActivate=null)
    {        
        base.Activate(onCompleteCoolDown, onActivate);
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable){                
        base.AttachAffect(ref wielderStats, wielderMovable);
        pickUpScale = transform.localScale;
    }

    public override void DeActivate()
    {
        angle = 0.0f;
        rotation = 0.0f;
        energySettings.isActive = false;
        damageCollider.enabled = false;        
        returning = false;
        transform.parent = modSocket;
        transform.localScale = pickUpScale;        
        transform.localPosition = Vector3.zero;
        initialPosition = Vector3.zero;        
        transform.forward = wielderMovable.GetForward();
    }

    public override void DetachAffect()
    {
        base.DetachAffect();
        damageCollider.enabled = false;        
    }

    public void SetEnemyDistance(float distance)
    {
        enemyDistance = distance;
    }
    #endregion

    #region Private Methods  
    protected override void BeginChargingArms() { }
    protected override void RunChargingArms() { GrowSize(); }
    protected override void ActivateStandardArms() { ReleaseBoomerang(); }
    protected override void ActivateChargedArms() { ReleaseBoomerang(); }

    protected override void BeginChargingLegs() { }
    protected override void RunChargingLegs() { }
    protected override void ActivateChargedLegs() {
        FireFromDick(true);
    }
    protected override void ActivateStandardLegs() {
        FireFromDick();
    }

    private void FireFromDick(bool charge = false)
    {
        if (charge)
        {
            if (NoActiveProjectiles())
            {
                boomerangProjectiles.Clear();
                Timing.RunCoroutine(SpawnTheHorde(),Segment.FixedUpdate);
            }
        }else
        {
            if (boomerangProjectile == null || !boomerangProjectile.activeSelf)
            {
                boomerangProjectile = FireBoomerang();
            }
        }
    }

    private IEnumerator<float> SpawnTheHorde()
    {
        int count = 0;
        while (count < chargeFeetBoomerangCount)
        {
            GameObject projectile = FireBoomerang(true);
            if (projectile)
            {
                boomerangProjectiles.Add(projectile);
            }
            count++;
            yield return Timing.WaitForSeconds(chargeFeetDelayBetweenBoomerangs);
        }
        yield return 0f;
    }

    private bool NoActiveProjectiles()
    {
        foreach(GameObject projectile in boomerangProjectiles)
        {
            if (projectile.activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    private GameObject FireBoomerang(bool charge = false)
    {
        GameObject projectile = ObjectPool.Instance.GetObject(PoolObjectType.BoomerangProjectile);
        if (projectile)
        {
            transform.rotation = Quaternion.identity;
            projectile.GetComponent<BoomerangProjectile>().Go(wielderStats, wielderStats.gameObject.GetInstanceID(), transform, charge);
        }
        return projectile;
    }

    private void GrowSize() {
        transform.localScale = pickUpScale * (1+ (chargedScale) * energySettings.chargeFraction);
    }
    private void ReleaseBoomerang(){
        energySettings.isActive = true;
        damageCollider.enabled = true;
        initialPosition = transform.position;
        transform.rotation = Quaternion.identity;
        modSocket = transform.parent;
        transform.SetParent(null);
        
        TRMatrix = Matrix4x4.TRS(initialPosition, wielderMovable.GetRotation(), Vector3.one);
        transform.forward = wielderMovable.GetForward();
        if(getModSpot() == ModSpot.ArmL)
        {
            angle = leftHandStartAngle;
        }else if(getModSpot() == ModSpot.ArmR)
        {
            angle = rightHandStartAngle;
        }
    }
    void UpdateBoomerangPosition(bool leftHand = false){
        if (leftHand)
        {
            angle -= boomerangSpeed;
        }
        else
        {
            angle += boomerangSpeed;
        }
        //Equation of an ellipse
        // x = a*cos(t)
        float x = minorAxisRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
        // y = b*sin(t)
        float z = majorAxisRadius * Mathf.Sin(angle * Mathf.Deg2Rad) + majorAxisRadius;
        transform.position = TRMatrix.MultiplyPoint3x4(new Vector3(x, 0f, z));
        rotation += minRotationRate;
        transform.rotation = Quaternion.AngleAxis(rotation, Vector3.up);
        if (leftHand)
        {
            if (angle < leftHandEndAngle)
            {
                DeActivate();
            }
        }
        else
        {
            if (angle > rightHandEndAngle)
            {
                DeActivate();
            }
        }
    }

    private float GetXCoordinate(float y) {
        float ySquared = y * y;
        float bSquared = majorAxisRadius * majorAxisRadius;
        float aSquared = minorAxisRadius * minorAxisRadius;
        return minorAxisRadius * Mathf.Sqrt(1 - ySquared / bSquared);
    }
    #endregion

    #region Private Structures
    private float minorAxisRadius {
        get {
            return majorAxisRadius/ellipseHeightWidthRatio;
        }
    }
    private float maxDistance {
        get {
            return IsCharged() ? chargedMaxDistance : standardMaxDistance;
        }
    }
    
    #endregion

}
