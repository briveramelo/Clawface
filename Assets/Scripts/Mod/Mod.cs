﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mod : MonoBehaviour {

    ModSpot spot;
    protected ModType type;

    [SerializeField]
    public Collider pickupCollider;

    protected Stats playerStats;
    protected List<IDamageable> recentlyHitEnemies = new List<IDamageable>();

    public abstract void Activate();

    public abstract void DeActivate();

    public abstract void AttachAffect(ref Stats playerStats, ref PlayerMovement playerMovement);

    public abstract void DetachAffect();

    public void setModType(ModType modType)
    {
        type = modType;
    }

    public ModType getModType()
    {
        return type;
    }

    public void setModSpot(ModSpot modSpot)
    {
        spot = modSpot;
    }

    public ModSpot getModSpot()
    {
        return spot;
    }
}
