using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MEC;
using System.Linq;
using SimpleJSON;

[RequireComponent(typeof(GSheetsMiner))]
public class GSheetsDeliveryBoy : RoutineRunner {

    [SerializeField] GSheetsMiner gSheetsMiner;

    public void Request<T>(Func<string, List<T>> onGetData) {
        Func<string, string> onGetJSON = (json) => {
            onGetData(json);
            return json;
        };
        gSheetsMiner.GetSheetData(onGetJSON);
    }

    public static List<T> Get<T, U>(string json) where T : class where U : GSheetsJSONParser<T>, new() {
        U adapter = new U();
        adapter.ParseJSON(json);
        return adapter.objectValues;
    }

}

[Serializable]
public abstract class GSheetsJSONParser<T> where T : class {

    public string range;
    public string majorDimension;
    public List<string> names = new List<string>();

    public List<T> objectValues = new List<T>();

    public GSheetsJSONParser() { }

    public List<T> ParseJSON(string json) {
        JSONNode jsonNode = JSON.Parse(json);
        range = jsonNode["range"];
        majorDimension = jsonNode["majorDimension"];
        JSONArray valuesArray = jsonNode["values"].AsArray;
        JSONArray namesArray = valuesArray[0].AsArray;
        AddNames(namesArray);
        ParseValuesArray(valuesArray);
        return objectValues;
    }

    void AddNames(JSONArray namesArray) {
        for (int i = 0; i < namesArray.Count; i++) {
            names.Add(namesArray[i]);
        }
    }

    public void ParseValuesArray(JSONArray array) {
        for (int i = 1; i < array.Count; i++) {
            JSONArray arrayObject = array[i].AsArray;
            objectValues.Add(ParseValueElement(arrayObject));
        }
    }

    /// <summary>
    /// Will set objectValues
    /// </summary>
    public abstract T ParseValueElement(JSONArray arrayObject);
}
