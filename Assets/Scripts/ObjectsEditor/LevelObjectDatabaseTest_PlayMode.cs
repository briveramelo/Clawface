#if UNITY_EDITOR
using NUnit.Framework;
using Turing.LevelEditor;

public class LevelObjectDatabaseTest_PlayMode {

	[Test]
	public void LevelObjectDatabaseTest_PlayModeSimplePasses()
    {
		LevelObjectDatabase database = new LevelObjectDatabase();
        LevelEditorObject obj = database.GetObject (0, 0);
        Assert.IsNotNull (obj);
	}

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	/*[UnityTest]
	public IEnumerator LevelObjectDatabaseTest_PlayModeWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}*/
}
#endif