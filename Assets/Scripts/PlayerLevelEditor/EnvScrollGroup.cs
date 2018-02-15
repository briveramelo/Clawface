using UnityEngine.UI;
using UnityEngine;

public class EnvScrollGroup : MonoBehaviour {


    #region Serialized Unity Fields

    [SerializeField] private GameObject iconTemplate;
    [SerializeField] private GameObject groupContent;

    #endregion

    #region Private Fields

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        InitializeWindow();
    }
    
    #endregion  

    #region Private Interface

    private void InitializeWindow(params object[] par)
    {

        GameObject[] props = Resources.LoadAll<GameObject>(Strings.Editor.ENV_OBJECTS_PATH) as GameObject[];

#if !UNITY_EDITOR
        Texture2D[] objectTextures;
        objectTextures = Resources.LoadAll<Texture2D>(Strings.Editor.IMAGE_PREVIEW_PATH) as Texture2D[];
#endif

        for(int i = 0; i < props.Length; i++)
        {
            GameObject go = props[i];

            GameObject toAdd = GameObject.Instantiate(iconTemplate);
            PLEProp propToSet = toAdd.GetComponent<PLEProp>();
            propToSet.registeredProp = go;

            toAdd.SetActive(true);
            toAdd.name = go.name;

#if UNITY_EDITOR
            Texture2D icon = UnityEditor.AssetPreview.GetAssetPreview(go);
#else
            Texture2D icon = objectTextures[i];
#endif
            if (!icon)
            {
                Debug.LogWarning("No icon image for prop: " + go.name);
                icon = Texture2D.whiteTexture;
            }
            else
            {
                Sprite newSprite = Sprite.Create(icon, new Rect(0, 0, icon.width,icon.height), new Vector2(0.5f, 0.5f));
                toAdd.GetComponent<Image>().sprite = newSprite;
            }

            toAdd.transform.SetParent(groupContent.transform);
            RectTransform myRec = toAdd.GetComponent<RectTransform>();
            myRec.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        }

    }

#endregion

}
