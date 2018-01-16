using UnityEngine.UI;
using UnityEngine;

public class AddEnemyMenu : Menu {

    #region Public Fields

    public override Button InitialSelection
    {
        get
        {
            return initiallySelected;
        }
    }

    #endregion

    #region Serialized Unity Fields

    [SerializeField] private Button initiallySelected;

    #endregion

    #region Public Interface

    public AddEnemyMenu() : base(Strings.MenuStrings.ADD_ENEMY_PLE)
    { }

    #endregion

    #region Private Interface 

    private bool addingEnabled;

    #endregion  


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

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


    public void AddAction()
    {
        //TODO: Set Button to activated state via Sprite change
#if UNITY_EDITOR
        Debug.Log("Adding enemies ooooh");
#endif

        addingEnabled = !addingEnabled;

    }
    

}
