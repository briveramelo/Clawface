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
    [SerializeField] private float maxMovementSpeed;
    [SerializeField] private float minMovementSpeed;
    [SerializeField] private float chargedScale;    
    [SerializeField] private float maxRotationRate;
    [SerializeField] private float minRotationRate;
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
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        type = ModType.Boomerang;
        category = ModCategory.Ranged;
        damageCollider.enabled = false;
        enemyDistance = Mathf.Infinity;
        initialScale = transform.localScale;        
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
        if (energySettings.isActive) {
            if (getModSpot() == ModSpot.ArmR)
            {
                UpdateBoomerangPosition();
            }else if (getModSpot() == ModSpot.ArmL)
            {
                UpdateBoomerangPosition(-1);
            }
        }
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

    public override void Activate(Action onComplete=null)
    {
        base.Activate();
    }

    protected override void BeginChargingArms(){ }
    protected override void RunChargingArms(){ GrowSize(); }
    protected override void ActivateStandardArms(){ ReleaseBoomerang(); }
    protected override void ActivateChargedArms(){ ReleaseBoomerang(); }

    protected override void BeginChargingLegs(){ }
    protected override void RunChargingLegs(){ }
    protected override void ActivateChargedLegs(){ }
    protected override void ActivateStandardLegs(){ }


    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable){                
        base.AttachAffect(ref wielderStats, wielderMovable);
        pickUpScale=transform.localScale;
    }

    public override void DeActivate()
    {
        energySettings.isActive = false;
        damageCollider.enabled = false;        
        returning = false;
        transform.localScale = initialScale;
        transform.parent = modSocket;
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
    private void GrowSize() {
        transform.localScale = pickUpScale * (1+ (chargedScale-1.8f) * energySettings.chargeFraction);
    }
    private void ReleaseBoomerang(){
        energySettings.isActive = true;
        damageCollider.enabled = true;
        initialPosition = transform.position;
        transform.rotation = Quaternion.identity;
        modSocket = transform.parent;
        transform.parent.DetachChildren();
        if (IsCharged()) {
            transform.localScale = initialScale * chargedScale;
        }
        TRMatrix = Matrix4x4.TRS(initialPosition, wielderMovable.GetRotation(), Vector3.one);
        transform.forward = wielderMovable.GetForward();
        majorAxisRadius = enemyDistance > maxDistance ? maxDistance / 2.0f : (enemyDistance+0.2f) / 2.0f;
        majorAxisRadius = Mathf.Clamp(majorAxisRadius, 0f, maxDistance);
    }
    void UpdateBoomerangPosition(int leftHandMultiplier = 1){
        //Equation of an ellipse x^2/a^2 + y^2/b^2 = 1
        Vector3 relativePosition = TRMatrix.inverse.MultiplyPoint3x4(transform.position);
        float xCoordinate = relativePosition.x;
        float yCoordinate = relativePosition.y;
        float zCoordinate = relativePosition.z;
        float zCoordinateCompletion = Mathf.Abs(zCoordinate / (majorAxisRadius*2));
        float movementSpeed = (returning ? -1 : 1) * (minMovementSpeed + maxMovementSpeed * (1.0f-zCoordinateCompletion));
        zCoordinate += movementSpeed * Time.deltaTime;
        if (zCoordinate > majorAxisRadius * 2f){
            returning = true;
            zCoordinate = majorAxisRadius * 2f;            
            float relativeZ = zCoordinate - majorAxisRadius;
            xCoordinate = GetXCoordinate(relativeZ);
            float relativeX = -xCoordinate * leftHandMultiplier;
            transform.position = TRMatrix.MultiplyPoint3x4(new Vector3(relativeX, yCoordinate, zCoordinate));
        }
        else if (zCoordinate < 0.0f){
            zCoordinate = 0.0f;            
            DeActivate();
        }
        else{
            float relativeZ = zCoordinate - majorAxisRadius;
            xCoordinate = GetXCoordinate(relativeZ);
            if (returning){
                xCoordinate = -xCoordinate;
            }
            float relativeX = xCoordinate * leftHandMultiplier;
            transform.position = TRMatrix.MultiplyPoint3x4(new Vector3(relativeX, yCoordinate, zCoordinate));
        }

        float rotationSpeed = zCoordinateCompletion * maxRotationRate + minRotationRate;
        transform.Rotate(Vector3.up * rotationSpeed);
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
