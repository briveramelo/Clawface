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
    [SerializeField]
    private GameObject Boomerang_Throw;
    [SerializeField]
    private GameObject DiceLauncher_Shoot;
    [SerializeField]
    private GameObject GeyserMod_Splash;
    [SerializeField]
    private GameObject ModCooldown;
    [SerializeField]
    private GameObject UI_Click;
    [SerializeField]
    private GameObject UI_Hover;
    [SerializeField]
    private GameObject UI_Back;
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
            {SFXType.SegwayBlast_Standard,      InitList(SegwayBlast_Standard) },
            {SFXType.Boomerang_Throw,           InitList(Boomerang_Throw) },
            {SFXType.DiceLauncher_Shoot,        InitList(DiceLauncher_Shoot) },
            {SFXType.GeyserMod_Splash,          InitList(GeyserMod_Splash) },
            {SFXType.ModCooldown,               InitList(ModCooldown) },
            {SFXType.UI_Click,                  InitList(UI_Click)},
            {SFXType.UI_Hover,                  InitList(UI_Hover)},
            {SFXType.UI_Back,                   InitList(UI_Back)}
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

            //If the SFX all are not available, create a new one 
            SoundEffect newSFX = new SoundEffect(Instantiate(sfxDictionary[i_Type][0].GetObject()), transform);
            newSFX.Available = false;
            newSFX.Play(position);
            sfxDictionary[i_Type].Add(newSFX);
            StartCoroutine(WaitForReturnList(newSFX));
            return;
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

            //If the SFX all are not available, create a new one 
            SoundEffect newSFX = new SoundEffect(Instantiate(sfxDictionary[i_Type][0].GetObject()), transform);
            newSFX.Available = false;
            newSFX.PlayFollowObject(i_parents);
            sfxDictionary[i_Type].Add(newSFX);
            StartCoroutine(WaitForReturnList(newSFX));
            return;
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
//       string message="No SFX Found for " + i_Type + ". Please add.";
//       Debug.LogFormat("<color=#0000FF>" + message + "</color>");
    }
    

    private List<SoundEffect> InitList(GameObject i_SFX)
    {
        List<SoundEffect> List = new List<SoundEffect>();
        int numSFX = 5;

        for(int i = 0; i < numSFX; i++)
        {
            List.Add(new SoundEffect(Instantiate(i_SFX), transform));
        }

        return List;
    }


    private IEnumerator WaitForReturnList(SoundEffect i_SFX)
    {
        //yield return new WaitForSeconds(5f); Took this out suspecting it could be causing the problem and replacing it with the lines below
        yield return null;

        while(i_SFX.IsPlaying)
        { 
            yield return null;        
        }

        i_SFX.Available = true;
        i_SFX.SetParent(transform);
    }
}
