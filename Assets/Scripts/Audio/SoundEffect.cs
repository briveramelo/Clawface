using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEffect
{
    public bool Available = true;
    protected Transform m_SFXManager = null;
    protected GameObject m_SFX_GameObject = null;
    protected Turing.Audio.AudioGroup m_AudioGroup = null;
    
    public SoundEffect(GameObject i_SFX, Transform i_SFXManager, AudioMixer i_mixerToSet)
    {
        Available = true;
        m_SFX_GameObject = i_SFX;
        m_SFXManager = i_SFXManager;
        m_SFX_GameObject.transform.parent = i_SFXManager;
        m_AudioGroup = m_SFX_GameObject.GetComponent<Turing.Audio.AudioGroup>();
        m_AudioGroup.SetMixer(i_mixerToSet);
        
    }

    public virtual void Play(Vector3 position)
    {
        Debug.Log("playing " + m_SFX_GameObject.name);
        if (m_SFX_GameObject.transform.parent != m_SFXManager)
        {
            m_SFX_GameObject.transform.parent = m_SFXManager;
        }

        m_AudioGroup.transform.position = position;
        m_AudioGroup.Play();        
    }

    public virtual void PlayFollowObject(Transform ParentsTransform)
    {
        m_SFX_GameObject.transform.SetParent(ParentsTransform);
        m_SFX_GameObject.transform.localPosition = Vector3.zero;
        m_AudioGroup.Play();
    }

    public virtual void Stop() {
        m_AudioGroup.Stop();
    }

    public virtual void SetParent(Transform parent) {
        m_SFX_GameObject.transform.parent = parent;
        m_SFX_GameObject.transform.localPosition = Vector3.zero;
    }

    public bool IsPlaying{ 
        get{return m_AudioGroup.IsPlaying; }
    }

    public GameObject GetObject()
    {
        return m_SFX_GameObject;
    }
}