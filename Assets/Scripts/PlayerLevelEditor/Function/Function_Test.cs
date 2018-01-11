using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PlayerLevelEditor
{
    public class Test : IFunction
    {
        GameObject _singletonObject;
        GameObject _movement;
        GameObject EditorCamera;
        GameObject CameraUI;

        static GameObject _player;

        float waveTime = 3.0f;
        float currentTime = 0.0f;

        int currentWave = 0;

        int NumEnemy_Max = 3;
        int NumEnemy = 0;

        List<GameObject> enemies = new List<GameObject>();

        Button Btn_Camera;
        UnityAction ACT_Camera;

        public Test(FunctionController Controller) : base(Controller)
        {

        }

        public override void Init()
        {
            base.Init();

            EditorCamera = UnityTool.FindGameObject("Camera");

            if(EditorCamera)
                EditorCamera.SetActive(false);

            currentTime = waveTime;
            CreateSingleton();
            BakeAI();


            SetUIObject("Function_Init");
            SetUIObject("Function_Add");
            SetUIObject("Function_Duplicate");
            SetUIObject("Function_Dynamic");
            SetUIObject("UI_Camera");

            foreach (GameObject UIObject in UIObjects)
            {
                if (UIObject.name == "UI_Camera") continue;

                UIObject.SetActive(false);
            }

           

            UITool.FindUIGameObject("Function_EndTest").SetActive(true);

            LevelEditor.m_DynamicLevelSystem.RegisterEvent();

            if (EventSystem.Instance)
            {
                EventSystem.Instance.RegisterEvent(Strings.Events.CALL_NEXTWAVEENEMIES, CallNextWave);
                EventSystem.Instance.TriggerEvent(Strings.Events.PLE_TEST_WAVE_0);
            }


            ACT_Camera = () => EnableCamera(Btn_Camera);
            Btn_Camera = PlayerLevelEditor.UITool.GetUIComponent<Button>("Button_Camera");
            if (Btn_Camera == null) Debug.Log("Btn_Add is null");

            Btn_Camera.onClick.AddListener(ACT_Camera);

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
            EditorCamera.SetActive(true);
            EditorCamera.tag = "MainCamera";

            foreach (GameObject _obj in enemies)
            {
                if (_obj.activeSelf)
                    _obj.GetComponent<MallCop>().OnDeath();
            }


            if (_player)
            {
                _player.SetActive(false);
            }


            DeleteSingleton();

            LevelEditor.m_DynamicLevelSystem.DeRegisterEvent();

            if(EventSystem.Instance)
                EventSystem.Instance.UnRegisterEvent(Strings.Events.CALL_NEXTWAVEENEMIES, CallNextWave);


            foreach (GameObject UIObject in UIObjects)
            {
                if (UIObject.name == "UI_Camera") continue;
                UIObject.SetActive(true);
            }

            UITool.FindUIGameObject("Function_EndTest").SetActive(false);

            Btn_Camera.onClick.RemoveListener(ACT_Camera);
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

                if (_player)
                {
                    _player.SetActive(true);

                    //GameObject _PC = UnityTool.FindChildGameObject(_player, "Player_Combat");

                    //if(_PC)
                    //    _PC.transform.localPosition = new Vector3(0, _PC.transform.localPosition.y, 0);
                }


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
                _player = UnityTool.FindGameObject(Strings.Editor.PLAYER_NAME);
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

            GameObject spawnedObject = ObjectPool.Instance.GetObject(PoolObjectType.MallCopBlaster);

            if (spawnedObject)
            {
                enemies.Add(spawnedObject);

                ISpawnable spawnable = spawnedObject.GetComponentInChildren<ISpawnable>();

                if (!spawnable.HasWillBeenWritten())
                {
                    spawnable.RegisterDeathEvent(ReportDeath);
                }

                RaycastHit hit;

                Vector3 spawnPosition = new Vector3(0, 5.0f, 0);

                if (Physics.Raycast(new Vector3(0.0f, 1000.0f, 0.0f), Vector3.down, out hit))
                {
                    spawnPosition = new Vector3(0, hit.point.y, 0);
                }

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
                    EventSystem.Instance.TriggerEvent(Strings.Events.PLE_TEST_WAVE_1);
                    BakeAI();
                    return;
                case 1:
                    currentWave = 2;
                    EventSystem.Instance.TriggerEvent(Strings.Events.PLE_TEST_WAVE_2);
                    BakeAI();
                    return;
                case 2:
                    currentWave = 0;
                    EventSystem.Instance.TriggerEvent(Strings.Events.PLE_TEST_WAVE_0);
                    BakeAI();
                    return;
            }
        }


        private void ReportDeath()
        {

        }


        void EnableCamera(Button thisBtn)
        {
            bool state = EditorCamera.activeSelf;
            Debug.Log(state);
            EditorCamera.SetActive(!state);
        }

    }

}

