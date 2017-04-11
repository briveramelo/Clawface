using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangMod : Mod {


    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private float maxDistance;
    [SerializeField] private float maxWidth;
    [SerializeField] private SphereCollider damageCollider;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float growRate;
    #endregion

    #region Private Fields
    private bool isActive;
    private Vector3 initialPosition;
    private Transform modSocket;
    private bool returning;
    private Matrix4x4 TRMatrix;
    private float enemyDistance;
    private float minorAxisLength;
    private float majorAxisLength;
    private Vector3 initialScale;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        type = ModType.Boomerang;
        category = ModCategory.Ranged;
        damageCollider.enabled = false;
        enemyDistance = Mathf.Infinity;
        minorAxisLength = maxWidth;
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
        if (isActive) {
            if (getModSpot() == ModSpot.ArmR)
            {
                UpdateBoomerangPosition();
            }else if (getModSpot() == ModSpot.ArmL)
            {
                UpdateBoomerangPosition(-1);
            }
        }
        if (energySettings.isCharging && !isActive){ 
            transform.localScale = Vector3.Lerp(transform.localScale, initialScale * 2.0f, growRate);
        }
	}    

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetInstanceID()!=GetWielderInstanceID() && isActive){
            if(other.gameObject.CompareTag(Strings.Tags.ENEMY) || other.gameObject.CompareTag(Strings.Tags.PLAYER)){
                IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
                if (damageable!=null){ 
                    damageable.TakeDamage(attack);
                }
            }            
        }
    }
    #endregion

    #region Public Methods
    public bool IsActive() { return isActive; }

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
        initialScale = transform.localScale;
    }

    public override void DeActivate()
    {
        isActive = false;
        damageCollider.enabled = false;
        movementSpeed = -movementSpeed;
        returning = false;
        transform.parent = modSocket;
        transform.localPosition = Vector3.zero;
        initialPosition = Vector3.zero;        
        transform.forward = wielderMovable.GetForward();
        transform.localScale = initialScale;
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
        transform.localScale = Vector3.Lerp(transform.localScale, initialScale * 2.0f, growRate);
    }
    private void ReleaseBoomerang(){
        isActive = true;
        damageCollider.enabled = true;
        initialPosition = transform.position;
        transform.rotation = Quaternion.identity;
        modSocket = transform.parent;
        transform.parent.DetachChildren();
        TRMatrix = Matrix4x4.TRS(initialPosition, wielderMovable.GetRotation(), Vector3.one);
        transform.forward = wielderMovable.GetForward();
        majorAxisLength = enemyDistance > maxDistance ? maxDistance / 2.0f : enemyDistance / 2.0f;
    }
    void UpdateBoomerangPosition(int leftHandMultiplier = 1){
        //Equation of an ellipse x^2/a^2 + y^2/b^2 = 1
        Vector3 relativePosition = TRMatrix.inverse.MultiplyPoint3x4(transform.position);
        float xCoordinate = relativePosition.x;
        float yCoordinate = relativePosition.y;
        float zCoordinate = relativePosition.z;
        zCoordinate += movementSpeed * Time.deltaTime;
        if (zCoordinate > majorAxisLength * 2.0f){
            returning = true;
            zCoordinate = majorAxisLength * 2.0f;
            movementSpeed = -movementSpeed;
            float relativeZ = zCoordinate - majorAxisLength;
            xCoordinate = (Mathf.Sqrt(1 - ((relativeZ) * (relativeZ)) / (majorAxisLength * majorAxisLength)) * minorAxisLength);
            float relativeX = -xCoordinate * leftHandMultiplier;
            transform.position = TRMatrix.MultiplyPoint3x4(new Vector3(relativeX, yCoordinate, zCoordinate));
        }
        else if (zCoordinate < 0.0f){
            zCoordinate = 0.0f;            
            DeActivate();
        }
        else{
            float relativeZ = zCoordinate - majorAxisLength;
            xCoordinate = (Mathf.Sqrt(1 - ((relativeZ) * (relativeZ)) / (majorAxisLength * majorAxisLength)) * minorAxisLength);
            if (returning){
                xCoordinate = -xCoordinate;
            }
            float relativeX = xCoordinate * leftHandMultiplier;
            transform.position = TRMatrix.MultiplyPoint3x4(new Vector3(relativeX, yCoordinate, zCoordinate));
        }
    }
    #endregion

    #region Private Structures
    #endregion

}
