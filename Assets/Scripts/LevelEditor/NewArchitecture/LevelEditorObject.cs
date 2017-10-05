using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Turing.LevelEditor
{
    [CreateAssetMenu(menuName = "LevelEditorObject")]
    public class LevelEditorObject : ScriptableObject
    {

        [SerializeField] new string name;
        [SerializeField] GameObject prefab;
        [SerializeField] int width = 1;
        [SerializeField] int length = 1;
        [SerializeField] int limit = -1;
        [SerializeField] SnapMode snapMode;

        public string Name { get { return name; } }

        public GameObject Prefab { get { return prefab; } }

        public int Limit { get { return limit; } }

        public SnapMode ObjectSnapMode { get { return snapMode; } }

        public int Width { get { return width; } }

        public int Length { get { return length; } }

        public enum SnapMode
        {
            Center,
            Edge,
            Corner
        }
    }
}