using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrack {

    protected GameObject m_AudTrac_GameObject = null;
    protected Turing.Audio.MusicGroup m_MusicGroup = null;

    public MusicTrack(GameObject i_track)
    {
        m_AudTrac_GameObject = i_track;
        m_MusicGroup = m_AudTrac_GameObject.GetComponent<Turing.Audio.MusicGroup>();
    }

    public virtual void Play(Vector3 position)
    {
        m_MusicGroup.transform.position = position;
        m_MusicGroup.Play();
    }

    public virtual void Stop()
    {
        m_MusicGroup.Stop();
    }

    public virtual void SetParent(Transform i_parent)
    {
        m_AudTrac_GameObject.transform.parent = i_parent;
    }
}
