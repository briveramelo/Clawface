using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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

        [SerializeField][HideInInspector]
        string path;

        public string Name { get { return name; } }

        public string Path { get { return path; } }

        public GameObject Prefab
        {
            get
            {
                if (prefab == null)
                    Debug.LogError ("No prefab for this LevelEditorObject!", this);

                return prefab;
            }
        }

        public int Limit { get { return limit; } }

        public SnapMode ObjectSnapMode { get { return snapMode; } }

        public int Width { get { return width; } }

        public int Length { get { return length; } }

        public void SetPath (string path) { this.path = path; }

        public enum SnapMode
        {
            Center,
            Edge,
            Corner
        }
    }
}