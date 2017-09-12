using MovementEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerMod : Mod {

    #region Public fields
    public IMovable WielderMovable { get { return wielderMovable; } }
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private Hook hook;    
    [SerializeField] private float maxHookLengthStandard;
    #endregion

    #region Private Fields
    private bool hitTargetThisShot;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    protected override void Awake() {
        type = ModType.Grappler;
        category = ModCategory.Ranged;
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update () {
        if (wielderMovable != null)
        {
            Vector3 forward = wielderMovable.GetForward().normalized;
            forward.y = 0f;
            transform.forward = forward;
        }        
	}
    #endregion

    #region Public Methods
    public override void Activate(Action onCompleteCoolDown=null, Action onActivate=null)
    {  
        onActivate = () => {
            hook.maxLength = maxHookLengthStandard;
            SFXManager.Instance.Play(SFXType.GrapplingGun_Shoot, transform.position);
        };   
        base.Activate(onCompleteCoolDown, onActivate);
    }
    
    protected override void ActivateStandardArms(){ hook.Throw(false); }    

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable){
        base.AttachAffect(ref wielderStats, wielderMovable);
        if (wielderStats.gameObject.CompareTag(Strings.Tags.PLAYER))
        {
            hook.SetShooterType(true);
        }
        else
        {
            hook.SetShooterType(false);
        }
    }

    public override void DeActivate()
    {
        
    }

    public override void DetachAffect(){
        base.DetachAffect();
    }

    public bool GetHitTargetThisShot() { return hitTargetThisShot; }
    public void SetHitTargetThisShot(bool hitTarget) { hitTargetThisShot = hitTarget;}
    #endregion

    #region Private Methods
    #endregion

    #region Private Structures
    #endregion

}
