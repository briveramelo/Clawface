using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : Singleton<SFXManager>
{
    protected SFXManager() { }
    private Dictionary<SFXType, List<SoundEffect>> sfxDictionary;

    #region SFX Object
    [SerializeField] private List<SFXUnit> SFXList;
    [SerializeField] private AudioMixer sfxMixer;
    #endregion

    private void Start()
    {
        sfxDictionary = new Dictionary<SFXType, List<SoundEffect>>();
        foreach (SFXUnit unit in SFXList)
        {

            sfxDictionary.Add(unit.type, InitList(unit.audioClipObject));
        }
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
            CreateNewSFX(i_Type, sfxDictionary[i_Type]).Play(position);
            return;
        }

        string message="No SFX Found for " + i_Type.ToString() + ". Please add.";
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
            CreateNewSFX(i_Type, sfxDictionary[i_Type]).PlayFollowObject(i_parents);
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
            List.Add(new SoundEffect(Instantiate(i_SFX), transform, sfxMixer));
        }

        return List;
    }

    private SoundEffect CreateNewSFX(SFXType i_Type, List<SoundEffect> List)
    {
        SoundEffect newSFX = new SoundEffect(Instantiate(List[0].GetObject()), transform, sfxMixer);
        List.Add(newSFX);
        newSFX.Available = false;
        StartCoroutine(WaitForReturnList(newSFX));

        return newSFX;
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

    [System.Serializable]
    public class SFXUnit
    {
        public SFXType type;
        public GameObject audioClipObject;
    }
}
