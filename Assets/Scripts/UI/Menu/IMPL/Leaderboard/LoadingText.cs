using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour {

    [SerializeField] private Text loadingText;
    [SerializeField] private string successMessage, failureMessage, waitingString;
    [SerializeField] private bool spaceEllipses;
    [SerializeField] private float maxWaitTime = 7f;
    [SerializeField] private float ellipsesInterval = .4f;

    private System.Action<bool> onToggleWaitingForConfirmation; 
    private bool isWaitingForConfirmation;
    private bool wasSuccessful = false;
    #region Public interface
    public bool IsWaitingForConfirmation {
        get {
            return isWaitingForConfirmation;
        }
        set {
            isWaitingForConfirmation = value;
            if (value) {
                AnimateLoadingText();
            }
            if (onToggleWaitingForConfirmation!=null) {
                onToggleWaitingForConfirmation(value);
            }
        }
    }

    public void SetOnToggleWaitingForConfirmation(System.Action<bool> onToggleWaitingForConfirmation=null) {
        this.onToggleWaitingForConfirmation = onToggleWaitingForConfirmation;
    }

    /// <summary>
    /// Also Sets "IsWaitingForConfirmation" to 'false'
    /// </summary>
    /// <param name="result"></param>
    public void SetSuccess(bool result) {
        IsWaitingForConfirmation = false;
        wasSuccessful = result;
    }
    #endregion

    #region Private Interface
    private void AnimateLoadingText() {
        StopAllCoroutines();
        StartCoroutine(DelayUploadMessage());
    }

    private IEnumerator DelayUploadMessage() {
        loadingText.text = waitingString;        
        float timeWaited = 0f;
        float ellipsesTimer = 0f;
        int ellipsesIndex = 0;
        while (IsWaitingForConfirmation && timeWaited < maxWaitTime) {
            timeWaited += Time.deltaTime;
            ellipsesTimer += Time.deltaTime;
            if (ellipsesTimer > ellipsesInterval) {
                ellipsesTimer = 0f;
                ellipsesIndex++;
                UpdateEllipsesText(ellipsesIndex);
            }
            yield return null;
        }
        IsWaitingForConfirmation = false;
        loadingText.text = wasSuccessful ? successMessage : failureMessage;
    }

    private void UpdateEllipsesText(int index) {
        int newIndex = index % 4;
        string finalString = waitingString;
        string periodString = spaceEllipses ? ". " : ".";
        for (int i = 0; i < newIndex; i++) {
            finalString += periodString;
        }
        loadingText.text = finalString;
    }    
    #endregion
}
