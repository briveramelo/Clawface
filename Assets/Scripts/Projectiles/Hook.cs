﻿using System.Collections;
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
    [SerializeField] private float pullForce;
    #endregion

    #region Private Fields
    private Vector3 initPos;
    private bool isCharged;
    private bool isPullingWielder;
    private bool isThrowing;
    private bool isRetracting;
    private Vector3 pullDirection;
    private Damager damager=new Damager();
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        initPos = transform.localPosition;
    }

    private void OnTriggerEnter(Collider other){   
        if ((other.gameObject.CompareTag(Strings.Tags.PLAYER) ||
            other.gameObject.CompareTag(Strings.Tags.ENEMY) ||
            other.gameObject.layer==(int)Layers.Ground) && 
            mod.getModSpot()!= ModSpot.Legs){


            if (isThrowing)
            {
                HitTarget();
                IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    if (this.transform.root.CompareTag(Strings.Tags.PLAYER))
                    {
                        AnalyticsManager.Instance.AddModDamage(ModType.Grappler, mod.Attack);

                        if (damageable.GetHealth() - mod.Attack <= 0.01f)
                        {
                            AnalyticsManager.Instance.AddModKill(ModType.Grappler);
                        }
                    }
                    else if (this.transform.root.CompareTag(Strings.Tags.ENEMY))
                    {
                        AnalyticsManager.Instance.AddEnemyModDamage(ModType.Grappler, mod.Attack);
                    }
                    
                    damager.Set(mod.Attack, DamagerType.GrapplingHook, transform.forward);
                    damageable.TakeDamage(damager);
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
        while (transform.localPosition.z < maxLength && isThrowing){
            //transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z + growRate * Time.deltaTime);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + growRate * Time.deltaTime);                          
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
        while (transform.localPosition.z > initPos.z){
            if (isPullingWielder) {
                PullToTarget();
            }
            //transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z - shrinkRate * Time.deltaTime);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - shrinkRate * Time.deltaTime);
            yield return 0f;
        }      
        FinishRetracting();        
    }

    #endregion

    #region Private Methods
    void FinishRetracting(){
        transform.localPosition = initPos;
        isPullingWielder = false;
    }        
    #endregion

    #region Private Structures
    #endregion

}
