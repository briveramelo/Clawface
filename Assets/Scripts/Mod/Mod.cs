using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mod : MonoBehaviour {

    #region Public fields
    #endregion

    #region Protected Fields
    protected ModType type;
    protected ModCategory category;
    public bool hasState;
    protected Stats wielderStats;
    protected IMovable wielderMovable;
    protected List<IDamageable> recentlyHitEnemies = new List<IDamageable>();
    protected int GetWielderInstanceID() {
        if (wielderStats != null) {
            return wielderStats.gameObject.GetInstanceID();
        }
        return 0;
    }

    protected bool isAttached;
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    public Collider pickupCollider;
    public GameObject modCanvas;
    #endregion

    #region Private Fields
    ModSpot spot;
    #endregion

    #region Unity Lifecycle
    #endregion

    #region Public Methods
    public abstract void Activate();

    public abstract void AlternateActivate(bool isHeld, float holdTime);

    public abstract void DeActivate();

    public abstract void AttachAffect(ref Stats wielderStats, IMovable wielderMovable);

    public abstract void DetachAffect();

    public abstract void ActivateModCanvas();

    public abstract void DeactivateModCanvas();

    public void setModType(ModType modType)
    {
        type = modType;
    }

    public ModType getModType()
    {
        return type;
    }

    public ModCategory getModCategory()
    {
        return category;
    }

    public void setModSpot(ModSpot modSpot)
    {
        spot = modSpot;
    }

    public ModSpot getModSpot()
    {
        return spot;
    }
    #endregion

    #region Private Methods
    #endregion

    #region Private Structures
    #endregion

}
