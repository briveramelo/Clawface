using UnityEngine.UI;
using UnityEngine;

public class InitPLEMenu : Menu {

    #region Public Fields

    public override Button InitialSelection
    {
        get
        {
            return confirmButton;
        }
    }

    #endregion

    #region Serialized Unity Fields

    [SerializeField] private Button confirmButton;
    [SerializeField] private Slider xSlide;
    [SerializeField] private Slider zSlide;

    #endregion

    #region Private Fields

    int levelX = 0;
    int levelZ = 0;

    GameObject levelBlock;

    #endregion

    #region Public Interface

    public InitPLEMenu() : base(Strings.MenuStrings.INIT_PLE_MENU)
    { }

    #endregion

    #region Unity Lifecycle

    protected override void Start()
    {
        base.Start();

        levelBlock = Resources.Load(Strings.Editor.RESOURCE_PATH + "Env/test") as GameObject;
        levelBlock.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }

    #endregion  

    #region Protected Interface

    protected override void DefaultShow(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    protected override void DefaultHide(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    #endregion

    #region Public Interface

    public void ConfirmAction()
    {
#if UNITY_EDITOR
        Debug.Log("Confirming level size");
#endif
    }

    public void UpdateBlockPreview()
    {
        levelX = (int)xSlide.value;
        levelZ = (int)zSlide.value;

        for (int i = -levelX; i <= levelX; i++)
        {
            for (int j = -levelZ; j <= levelZ; j++)
            {
                Vector3 new_position = new Vector3(i * PlayerLevelEditor.ParameterSystem.unit_size, 0.0f, j * PlayerLevelEditor.ParameterSystem.unit_size);
                PlayerLevelEditor.ToolLib.draft(levelBlock, new_position, Color.yellow);
            }
        }
    }
    #endregion
}
