// LevelAsset.cs

using UnityEngine;

/// <summary>
/// A ScriptableObject wrapper for the level struct.
/// </summary>
[System.Serializable]
public class LevelAsset : ScriptableObject {

	#region Vars

	/// <summary>
	/// The Level contained in this LevelAsset.
	/// </summary>
	[SerializeField]
	Level _level;

	#endregion
	#region Methods

	/// <summary>
	/// Packs a Level struct into this LevelAsset.
	/// </summary>
	public void Pack (Level level) {
		_level = level;
	}

    /// <summary>
    /// Unpacks a Level struct from this LevelAsset.
    /// </summary>
	public Level Unpack () {
		return _level;
	}

	#endregion
	#region Overrides

	public override string ToString() {
		return _level.ToString();
	}

	#endregion
}
