using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Turing.LevelEditor;

public class SerializableLevelTest_EditMode {

	[Test]
	public void SerializableLevelTest_EditModeSimplePasses()
    {
        // Load database
		LevelObjectDatabase database = new LevelObjectDatabase();

        // Create level
        GameObject[] objects = new GameObject[0];
        LevelEditorObject data = database.GetObject(0,0);
        GameObject instance = MonoBehaviour.Instantiate (data.Prefab);
        instance.name = data.Path;

        instance.transform.position = new Vector3 (-1f, 5f, 3);
        instance.transform.rotation = Quaternion.Euler (-45f, 90f, 15f);
        instance.transform.localScale = new Vector3 (2f, 0.5f, 1.5f);
        objects[0] = instance;
     
	}
}
