using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect
{
    protected GameObject m_SFX_GameObject = null;
    protected Turing.Audio.AudioGroup m_AudioGroup = null;

    public SoundEffect(GameObject i_SFX)
    {
        m_SFX_GameObject = i_SFX;
        m_AudioGroup = m_SFX_GameObject.GetComponent<Turing.Audio.AudioGroup>();
    }

    public virtual void Play()
    {
        m_AudioGroup.Play();
    }
}