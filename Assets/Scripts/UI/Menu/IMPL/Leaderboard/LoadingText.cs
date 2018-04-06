using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour {

    private Text loadingText;
    private int loadingCount;
    private float time;
    private bool isError;

    // Use this for initialization
    private void Awake()
    {
        loadingText = GetComponent<Text>();
        loadingText.text = Strings.LEADERBOARD_LOADING;
    }

    private void OnEnable()
    {
        loadingCount = 0;
        time = 0.0f;
        isError = false;
    }

    // Update is called once per frame
    void Update () {
        if (isError)
        {
            loadingText.text = Strings.LEADERBOARD_ERROR;
        }
        else
        {
            time += Time.deltaTime;
            if (time > 1.0f)
            {
                time = 0.0f;
                loadingCount++;
                loadingCount %= 4;
                string finalString = Strings.LEADERBOARD_LOADING;
                for (int i = 0; i < loadingCount; i++)
                {
                    finalString += ".";
                }
                loadingText.text = finalString;
            }
        }
    }

    public void SetError(bool result)
    {
        isError = !result;
    }
}
