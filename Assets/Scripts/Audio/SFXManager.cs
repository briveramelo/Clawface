using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    Dictionary<SFXType, SoundEffect> SFD = new Dictionary<SFXType, SoundEffect>();

    #region SFX Object
    [SerializeField]
    private GameObject BloodExplosion;
    [SerializeField]
    private GameObject BlasterCharge;
    [SerializeField]
    private GameObject BlasterProjectile_Impact;
    [SerializeField]
    private GameObject BlasterShoot;
    [SerializeField]
    private GameObject GrapplingGun_Shoot;
    [SerializeField]
    private GameObject StunBatonCharge;
    [SerializeField]
    private GameObject StunBatonImpact;
    [SerializeField]
    private GameObject StunBatonSwing;
    [SerializeField]
    private GameObject TankTreads_Attack;
    [SerializeField]
    private GameObject TankTreads_Swing;
    #endregion

    private void Start()
    {
        SoundEffect SFX = new SoundEffect(BloodExplosion);
        SFD.Add(SFXType.BloodExplosion, SFX);

        SFX = new SoundEffect(BlasterCharge);
        SFD.Add(SFXType.BlasterCharge, SFX);

        SFX = new SoundEffect(BlasterProjectile_Impact);
        SFD.Add(SFXType.BlasterProjectileImpact, SFX);

        SFX = new SoundEffect(BlasterShoot);
        SFD.Add(SFXType.BlasterShoot, SFX);

        SFX = new SoundEffect(GrapplingGun_Shoot);
        SFD.Add(SFXType.GrapplingGun_Shoot, SFX);

        SFX = new SoundEffect(StunBatonCharge);
        SFD.Add(SFXType.StunBatonCharge, SFX);

        SFX = new SoundEffect(StunBatonImpact);
        SFD.Add(SFXType.StunBatonImpact, SFX);

        SFX = new SoundEffect(StunBatonSwing);
        SFD.Add(SFXType.StunBatonSwing, SFX);

        SFX = new SoundEffect(TankTreads_Attack);
        SFD.Add(SFXType.TankTreads_Attack, SFX);

        SFX = new SoundEffect(TankTreads_Swing);
        SFD.Add(SFXType.TankTreads_Swing, SFX);

    }

    public void Play(SFXType i_Type)
    {
        SFD[i_Type].Play();
    }
}
