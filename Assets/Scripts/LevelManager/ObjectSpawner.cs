// ObjectSpawner.cs
// Author: Aaron

using ModMan;

using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Turing.LevelEditor
{
    /// <summary>
    /// Class to spawn an object on level start.
    /// </summary>
    public class ObjectSpawner : MonoBehaviour
    {
        #region Vars

        /// <summary>
        /// Prefab to spawn.
        /// </summary>
        [SerializeField] GameObject template;

        /// <summary>
        /// Preview children.
        /// </summary>
        [SerializeField]
        List<GameObject> children = new List<GameObject>();

        /// <summary>
        /// Spawned instance of template.
        /// </summary>
        GameObject instance;

        #endregion
        #region Properties

        /// <summary>
        /// Returns the template used in this spawner (read-only).
        /// </summary>
        public GameObject Template { get { return template; } }

        #endregion
        #region Methods

        /// <summary>
        /// Sets the template of this spawner.
        /// </summary>
        public void SetTemplate(GameObject template)
        {
            if (template == null)
                throw new System.NullReferenceException("Null template given to spawner!");

            this.template = template;
            //transform.localScale = template.transform.localScale;

            // Show preview
            var meshFilters = template.GetComponentsInChildren<MeshFilter>();
            for (int i = 0; i < meshFilters.Length; i++)
            {
                GameObject child;

                // Add new children if necessary
                if ((i + 1) >= children.Count)
                {
                    child = new GameObject("Preview" + i, typeof(MeshFilter), typeof(MeshRenderer));
                    var materials = new Material[8];
                    for (int m = 0; m < 8; m++) {
                        materials[m] = LevelManager.Instance.PreviewMaterial;
                    }
                    child.GetComponent<MeshRenderer>().sharedMaterials = materials;
                    child.CopyColliderFromGameObject(meshFilters[i].gameObject);
                    child.transform.SetParent(transform);
                    children.Add(child);
                } 
                
                else child = children[i];

                // Copy mesh/transform of children
                child.transform.localPosition = meshFilters[i].transform.localPosition;
                child.transform.localRotation = meshFilters[i].transform.localRotation;
                child.transform.localScale = meshFilters[i].transform.localScale;
                child.GetComponent<MeshFilter>().sharedMesh = meshFilters[i].sharedMesh;
            }
        }

        /// <summary>
        /// Spawns an instance of the template and hides the spawner.
        /// </summary>
        public void Play()
        {
            instance = SpawnObject();

            if (instance == null)
            {
                if (Application.isEditor || Debug.isDebugBuild)
                    Debug.LogError("Failed to spawn object!", gameObject);
                return;
            }

            instance.transform.SetParent(LevelManager.Instance.transform);
            instance.transform.position = transform.position;
            instance.transform.localRotation = transform.localRotation;
            instance.transform.localScale = transform.localScale;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Deletes the instance and reenables the spawner.
        /// </summary>
        public void ResetSpawner()
        {
            DeleteObject(instance);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Instantiates the template.
        /// </summary>
        public GameObject SpawnObject()
        {
            if (template == null)
                throw new System.NullReferenceException("No prefab defined!");

            #if UNITY_EDITOR
            if (Application.isEditor)
                return (GameObject)PrefabUtility.InstantiatePrefab(template);
            else
            #endif
                return Instantiate(template);
        }

        /// <summary>
        /// Deletes an object.
        /// </summary>
        public void DeleteObject(GameObject toDelete)
        {
            if (Application.isEditor) DestroyImmediate(toDelete);
            else Destroy(toDelete);
        }

        #endregion
    }
}