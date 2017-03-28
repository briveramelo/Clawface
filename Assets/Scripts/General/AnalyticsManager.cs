using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : Singleton<AnalyticsManager> {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    #endregion



    #region Private Fields
    public Dictionary<string, object> modDictionary;


    private ModManager manager;
    #endregion


    #region Unity Lifecycle


    // Use this for initialization
    void Start () {
        modDictionary = new Dictionary<string, object>();
        modDictionary.Add("armL", "");
        modDictionary.Add("armR", "");
        modDictionary.Add("legs", "");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnApplicationQuit()
    {
        Debug.Log("Sending data on application quit!");
        Analytics.CustomEvent("playerModsEquippedOnExit", modDictionary);
    }
    #endregion

    #region Public Methods
    public void UpdatePlayerMods()
    {

    }

    #endregion

    #region Private Methods
    #endregion

    #region Private Structures
    #endregion

}
