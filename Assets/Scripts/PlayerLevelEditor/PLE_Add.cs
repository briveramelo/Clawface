using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using PLE_ToolKit;

using Turing.LevelEditor;




public class PLE_Add : PLE_IFunction
{
    public static GameObject _prefab;

    Button  Btn_Add;
    Button  Btn_Delete;

    UnityAction ACT_Add;
    UnityAction ACT_Delete;


    public Vector3 mousePositionInScene;

    private bool clickToAddEnabled = false;
    private bool clickToDeleteEnabled = false;



    GameObject DB_List;


    List<LAIButton> BTS = new List<LAIButton>();

    public PLE_Add(PLE_FunctionController Controller) : base(Controller)
	{

    }

    public override void Init()
    {
        base.Init();

        SetUIObject("UI_Add");

        ACT_Add = () => EnableAdd(Btn_Add);
        Btn_Add = PLE_ToolKit.UITool.GetUIComponent<Button>("Button_Add");
        if (Btn_Add == null) Debug.Log("Btn_Add is null");
  
        Btn_Add.onClick.AddListener(ACT_Add);
        Btn_Add.image.color = Color.white;


        ACT_Delete = () => EnableDelete(Btn_Delete);
        Btn_Delete = PLE_ToolKit.UITool.GetUIComponent<Button>("Button_Delete");
        if (Btn_Delete == null) Debug.Log("Btn_Delete is null");

        Btn_Delete.onClick.AddListener(ACT_Delete);
        Btn_Delete.image.color = Color.white;

        _prefab = Resources.Load("LevelEditorObjects/CommonArea/test") as GameObject;


        {
            DB_List = PLE_ToolKit.UITool.FindUIGameObject("DB_List");

            GameObject _ItemExample = PLE_ToolKit.UITool.FindUIGameObject("ItemExample");

            GameObject[] _DBObject;
            _DBObject = Resources.LoadAll<GameObject>("LevelEditorObjects") as GameObject[];

            foreach(GameObject _object in _DBObject)
            {
                GameObject _item = GameObject.Instantiate(_ItemExample);

                _item.SetActive(true);
                _item.name = "Item";
                _item.GetComponentInChildren<Text>().text = _object.name;

                LAIButton _BT = new LAIButton(PLE_ToolKit.UnityTool.FindChildGameObject(_item, "Button").GetComponent<Button>(), _object);

#if UNITY_EDITOR
                Texture2D   _texture = UnityEditor.AssetPreview.GetAssetPreview(_object);
#else   
                Debug.LogWarning("Need to get a proper thumbnail for level editor prop assets!");
                Texture2D _texture = Texture2D.whiteTexture;
#endif

                Sprite _sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f));
                _item.GetComponentInChildren<Image>().sprite = _sprite;


                _item.transform.SetParent(DB_List.transform);
            }
        }


    }


    public override void Update()
    {
        base.Update();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            mousePositionInScene = hit.point;
           
            if (Input.GetMouseButtonDown(0) && clickToAddEnabled)
            {
                Vector3 _pos = PLE_ToolKit.ToolLib.ConvertToGrid(mousePositionInScene);

                GameObject.Instantiate(_prefab, _pos, Quaternion.identity);
            }

            //Right Click
            if(Input.GetMouseButtonDown(1) && clickToDeleteEnabled)
            {
                GameObject.DestroyImmediate(hit.collider.gameObject);
            }

        }

        PLE_ToolKit.ToolLib.draft(_prefab, PLE_ToolKit.ToolLib.ConvertToGrid(mousePositionInScene - _prefab.transform.position), Color.green);
    }

    public override void Release()
    {
        base.Release();
        Btn_Add.onClick.RemoveListener(ACT_Add);
    }


    public void EnableAdd(Button thisBtn)
    {
        clickToAddEnabled = !clickToAddEnabled;
        thisBtn.image.color = clickToAddEnabled ? Color.red : Color.white;
    }


    public void EnableDelete(Button thisBtn)
    {
        clickToDeleteEnabled = !clickToDeleteEnabled;
        thisBtn.image.color = clickToDeleteEnabled ? Color.red : Color.white;
    }

}


public class LAIButton
{
    Button Button;
    UnityAction Action;
    GameObject DBObject;

    public LAIButton(Button i_Button, GameObject i_Object)
    {
        Action = () => OnClick(Button);

        Button = i_Button;
        Button.onClick.AddListener(Action);

        DBObject = i_Object;
    }

    ~LAIButton()
    {
        Button.onClick.RemoveListener(Action);
    }


    public void OnClick(Button thisBtn)
    {
        PLE_Add._prefab = DBObject;
    }
}

