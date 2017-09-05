using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapFrameRate : MonoBehaviour {

    #region Public fields
    #endregion

    #region Serialized unity inspector fields
    [SerializeField]
    private int targetFrameRate;
    #endregion

    #region Private fields
    #endregion

    #region Unity lifecycle
    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	#endregion
	
	#region Public methods
    #endregion
	
	#region Private methods
    #endregion
	
	#region Public structures
    #endregion
	
	#region Private structures
    #endregion
}
