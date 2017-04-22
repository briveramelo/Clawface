﻿/**
 *  @author Cornelia Schultz
 */

using UnityEngine;
using UnityEngine.Assertions;

public class HealthBar : MonoBehaviour {
    

    #region Unity Inspector Fields
    [SerializeField]
    private Transform mask, bar;
    #endregion

    #region Public Interface
    public void SetHealth(float health)
    {
        Assert.IsTrue(health >= 0.0F && health <= 1.0F);
        mask.localScale = new Vector3(health, 1.0F, 1.0F);
        bar.localScale = new Vector3(health == 0 ? 0 : 1 / health, 1.0F, 1.0F);
    }
    #endregion

    #region Protected Interface
    protected HealthBar() { }
    #endregion
}
