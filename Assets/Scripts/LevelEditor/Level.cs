// Level.cs

using UnityEngine;
using UnityEngine.Events;
using System;

/// <summary>
/// Serializable level struct for easy network transfer.
/// </summary>
[Serializable]
public class Level {

    #region Nested Classes

    /// <summary>
    /// Floor class holds 3D object data.
    /// </summary>
    [Serializable]
    public class Floor {
        
        /// <summary>
        /// Byte values at each tile.
        /// </summary>
        public ObjectAttributes[,,] values;

        /// <summary>
        /// Default constructor -- initializes all byte values to byte.MaxValue
        /// which represents empty space.
        /// </summary>
        public Floor () {
            values = new ObjectAttributes[FLOOR_WIDTH, FLOOR_HEIGHT, FLOOR_DEPTH];

            // Init values
            for (int x = 0; x < FLOOR_WIDTH; x++)
                for (int y = 0; y < FLOOR_HEIGHT; y++) 
                    for (int z = 0; z < FLOOR_DEPTH; z++)
                        values[x,y,z] = new ObjectAttributes(byte.MaxValue, 0);
        }

        /// <summary>
        /// Direct accessor.
        /// </summary>
        public ObjectAttributes this[int x, int y, int z] {
		    get { return values[x,y,z]; }
		    set { values[x,y,z] = value; }
	    }
    }

    [Serializable]
    public struct CoordinateSet {
        public int floor;
        public int x;
        public int y;
        public int z;

        public CoordinateSet (int floor, int x, int y, int z) {
            this.floor = floor;
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString() {
            return string.Format ("Coords: Floor {0}, [{1},{2},{3}]", floor, x, y, z);
        }
    }

    [Serializable]
    public class ObjectAttributes {

        [SerializeField]
        public byte index; 

        [SerializeField]
        public int yRotation;

        //[SerializeField]


        public ObjectAttributes (byte index, int yRotation) {
            this.index = index;
            this.yRotation = yRotation;
        }
    }

    [Serializable]
    public class LevelEvent : UnityEvent { }

    #endregion
    #region Vars

    const string _DEFAULT_NAME = "New Level";
    public const int MAX_FLOORS = 5;
    public const int FLOOR_WIDTH = 64;
    public const int FLOOR_DEPTH = 64;
    public const int FLOOR_HEIGHT = 10;

	/// <summary>
	/// Name of the level.
	/// </summary>
	public string name;

	/// <summary>
	/// All floor structures in the level.
	/// </summary>
    [SerializeField]
	Floor[] _floors;

    [SerializeField]
    CoordinateSet _playerSpawn;

    public LevelEvent onLevelComplete;

	#endregion
	#region Constructors

	/// <summary>
	/// Default constructor -- inits floor structures.
	/// </summary>
	public Level () {
        name = _DEFAULT_NAME;

		_floors = new Floor[MAX_FLOORS];
        for (int i = 0; i < MAX_FLOORS; i++) {
            _floors[i] = new Floor();
        }
	}

    #endregion
    #region Methods

    /// <summary>
    /// Direct accessor.
    /// </summary>
    public Floor this[int index] {
        get { return _floors[index]; }
    }

    public CoordinateSet PlayerSpawn { get { return _playerSpawn; } }

    public ObjectAttributes ObjectAt (CoordinateSet coords) {
        return _floors[coords.floor][coords.x, coords.y, coords.z];
    }

    //public void SetObjectAt (CoordinateSet coords, ObjectAttributes 

    /// <summary>
    /// Places an object in the level.
    /// </summary>
    /// <param name="index">Index of object.</param>
    public void CreateObject (int index, int floor, int x, int y, int z) {
        _floors[floor][x,y,z].index = (byte)index;
    }

    public void CreateObject (int index, CoordinateSet coords) {
        CreateObject (index, coords.floor, coords.x, coords.y, coords.z);
    }

    /// <summary>
    /// Removes an object from the level.
    /// </summary>
    public void DeleteObject (int floor, int x, int y, int z) {
        _floors[floor][x,y,z].index = byte.MaxValue;
    }

    public void DeleteObject (CoordinateSet coords) {
        DeleteObject (coords.floor, coords.x, coords.y, coords.z);
    }

    /// <summary>
    /// Creates a LevelAsset wrapper for this level.
    /// </summary>
    public LevelAsset ToLevelAsset () {
		var asset = ScriptableObject.CreateInstance<LevelAsset>();
		asset.Pack(this);
		return asset;
	}

    #endregion
    #region Overrides

	public override string ToString() {
		 return name;
	}

	#endregion
}
