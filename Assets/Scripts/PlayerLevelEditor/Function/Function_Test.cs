using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlayerLevelEditor
{
    public class Test : IFunction
    {
        GameObject _singletonObject;
        GameObject _movement;

        static GameObject _player;

        float waveTime = 5.0f;
        float currentTime = 0.0f;


        List<GameObject> enemies = new List<GameObject>();

        public Test(FunctionController Controller) : base(Controller)
        {

        }

        public override void Init()
        {
            base.Init();

            currentTime = waveTime;

            CreateSingleton();
            BakeAI();
        }


        public override void Update()
        {
            base.Update();

            UpdatePlayer();
            UpdateMonster();
        }


        public override void Release()
        {
            base.Release();

            _player.SetActive(false);

            foreach (GameObject _obj in enemies)
            {
                if (_obj.activeSelf)
                    _obj.GetComponent<MallCop>().OnDeath();
            }


            DeleteSingleton();
        }


        void CreateSingleton()
        {
            _singletonObject = UnityTool.FindGameObject("SingletonObject");

            if (_singletonObject == null)
            {
                _singletonObject = new GameObject("SingletonObject");

                CreateSingleton("PlayerSpawner", _singletonObject, new Vector3(0, 2.5f, 0));
                CreateSingleton("ServiceWrangler", _singletonObject, new Vector3(0, 0, 0));
 //               CreateSingleton("SpawnManager", _singletonObject, new Vector3(0, 0, 0));
                CreateSingleton("NavMeshSurface", _singletonObject, new Vector3(0, 0, 0));
            }
            else
            {

                _player.SetActive(true);

                foreach(Transform child in _singletonObject.transform)
                {
                    child.gameObject.SetActive(true);
                }

                UnityTool.FindGameObject("PlayerSpawner").SetActive(false);


                return;
            }
        }


        GameObject CreateSingleton(string objectName, GameObject _singletonObject, Vector3 position)
        {
            GameObject _prefab;
            GameObject _instance = null;

            if (UnityTool.FindGameObject(objectName) == null)
            {
                string path = "PlayerLevelEditorObjects/" + objectName;

                _prefab = Resources.Load(path) as GameObject;

                if (_prefab == null)
                {
                    Debug.Log(objectName + ": PATH ERROR!!!");
                }
                else
                {
                    _instance = GameObject.Instantiate(_prefab, position, Quaternion.identity);
                    _instance.name = objectName;
                    _instance.transform.SetParent(_singletonObject.transform);

                };
            }

            return _instance;
        }


        void DeleteSingleton()
        {
            if(_singletonObject)
            {
                foreach (Transform child in _singletonObject.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        void BakeAI()
        {
            GameObject NavSurface = GameObject.Find("NavMeshSurface");

            if (NavSurface)
            {
                NavSurface.GetComponent<UnityEngine.AI.NavMeshSurface>().BuildNavMesh();
            }
            else
            {
                Debug.Log("Error, No NavMeshSurface");
            }       
        }


        void UpdatePlayer()
        {

            if (_player == null)
            {
                _player = UnityTool.FindGameObject("Keira_GroupV1.5(Clone)");
            }
            else
            {
                Stats status = _player.GetComponentInChildren<Stats>();
                status.health = status.maxHealth;
            }


            if (_movement == null)
            {
                _movement = UnityTool.FindGameObject("Movement Effects");

                if (_movement != null)
                {
                    _movement.transform.SetParent(_singletonObject.transform);
                }
            }

        }


        void UpdateMonster()
        {
            currentTime -= Time.deltaTime;

            if (currentTime > 0.0f) return;

            currentTime = waveTime;

            for (int i = 0; i < 1; i++)
            {
                GameObject spawnedObject = ObjectPool.Instance.GetObject(PoolObjectType.MallCopBlaster);

                if (spawnedObject)
                {
                    enemies.Add(spawnedObject);

                    ISpawnable spawnable = spawnedObject.GetComponentInChildren<ISpawnable>();

                    if (!spawnable.HasWillBeenWritten())
                    {
                        spawnable.RegisterDeathEvent(ReportDeath);
                    }

                    Vector3 spawnPosition = new Vector3(0, 5, 0);

                    spawnable.WarpToNavMesh(spawnPosition);
                    EventSystem.Instance.TriggerEvent(Strings.Events.ENEMY_SPAWNED, spawnedObject);
                }
            }

        }


        private void ReportDeath()
        {

        }
    }

}

