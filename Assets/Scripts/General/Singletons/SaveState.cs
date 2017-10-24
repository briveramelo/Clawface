using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveState : Singleton<SaveState> {

    public void Load()
    {

    }

    public bool GetBool(string key, bool defaultValue)
    {
        int defaultInt = defaultValue ? 1 : 0;        
        return PlayerPrefs.GetInt(key, defaultInt) == 1;
    }

    public void SetBool(string key, bool value)
    {
        int intValue = value ? 1 : 0;
        PlayerPrefs.SetInt(key, intValue);
    }

    public string GetString(string key, string defaultValue)
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public float GetFloat(string key, float defaultValue)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public int GetInt(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    public void Save()
    {
        PlayerPrefs.Save();
    }
}
