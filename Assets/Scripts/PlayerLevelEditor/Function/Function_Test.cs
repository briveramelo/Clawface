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
        static GameObject _enemyPool;



        float waveTime = 3.0f;
        float currentTime = 0.0f;

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

            InitObject();

            CreateMonster();

        }


        public override void Release()
        {
            base.Release();

            _player.SetActive(false);

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


        void CreateSingleton(string objectName, GameObject _singletonObject, Vector3 position)
        {
            GameObject _prefab;
            GameObject _instance;

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


        void InitObject()
        {

            if (_player == null)
            {
                _player = UnityTool.FindGameObject("Keira_GroupV1.5(Clone)");
            }

            if(_enemyPool == null)
            {
                _enemyPool = UnityTool.FindGameObject("BlasterCop");
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


        void CreateMonster()
        {
            currentTime -= Time.deltaTime;

            if (currentTime > 0.0f || _enemyPool == null) return;

            currentTime = waveTime;


            int NumEnemy = 2;

            foreach(Transform enemy in _enemyPool.transform)
            {
                if(enemy.gameObject.activeSelf == false)
                {
                    enemy.gameObject.SetActive(true);
                    NumEnemy--;
                }


                if (NumEnemy == 0) return;
            }
        }


    }

}

