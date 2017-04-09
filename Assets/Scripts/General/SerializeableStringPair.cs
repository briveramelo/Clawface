using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public class SerializeableStringPair {

    [SerializeField]
    string _key;

    [SerializeField]
    string _value;

    
	public string Key { get { return _key; } }

    
    public string Value { get { return _value; } }

    public SerializeableStringPair (string key, string value) {
        _key = key;
        _value = value;
    }

    public void SetValue (string newValue) {
        _value = newValue;
    }

    public override string ToString() {
        return string.Format ("{0}: {1}", _key, _value);
    }
}
