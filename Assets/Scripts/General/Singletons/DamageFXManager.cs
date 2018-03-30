using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using ModMan;

public class DamageFXManager : Singleton<DamageFXManager> {
    
    protected DamageFXManager() { }
    private Dictionary<DamagedType, Dictionary<DamagerType, System.Action<DamagePack>>> RedirectionEvents;

    protected override void Start() {
        base.Start();
        RedirectionEvents = new Dictionary<DamagedType, Dictionary<DamagerType, System.Action<DamagePack>>>() {
            {DamagedType.MallCop,
                new Dictionary<DamagerType, System.Action<DamagePack>>() {
                    {DamagerType.SegwayPush, EmitDirectionalBlood },
                    {DamagerType.BlasterBullet, EmitDirectionalBlood },
                    {DamagerType.SpreadGun, EmitDirectionalBlood },
                    {DamagerType.TankTreads, EmitDirectionalBlood },
                    {DamagerType.GrapplingHook, EmitDirectionalBlood },
                    {DamagerType.Boomerang, EmitDirectionalBlood },
                    {DamagerType.Geyser, EmitDirectionalBlood },
                    //{DamagerType.StunMine, EmitGeyser },
                    {DamagerType.FireTrap, EmitDirectionalBlood },
                    {DamagerType.GrapplingBotExplosion, EmitDirectionalBlood },
                    {DamagerType.Dice, EmitDirectionalBlood },
                    //{DamagerType.StunStomp, EmitGeyser}
                }
            },
            {DamagedType.Milo,
                new Dictionary<DamagerType, System.Action<DamagePack>>() {
                    {DamagerType.SegwayPush, EmitDirectionalBlood },
                    {DamagerType.BlasterBullet, EmitDirectionalBlood },
                    {DamagerType.SpreadGun, EmitDirectionalBlood },
                    {DamagerType.TankTreads, EmitDirectionalBlood },
                    {DamagerType.GrapplingHook, EmitDirectionalBlood },
                    {DamagerType.Boomerang, EmitDirectionalBlood },
                    {DamagerType.Geyser, EmitDirectionalBlood },
                    //{DamagerType.StunMine, EmitGeyser },
                    {DamagerType.FireTrap, EmitDirectionalBlood },
                    {DamagerType.GrapplingBotExplosion, EmitDirectionalBlood },
                    {DamagerType.Dice, EmitDirectionalBlood },
                    //{DamagerType.StunStomp, EmitGeyser}
                }
            },
            {DamagedType.Zombie,
                new Dictionary<DamagerType, System.Action<DamagePack>>() {
                    {DamagerType.SegwayPush, EmitDirectionalBlood },
                    {DamagerType.BlasterBullet, EmitDirectionalBlood },
                    {DamagerType.SpreadGun, EmitDirectionalBlood },
                    {DamagerType.TankTreads, EmitDirectionalBlood },
                    {DamagerType.GrapplingHook, EmitDirectionalBlood },
                    {DamagerType.Boomerang, EmitDirectionalBlood },
                    {DamagerType.Geyser, EmitDirectionalBlood },
                    //{DamagerType.StunMine, EmitGeyser },
                    {DamagerType.FireTrap, EmitDirectionalBlood },
                    {DamagerType.GrapplingBotExplosion, EmitDirectionalBlood },
                    {DamagerType.Dice, EmitDirectionalBlood },
                    //{DamagerType.StunStomp, EmitGeyser}
                }
            },
            {DamagedType.Bouncer,
                new Dictionary<DamagerType, System.Action<DamagePack>>() {
                    {DamagerType.SegwayPush, EmitDirectionalBlood },
                    {DamagerType.BlasterBullet, EmitDirectionalBlood },
                    {DamagerType.SpreadGun, EmitDirectionalBlood },
                    {DamagerType.TankTreads, EmitDirectionalBlood },
                    {DamagerType.GrapplingHook, EmitDirectionalBlood },
                    {DamagerType.Boomerang, EmitDirectionalBlood },
                    {DamagerType.Geyser, EmitDirectionalBlood },
                    //{DamagerType.StunMine, EmitGeyser },
                    {DamagerType.FireTrap, EmitDirectionalBlood },
                    {DamagerType.GrapplingBotExplosion, EmitDirectionalBlood },
                    {DamagerType.Dice, EmitDirectionalBlood },
                    //{DamagerType.StunStomp, EmitGeyser}
                }
            },
            {DamagedType.Kamikaze,
                new Dictionary<DamagerType, System.Action<DamagePack>>() {
                    {DamagerType.SegwayPush, EmitDirectionalBlood },
                    {DamagerType.BlasterBullet, EmitDirectionalBlood },
                    {DamagerType.SpreadGun, EmitDirectionalBlood },
                    {DamagerType.TankTreads, EmitDirectionalBlood },
                    {DamagerType.GrapplingHook, EmitDirectionalBlood },
                    {DamagerType.Boomerang, EmitDirectionalBlood },
                    {DamagerType.Geyser, EmitDirectionalBlood },
                    //{DamagerType.StunMine, EmitGeyser },
                    {DamagerType.FireTrap, EmitDirectionalBlood },
                    {DamagerType.GrapplingBotExplosion, EmitDirectionalBlood },
                    {DamagerType.Dice, EmitDirectionalBlood },
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

    #region Private Interface

    private void EmitDirectionalBlood(DamagePack pack) {
        // Determine Position to emit blood from:
        Vector3 position = pack.damaged.owner.position;
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, 5F, LayerMasker.GetLayerMask(Layers.Ground)))
        {
            position = hit.point;
        } else
        {
            // We'll set the 'y' to zero which should be near the floor...
            position.y = 0;
        }

        // Obtain lateral impact direction
        Vector3 impactDir = pack.damager.impactDirection;
        Vector2 projectileDir = new Vector2(impactDir.x, impactDir.z);

        // Queue Up Blood with the GoreManager
        //GoreManager.Instance.QueueSplat(position, projectileDir);
    }

    #endregion
}