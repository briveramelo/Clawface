using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Turing.LevelEditor;
using System.IO;

public class SerializableLevelTest_EditMode {

	[Test]
	public void SerializableLevelTest_EditModeSimplePasses()
    {
        // Load database
		LevelObjectDatabase database = new LevelObjectDatabase();

        // Create level
        GameObject[] objects = new GameObject[1];

        // Make an object
        LevelEditorObject data = database.GetObject(0,0);
        Assert.IsNotNull (data);

        GameObject instance = MonoBehaviour.Instantiate (data.Prefab);
        instance.name = database.GetObjectPath (data);
        instance.transform.position = new Vector3 (-1f, 5f, 3);
        instance.transform.rotation = Quaternion.Euler (-45f, 90f, 15f);
        instance.transform.localScale = new Vector3 (2f, 0.5f, 1.5f);
        objects[0] = instance;
     
        // Serialize and save level
        SerializableLevel level = SerializableLevel.SerializeLevel ("New Level", objects);
        JSONFileUtility.SaveToJSONFile (level, "Assets/Resources/Levels/New Level.json");

        // Refresh database
        database = new LevelObjectDatabase();

        // Load and reconstruct database
        SerializableLevel loadedLevel = JSONFileUtility.LoadFromJSONFile<SerializableLevel> ("Assets/Resources/Levels/New Level.json");
        GameObject[] newObjects = loadedLevel.ReconstructLevel (database);

        Assert.AreEqual (newObjects[0].transform.position.x, -1f);
	}
}
