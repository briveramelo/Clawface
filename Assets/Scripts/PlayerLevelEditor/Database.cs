using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerLevelEditor
{
    public class Database
    {
        static GameObject DBScrollView;

        GameObject DB_List;

        public Database()
        {
            DBScrollView = PlayerLevelEditor.UITool.FindUIGameObject("DBScrollView");

            DB_List = PlayerLevelEditor.UITool.FindUIGameObject("DB_List");

            GameObject _ItemExample = PlayerLevelEditor.UITool.FindUIGameObject("ItemExample");

            GameObject[] _DBObject;
            _DBObject = Resources.LoadAll<GameObject>("PlayerLevelEditorObjects/Env/") as GameObject[];

#if !UNITY_EDITOR
            Texture2D[] _DBObjectTextures;
            _DBObjectTextures = Resources.LoadAll<Texture2D>("PlayerLevelEditorObjects/png/") as Texture2D[];
#endif

            for (int index = 0; index < _DBObject.Length; index++)
            {
                GameObject _object = _DBObject[index];
                GameObject _item = GameObject.Instantiate(_ItemExample);

                _item.SetActive(true);
                _item.name = "Item";
                _item.GetComponentInChildren<UnityEngine.UI.Text>().text = _object.name;

                ItemButton _BT = new ItemButton(PlayerLevelEditor.UnityTool.FindChildGameObject(_item, "Button").GetComponent<UnityEngine.UI.Button>(), _object);

#if UNITY_EDITOR
                Texture2D _texture = UnityEditor.AssetPreview.GetAssetPreview(_object);
#else
                Texture2D _texture = _DBObjectTextures[index];
#endif

                if (!_texture)
                {
                    Debug.LogWarning("No Texture for ResourceObj: " + _object);
                    _texture = Texture2D.whiteTexture;
                }
                Sprite _sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f));
                _item.GetComponentInChildren<UnityEngine.UI.Image>().sprite = _sprite;

                _item.transform.SetParent(DB_List.transform);
            }


            Disable();
        }

        public static void Enable()
        {
            DBScrollView.SetActive(true);
        }

        public static void Disable()
        {
            DBScrollView.SetActive(false);
        }

    }


    public class ItemButton
    {
        UnityEngine.UI.Button Button;
        UnityEngine.Events.UnityAction Action;
        GameObject DBObject;

        public ItemButton(UnityEngine.UI.Button i_Button, GameObject i_Object)
        {
            Action = () => OnClick(Button);

            Button = i_Button;
            Button.onClick.AddListener(Action);

            DBObject = i_Object;
        }

        ~ItemButton()
        {
            Button.onClick.RemoveListener(Action);
        }


        public void OnClick(UnityEngine.UI.Button thisBtn)
        {
            Add._prefab = DBObject;
        }
    }

}
