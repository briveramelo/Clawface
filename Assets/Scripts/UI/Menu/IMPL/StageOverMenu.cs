/**
*  @author Cornelia Schultz
*/
using UnityEngine;
using UnityEngine.UI;

public class StageOverMenu : Menu
{
    #region Public Fields

    public override Button InitialSelection
    {
        get
        {
            return quitButton;
        }
    }
    #endregion

    #region Serialized Unity Fields

    [SerializeField]
    private Button quitButton;

    [SerializeField]
    private Text score;

    [SerializeField]
    private Text combo;

    #endregion

    #region Public Interface
    public StageOverMenu() : base(Strings.MenuStrings.STAGE_OVER) { }

    public void QuitAction()
    {
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = (LoadMenu)menu;
        loadMenu.TargetScene = Strings.Scenes.MainMenu;
        loadMenu.Fast = true;
        MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
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

    protected override void ShowStarted()
    {
        UpdateScores();
    }

    protected override void ShowComplete()
    {
        base.ShowComplete();
    }

    protected override void HideComplete()
    {
        base.HideComplete();
    }

    #endregion

    #region Private Interface

    private void UpdateScores()
    {
        score.text = ScoreManager.Instance.GetScore().ToString();
        combo.text = ScoreManager.Instance.GetHighestCombo().ToString();
    }

    #endregion
}
