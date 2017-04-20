using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : Singleton<SFXManager>
{
    protected SFXManager() { }
    private Dictionary<SFXType, List<SoundEffect>> sfxDictionary;

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
        sfxDictionary = new Dictionary<SFXType, List<SoundEffect>>()
        {
            {SFXType.BloodExplosion,            InitList(BloodExplosion) },
            {SFXType.BlasterCharge,             InitList(BlasterCharge) },
            {SFXType.BlasterProjectileImpact,   InitList(BlasterProjectile_Impact) },
            {SFXType.BlasterShoot,              InitList(BlasterShoot) },
            {SFXType.GrapplingGun_Shoot,        InitList(GrapplingGun_Shoot) },
            {SFXType.StunBatonCharge,           InitList(StunBatonCharge) },
            {SFXType.StunBatonImpact,           InitList(StunBatonImpact) },
            {SFXType.StunBatonSwing,            InitList(StunBatonSwing) },
            {SFXType.TankTreads_Attack,         InitList(TankTreads_Attack) },
            {SFXType.TankTreads_Swing,          InitList(TankTreads_Swing) },
            {SFXType.Dash,                      InitList(Dash) },
            {SFXType.SegwayBlast_Standard,      InitList(SegwayBlast_Standard) }
        };
    }

    public void Play(SFXType i_Type, Vector3 position)
    {
        if (sfxDictionary.ContainsKey(i_Type))
        {
            foreach(SoundEffect SFX in sfxDictionary[i_Type])
            {
                if(SFX.Available)
                {
                    SFX.Available = false;
                    SFX.Play(position);
                    StartCoroutine(WaitForReturnList(SFX));
                    return;
                }
            }
        }

        string message="No SFX Found for " + i_Type + ". Please add.";
        Debug.LogFormat("<color=#0000FF>" + message + "</color>");
    }

    
    public void PlayFollowObject(SFXType i_Type, Transform i_parents)
    {
        if (sfxDictionary.ContainsKey(i_Type))
        {
            foreach (SoundEffect SFX in sfxDictionary[i_Type])
            {
                if (SFX.Available)
                {
                    SFX.Available = false;
                    SFX.PlayFollowObject(i_parents);
                    StartCoroutine(WaitForReturnList(SFX));
                    return;
                }
            }

        }

        string message = "No SFX Found for " + i_Type + ". Please add.";
        Debug.LogFormat("<color=#0000FF>" + message + "</color>");
    }
    
    
    public void Stop(SFXType i_Type)
    {
        if (sfxDictionary.ContainsKey(i_Type))
        {
            foreach (SoundEffect SFX in sfxDictionary[i_Type])
            {
                if (!SFX.Available)
                {
                    SFX.Stop();
                    return;
                }
            }
        }

        string message="No SFX Found for " + i_Type + ". Please add.";
        Debug.LogFormat("<color=#0000FF>" + message + "</color>");
    }
    

    private List<SoundEffect> InitList(GameObject i_SFX)
    {
        List<SoundEffect> List = new List<SoundEffect>();
        int numSFX = 10;

        for(int i = 0; i < numSFX; i++)
        {
            List.Add(new SoundEffect(Instantiate(i_SFX), transform));
        }

        return List;
    }

    private IEnumerator WaitForReturnList(SoundEffect i_SFX)
    {
        yield return new WaitForSeconds(5.0f);
        i_SFX.Available = true;
        i_SFX.SetParent(transform);
    }
}
