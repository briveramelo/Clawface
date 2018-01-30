using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;

public class GenerateLEDataBaseTextures : IPreprocessBuild {

    #region Interface (IPreprocessBuild)

    public int callbackOrder
    {
        get
        {
            return 0;
        }
    }
    
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        ClearOldData();
        Output[] envData = GetOutputData(Strings.Editor.ENV_OBJECTS_PATH);
        Output[] spawnData = GetOutputData(Strings.Editor.SPAWN_OBJECTS_PATH);
        OutputNewData(envData);
        OutputNewData(spawnData);
    }

    #endregion

    #region Interface (Private)

    private void ClearOldData()
    {
        string[] oldAssets = AssetDatabase.FindAssets("t:Texture2D", new string[] { "Assets/Resources/PlayerLevelEditorObjects/png" });
        foreach (string guid in oldAssets)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AssetDatabase.DeleteAsset(path);
        }
        AssetDatabase.SaveAssets();
    }

    private Output[] GetOutputData(string i_path)
    {
        GameObject[] databaseObjects =
            Resources.LoadAll<GameObject>(i_path) as GameObject[];
        Output[] data = new Output[databaseObjects.Length];

        for (int index = 0; index < databaseObjects.Length; index++)
        {
            data[index].name = databaseObjects[index].name;
            data[index].texture = AssetPreview.GetAssetPreview(databaseObjects[index]);
        }

        return data;
    }

    private void OutputNewData(Output[] data)
    {
        const string STUB = "/Resources/PlayerLevelEditorObjects/png/";

        if (!Directory.Exists(Application.dataPath + STUB))
        {
            Directory.CreateDirectory(Application.dataPath + STUB);
        }

        foreach (Output datum in data)
        {
            byte[] bytes = datum.texture.EncodeToPNG();
            string path = Application.dataPath + STUB + datum.name + ".png";
            OutputNewData(bytes, path);
        }
        AssetDatabase.Refresh();
    }

    private void OutputNewData(byte[] bytes, string path)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
        {
            writer.Write(bytes);
        }
    }

    #endregion

    #region Types (Private)

    private struct Output
    {
        #region Fields (Public)

        public Texture2D texture;
        public string name;

        #endregion
    }

    #endregion
}
