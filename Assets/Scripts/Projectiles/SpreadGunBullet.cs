﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SpreadGunBullet : MonoBehaviour {

    #region Serialized Unity Inspector Fields

    [SerializeField] PoolObjectType impactVFX = PoolObjectType.VFXSpreadshotImpact;

    #endregion

    #region Private variables
    private SpreadGun.SpreadGunProperties properties;
    private bool isReady;
    private Vector3 initPosition;
    private float currentDamage;
    private float maxAlpha;
    private Material material;
    private Color oldColor;
    #endregion

    #region Unity lifecycle
    private void Awake()
    {
        Assert.IsNotNull(GetComponent<MeshRenderer>());
        material = GetComponent<MeshRenderer>().material;
        oldColor = material.GetColor("_TintColor");
        maxAlpha = oldColor.a;
    }

    // Use this for initialization
    void Start () {
        currentDamage = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (isReady)
        {
            //Move and shit
            transform.position += transform.forward * properties.bulletSpeed * Time.deltaTime;
            float distanceTravelled = Vector3.Distance(initPosition, transform.position);
            if(distanceTravelled > properties.bulletMaxDistance)
            {
                ResetBullet();
            }
            else
            {
                float sampleRatio = distanceTravelled / properties.bulletMaxDistance;
                float currentScale = Mathf.Lerp(properties.bulletMinScale, properties.bulletMaxScale, sampleRatio);
                transform.localScale = new Vector3(currentScale, transform.localScale.y, -currentScale);

                currentDamage = properties.bulletMinDamage + (properties.bulletMaxDamage - properties.bulletMinDamage) * (1 - sampleRatio);

                //currentDamage = Mathf.Lerp(properties.bulletMaxDamage, properties.bulletMinDamage, sampleRatio);
                float currentAlpha = Mathf.Lerp(maxAlpha, properties.bulletMinAlpha, sampleRatio);                
                Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, currentAlpha);
                material.SetColor("_TintColor", newColor);
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {        
        if(other.tag == Strings.Tags.ENEMY)
        {
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Damager damager = new Damager();
                // HACK - This isn't perfect.. but I think it will suffice for our type of gameplay.
                damager.Set(currentDamage, DamagerType.SpreadGun, transform.forward);
                damageable.TakeDamage(damager);
            }
            GameObject vfx = ObjectPool.Instance.GetObject(impactVFX);
            if (vfx) {
                vfx.transform.position = other.transform.position;
            }
        }else if(other.tag == Strings.Tags.WALL)
        {
            GameObject vfx = ObjectPool.Instance.GetObject(impactVFX);
            if (vfx) {
                vfx.transform.position = other.transform.position;
            }
            gameObject.SetActive(false);
        }
    }
    #endregion

    #region Public functions
    public void Init(SpreadGun.SpreadGunProperties properties)
    {
        this.properties = properties;
        transform.localScale = new Vector3(properties.bulletMinScale, transform.localScale.y, -properties.bulletMinScale);
        isReady = true;
        initPosition = transform.position;
    }
    #endregion

    #region Private functions
    private void ResetBullet()
    {        
        isReady = false;
        gameObject.SetActive(false);
        transform.SetParent(ObjectPool.Instance.transform);
        currentDamage = 0;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, maxAlpha);
        material.SetColor("_TintColor", newColor);
    }
    #endregion

}
