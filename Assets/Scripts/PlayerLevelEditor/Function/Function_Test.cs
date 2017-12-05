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

        float waveTime = 3.0f;
        float currentTime = 0.0f;

        int currentWave = 0;

        int NumEnemy_Max = 5;
        int NumEnemy = 0;

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

            LevelEditor.m_DynamicLevelSystem.RegisterEvent();

            if (EventSystem.Instance)
                EventSystem.Instance.RegisterEvent(Strings.Events.CALL_NEXTWAVEENEMIES, CallNextWave);

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

            if(_player)
               _player.SetActive(false);

            foreach (GameObject _obj in enemies)
            {
                if (_obj.activeSelf)
                    _obj.GetComponent<MallCop>().OnDeath();
            }


            DeleteSingleton();

            LevelEditor.m_DynamicLevelSystem.DeRegisterEvent();

            if(EventSystem.Instance)
                EventSystem.Instance.UnRegisterEvent(Strings.Events.CALL_NEXTWAVEENEMIES, CallNextWave);
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

                if(_player)
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

            EventSystem.Instance.TriggerEvent(Strings.Events.CALL_NEXTWAVEENEMIES);
            return;


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

            if (NumEnemy < NumEnemy_Max)
                NumEnemy++;
            else
            {
                NumEnemy = 0;
                EventSystem.Instance.TriggerEvent(Strings.Events.CALL_NEXTWAVEENEMIES);
            }
        }

        public void CallNextWave(params object[] parameters)
        {
            switch (currentWave)
            {
                case 0:
                    currentWave = 1;
                    Debug.Log("W1");
                    EventSystem.Instance.TriggerEvent(Strings.Events.PLE_TEST_WAVE_1);
                    return;
                case 1:
                    currentWave = 2;
                    Debug.Log("W2");
                    EventSystem.Instance.TriggerEvent(Strings.Events.PLE_TEST_WAVE_2);

                    return;
                case 2:
                    currentWave = 0;
                    Debug.Log("W0");
                    EventSystem.Instance.TriggerEvent(Strings.Events.PLE_TEST_WAVE_0);

                    return;
            }
        }


        private void ReportDeath()
        {

        }

    }

}

