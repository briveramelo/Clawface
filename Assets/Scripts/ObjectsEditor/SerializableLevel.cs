using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ModMan;

namespace Turing.LevelEditor
{

    [Serializable]
    public class SerializableLevel
    {
        [SerializeField] ObjectData[] objects;

        public List<GameObject> ReconstructLevel (LevelObjectDatabase database)
        {
            List<GameObject> result = new List<GameObject>();

            GameObject parent = new GameObject("LOADED LEVEL");

            for (int i = 0; i < objects.Length; i++)
            {
                GameObject obj = ReconstructObject (database, objects[i]);
                obj.transform.SetParent (parent.transform);
                result.Add(obj);
            }

            return result;
        }

        public GameObject ReconstructObject (LevelObjectDatabase database, ObjectData obj)
        {
            if (obj.Equals(default(ObjectData)))
            {
                Debug.LogError ("Failed to deserialize object");
                return null;
            }

            LevelEditorObject objData = database.GetObject (obj.path);
            GameObject instance = MonoBehaviour.Instantiate(objData.Prefab);
            instance.name = obj.path;
            instance.transform.position = obj.position;
            instance.transform.rotation = Quaternion.Euler (obj.rotation);
            instance.transform.localScale = obj.scale;
            return instance;
        }

        public static SerializableLevel MakeSerializableLevel (List<GameObject> objects)
        {
            SerializableLevel level = new SerializableLevel();
            level.objects = new ObjectData[objects.Count];

            for (int i = 0; i < objects.Count; i++)
            {
                GameObject obj = objects[i];
                ObjectData data = new ObjectData();
                data.path = obj.name;
                data.position = obj.transform.position.ToFloat3();
                data.rotation = obj.transform.rotation.eulerAngles.ToFloat3();
                data.scale = obj.transform.localScale.ToFloat3();
                level.objects[i] = data;
            }

            return level;
        }

        [Serializable]
        public struct ObjectData
        {
            public string path;
            public Float3 position;
            public Float3 rotation;
            public Float3 scale;
        }
    }
}