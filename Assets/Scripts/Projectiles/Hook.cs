using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class Hook : MonoBehaviour {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] GrapplerMod mod;
    [SerializeField] private float growRate;
    [SerializeField] private float shrinkRate;
    [SerializeField] private float maxLength;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float pullForce;
    #endregion

    #region Private Fields
    private float initSize;
    private Vector3 initPos;
    private bool isCharged;
    private bool isPullingWielder;
    private bool isThrowing;
    private bool isRetracting;
    private Vector3 pullDirection;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        initSize = transform.localScale.z;
        initPos = transform.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {   
        if ((other.gameObject.CompareTag(Strings.Tags.PLAYER) ||
            other.gameObject.CompareTag(Strings.Tags.ENEMY) ||
            other.gameObject.layer==(int)Layers.Ground) && 
            mod.getModSpot()!= ModSpot.Legs){

            if (isThrowing){
                HitTarget();
                IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
                if (damageable != null){
                    damageable.TakeDamage(mod.attack);
                }
            }            
        }
    }    
    #endregion

    #region Public Methods
    public void Throw(bool isCharged){
        this.isCharged = isCharged;
        Timing.RunCoroutine(ThrowHook());
    }
    IEnumerator<float> ThrowHook() {
        isThrowing= true;
        while (transform.localScale.z < maxLength && isThrowing){
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z + growRate * Time.deltaTime);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + growRate * Time.deltaTime / 2.0f);                          
            yield return 0f;
        }
        isThrowing= false;
        Timing.RunCoroutine(Retract());
    }

    void HitTarget(){
        isThrowing = false;
        mod.SetHitTargetThisShot(true);
        if (isCharged) {
            isPullingWielder = true;
            pullDirection = mod.WielderMovable.GetForward();
        }
    }

    private void PullToTarget() {
        mod.WielderMovable.AddDecayingForce(pullDirection * pullForce);
    }

    private IEnumerator<float> Retract(){        
        while (transform.localScale.z > initSize){
            if (isPullingWielder) {
                PullToTarget();
            }
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z - shrinkRate * Time.deltaTime);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - shrinkRate * Time.deltaTime / 2.0f);
            yield return 0f;
        }      
        FinishRetracting();        
    }

    #endregion

    #region Private Methods
    void FinishRetracting(){
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, initSize);
        transform.localPosition = initPos;
        isPullingWielder = false;
    }        
    #endregion

    #region Private Structures
    #endregion

}
