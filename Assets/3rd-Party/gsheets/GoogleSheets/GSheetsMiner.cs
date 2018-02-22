using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System;

[ExecuteInEditMode, RequireComponent(typeof(GSheetsDeliveryBoy))]
public class GSheetsMiner : MonoBehaviour {

    [Tooltip("Eg: https://sheets.googleapis.com/v4/spreadsheets")]
    public string urlRoot= "https://sheets.googleapis.com/v4/spreadsheets";

    [Tooltip("Eg: docs.google.com/spreadsheets/d/sheetID/edit")]
    public string sheetID;

    [Tooltip("Eg: Sheet2!D2:D")]
    public string sheetRange="Sheet1";

    [Tooltip("Find at your https://console.cloud.google.com/apis/credentials page")]
    public string apiKey;

    public bool AllFieldsFilled { get { return !string.IsNullOrEmpty(urlRoot) && !string.IsNullOrEmpty(sheetID) && !string.IsNullOrEmpty(sheetRange) && !string.IsNullOrEmpty(apiKey); } }
    public string Url { get { return string.Format("{0}/{1}/values/{2}?key={3}", urlRoot, sheetID, sheetRange, apiKey); } }		

    public void GetSheetData(Func<string, string> onGetJSON) {
        StartCoroutine(IEGetSheetData(onGetJSON));
    }

    IEnumerator IEGetSheetData(Func<string, string> jsonOnReturn) {
        string jsonOutput = null;
        WWW www = new WWW(Url);
        yield return www;
        if (www.error == null) {
            jsonOutput = www.text;
        }
        else {
            jsonOutput = null;
            print(www.error);
        }
        jsonOnReturn(jsonOutput);
    }
}