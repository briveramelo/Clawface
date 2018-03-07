using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour {

    private Text loadingText;
    private int loadingCount;
    private string loadingString = "LOADING";
    private float time;

    // Use this for initialization
    private void Awake()
    {
        loadingText = GetComponent<Text>();
        loadingText.text = loadingString;
    }

    private void OnEnable()
    {
        loadingCount = 0;
        time = 0.0f;
    }

    // Update is called once per frame
    void Update () {
        time += Time.deltaTime;
        if (time > 1.0f)
        {
            time = 0.0f;
            loadingCount++;
            loadingCount %= 4;
            string finalString = loadingString;
            for (int i = 0; i < loadingCount; i++)
            {
                finalString += ".";
            }
            loadingText.text = finalString;
        }
    }
}
