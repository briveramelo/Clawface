using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheCodex : MonoBehaviour {

    public static TheCodex Instance;
    Dictionary<CodexType, bool> codexDictionary;

    void Awake () {
        Instance = this;
        codexDictionary = new Dictionary<CodexType, bool>() {
            {CodexType.Journal, false },
            
        };
    }

    public void CollectCodex(CodexType codexType) {
        //TO DO, actual logic for cool codex stuff
        codexDictionary[codexType] = true;
    }
	
	
}
