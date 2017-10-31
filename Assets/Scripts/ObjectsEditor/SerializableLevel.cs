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
        [SerializeField] string name;
        [SerializeField] ObjectData[] objects;

        public GameObject[] ReconstructLevel (LevelObjectDatabase database)
        {
            GameObject[] result = new GameObject[objects.Length];

            for (int i = 0; i < objects.Length; i++)
            {
                result[i] = ReconstructObject (database, objects[i]);
            }

            return result;
        }

        public GameObject ReconstructObject (LevelObjectDatabase database, ObjectData obj)
        {
            LevelEditorObject objData = database.GetObject (obj.path);
            GameObject instance = MonoBehaviour.Instantiate(objData.Prefab);
            instance.name = obj.path;
            instance.transform.position = obj.position;
            instance.transform.rotation = Quaternion.Euler (obj.rotation);
            instance.transform.localScale = obj.scale;
            return instance;
        }

        public SerializableLevel SerializeLevel (string name, GameObject[] objects)
        {
            SerializableLevel level = new SerializableLevel();
            level.name = name;
            level.objects = new ObjectData[objects.Length];

            for (int i = 0; i < objects.Length; i++)
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