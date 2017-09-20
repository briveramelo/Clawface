using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HookEndPoint : MonoBehaviour {

    #region Public fields
    public HitEvent targetHitEvent;
    #endregion

    #region Serialized unity inspector fields
    #endregion

    #region Private fields
    #endregion

    #region Unity lifecycle
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        targetHitEvent.Invoke(other);
    }
    #endregion

    #region Public methods
    #endregion

    #region Private methods
    #endregion

    #region Public structures
    public class HitEvent : UnityEvent<Collider>
    {
    }
    #endregion

    #region Private structures    
    #endregion
}
