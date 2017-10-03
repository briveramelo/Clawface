using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using ModMan;

public class DamageFXManager : Singleton<DamageFXManager> {
    
    protected DamageFXManager() { }
    private Dictionary<DamagedType, Dictionary<DamagerType, System.Action<DamagePack>>> RedirectionEvents;

    private void Start() {
        RedirectionEvents = new Dictionary<DamagedType, Dictionary<DamagerType, System.Action<DamagePack>>>() {
            {DamagedType.MallCop,
                new Dictionary<DamagerType, System.Action<DamagePack>>() {
                    {DamagerType.SegwayPush, EmitBlood },
                    {DamagerType.BlasterBullet, EmitBlood },
                    {DamagerType.SpreadGun, EmitBlood },
                    {DamagerType.TankTreads, EmitBlood },
                    {DamagerType.GrapplingHook, EmitBlood },
                    {DamagerType.Boomerang, EmitBlood },
                    {DamagerType.Geyser, EmitGeyser },
                    //{DamagerType.StunMine, EmitGeyser },
                    {DamagerType.FireTrap, EmitBlood },
                    {DamagerType.GrapplingBotExplosion, EmitBlood },
                    {DamagerType.Dice, EmitGeyser },
                    //{DamagerType.StunStomp, EmitGeyser}
                }
            },
            {DamagedType.Milo,
                new Dictionary<DamagerType, System.Action<DamagePack>>() {
                    {DamagerType.SegwayPush, EmitBlood },
                    {DamagerType.BlasterBullet, EmitBlood },
                    {DamagerType.SpreadGun, EmitBlood },
                    {DamagerType.TankTreads, EmitBlood },
                    {DamagerType.GrapplingHook, EmitBlood },
                    {DamagerType.Boomerang, EmitBlood },
                    {DamagerType.Geyser, EmitGeyser },
                    //{DamagerType.StunMine, EmitGeyser },
                    {DamagerType.FireTrap, EmitBlood },
                    {DamagerType.GrapplingBotExplosion, EmitBlood },
                    {DamagerType.Dice, EmitGeyser },
                    //{DamagerType.StunStomp, EmitGeyser}
                }
            },            
        };
    }

    #region THE Public Function
    public void EmitDamageEffect(DamagePack damagePack) {
        string debugMessage = "";
        DamagedType damagedType = damagePack.damaged.damageType;
        if (RedirectionEvents.ContainsKey(damagedType)) {
            DamagerType damagerType = damagePack.damager.damagerType;
            if (RedirectionEvents[damagedType].ContainsKey(damagerType)) {
                RedirectionEvents[damagedType][damagerType](damagePack);
                return;
            }
            debugMessage = damagedType + " has no VFX function for " + damagerType + " loaded. Please add this.";            
        }
        else {
            debugMessage = damagedType + " has no VFX functions loaded. Please add them.";
        }
        Debug.LogFormat("<color=#0000FF>" + debugMessage + "</color>");
    }
    #endregion

    void EmitBlood(DamagePack dPack) {        
        if (Mathf.Abs(dPack.damager.impactDirection.y) < 0.5f) {
            EmitBloodBilaterally(dPack.damaged);
        }
        else {            
            Vector3 bloodDirection = dPack.damager.impactDirection;
            bloodDirection.x = 23.38f;
            EmitBloodInDirection(bloodDirection, dPack.damaged.owner.position);
        }    
    }
    void EmitGeyser(DamagePack dPack) {
        EmitBloodInDirection(dPack.damager.impactDirection, dPack.damaged.owner.position);
    }
    
    #region Private Helper Functions
    private void EmitBloodBilaterally(Damaged damaged) {
        SetupBlood(ObjectPool.Instance.GetObject(PoolObjectType.VFXBloodEmitter), damaged, true);
        SetupBlood(ObjectPool.Instance.GetObject(PoolObjectType.VFXBloodEmitter), damaged, false);
    }

    private void SetupBlood(GameObject emitter, Damaged damaged, bool facingFront){        
        if (emitter != null){ // && damaged.owner!=null
            emitter.transform.position = damaged.owner.position;
            Vector3 bulletAngs = damaged.owner.rotation.eulerAngles + (facingFront ? Vector3.zero : Vector3.up * 180f);
            Vector3 projectileAngs = emitter.transform.rotation.eulerAngles;
            emitter.transform.rotation = Quaternion.Euler(projectileAngs.x, bulletAngs.y, 0f);
        }
    }

    private void EmitBloodInDirection(Vector3 emissionDirection, Vector3 spawnPoint) {
        GameObject bloodEmitter = ObjectPool.Instance.GetObject(PoolObjectType.VFXBloodEmitter);
        if (bloodEmitter) {
            bloodEmitter.transform.ResetRotation(3f);
            bloodEmitter.transform.position = spawnPoint;
            bloodEmitter.transform.rotation = Quaternion.LookRotation(emissionDirection);
        }        
    }

    #endregion
}