using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Turing.LevelEditor;

public class LevelObjectDatabaseTest_EditMode {

	[Test]
	public void LevelObjectDatabaseTest_EditModeSimplePasses()
    {
		LevelObjectDatabase database = new LevelObjectDatabase();
        LevelEditorObject obj = database.GetObject (0, 0);
        Assert.IsNotNull (obj);
	}

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	/*[UnityTest]
	public IEnumerator LevelObjectDatabaseTest_EditModeWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}*/
}
