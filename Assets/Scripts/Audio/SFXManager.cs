using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : Singleton<SFXManager>
{
    protected SFXManager() { }
    private Dictionary<SFXType, SoundEffect> sfxDictionary;

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
    [SerializeField]
    private GameObject Dash;
    [SerializeField]
    private GameObject SegwayBlast_Standard;
    #endregion

    private void Start()
    {
        sfxDictionary = new Dictionary<SFXType, SoundEffect>()
        {
            {SFXType.BloodExplosion, new SoundEffect(Instantiate(BloodExplosion)) },
            {SFXType.BlasterCharge, new SoundEffect(Instantiate(BlasterCharge)) },
            {SFXType.BlasterProjectileImpact, new SoundEffect(Instantiate(BlasterProjectile_Impact)) },
            {SFXType.BlasterShoot, new SoundEffect(Instantiate(BlasterShoot)) },
            {SFXType.GrapplingGun_Shoot, new SoundEffect(Instantiate(GrapplingGun_Shoot)) },
            {SFXType.StunBatonCharge, new SoundEffect(Instantiate(StunBatonCharge)) },
            {SFXType.StunBatonImpact, new SoundEffect(Instantiate(StunBatonImpact)) },
            {SFXType.StunBatonSwing, new SoundEffect(Instantiate(StunBatonSwing)) },
            {SFXType.TankTreads_Attack, new SoundEffect(Instantiate(TankTreads_Attack)) },
            {SFXType.TankTreads_Swing, new SoundEffect(Instantiate(TankTreads_Swing)) },
            {SFXType.Dash, new SoundEffect(Instantiate(Dash)) },
            {SFXType.SegwayBlast_Standard, new SoundEffect(Instantiate(SegwayBlast_Standard)) }
        };
        foreach(KeyValuePair<SFXType, SoundEffect> kp in sfxDictionary) {
            kp.Value.SetParent(transform);
        }      
    }

    public void Play(SFXType i_Type, Vector3 position)
    {
        if (sfxDictionary.ContainsKey(i_Type)) {
            sfxDictionary[i_Type].Play(position);
            return;
        }
        string message="No SFX Found for " + i_Type + ". Please add.";
        Debug.LogFormat("<color=#0000FF>" + message + "</color>");
        
    }

    public void Stop(SFXType i_Type) {
        if (sfxDictionary.ContainsKey(i_Type)) {
            sfxDictionary[i_Type].Stop();
            return;
        }
        string message="No SFX Found for " + i_Type + ". Please add.";
        Debug.LogFormat("<color=#0000FF>" + message + "</color>");
    }
}
