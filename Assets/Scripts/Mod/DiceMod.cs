using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceMod : Mod {

    #region Serialized Fields
    [SerializeField]
    private VFXBlasterShoot blasterEffect;
    [SerializeField]
    private Transform bulletSpawnPoint;
    #endregion

    #region Unity Lifetime
    // Use this for initialization
    void Start () {
        type = ModType.Dice;
        category = ModCategory.Ranged;
	}
	
	// Update is called once per frame
	protected override void Update () {
        if (wielderMovable != null)
        {
            if (getModSpot() != ModSpot.Legs)
            {
                transform.forward = wielderMovable.GetForward();
            }
        }
    }
    #endregion

    #region Public Methods
    public override void Activate(Action onComplete = null)
    {
        base.Activate(onComplete);
    }


    public override void DeActivate()
    {
        throw new NotImplementedException();
    }

    protected override void ActivateChargedArms()
    {
        throw new NotImplementedException();
    }

    protected override void ActivateChargedLegs()
    {
        throw new NotImplementedException();
    }

    protected override void ActivateStandardArms()
    {
        throw new NotImplementedException();
    }

    protected override void ActivateStandardLegs()
    {
        throw new NotImplementedException();
    }

    protected override void BeginChargingArms()
    {
        throw new NotImplementedException();
    }

    protected override void BeginChargingLegs()
    {
        throw new NotImplementedException();
    }

    protected override void RunChargingArms()
    {
        throw new NotImplementedException();
    }

    protected override void RunChargingLegs()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Private Methods
    private DiceBlock SpawnDice()
    {
        DiceBlock diceBlock = ObjectPool.Instance.GetObject(PoolObjectType.DiceBlock).GetComponent<DiceBlock>();
        if (diceBlock)
        {
            diceBlock.transform.position = bulletSpawnPoint.position;
            diceBlock.transform.rotation = transform.rotation;
        }
        return diceBlock;
    }

    #endregion

}
