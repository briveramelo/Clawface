using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable, StructLayout(LayoutKind.Sequential)]
public struct SerializeableKeyValuePair<TKey, TValue> {

    TKey _key;
    TValue _value;

	public TKey Key { get { return _key; } }
    public TValue Value { get { return _value; } }

    public SerializeableKeyValuePair (TKey key, TValue value) {
        _key = key;
        _value = value;
    }

    public void SetValue (TValue newValue) {
        _value = newValue;
    }

}
