using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(GSheetsDeliveryBoy),typeof(GSheetsMiner))]
public abstract class Grabber<T, U> : MonoBehaviour where T: GSheetData where U : GSheetsJSONParser<T>, new() {

    [SerializeField] GSheetsDeliveryBoy deliveryBoy;
    public List<T> myDataList;

    public void RequestData() {
        deliveryBoy.Request<T>(OnGetData);
    }

    List<T> OnGetData(string json) {
        myDataList = GSheetsDeliveryBoy.Get<T, U>(json);
        return myDataList;
    }
}

[System.Serializable]
public class GSheetData {

}