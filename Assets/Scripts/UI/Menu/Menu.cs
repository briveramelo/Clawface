/**
 *  @author Cornelia Schultz
 */

using UnityEngine;
using UnityEngine.UI;

public abstract class Menu : MonoBehaviour {

    #region Properties
    private string menuName;
    public string MenuName
    {
        get { return menuName; }
    }

    public abstract Button InitialSelection
    {
        get;
    }
    public abstract bool Displayed { get; }
    #endregion

    #region Serialized Unity Fields
    [SerializeField]
    public CanvasGroup canvasGroup;
    #endregion

    #region Unity Lifecycle Methods
    protected virtual void Start()
    {
        canvasGroup.alpha = 0.0F;
    }
    #endregion

    #region Public Interface
    public Menu(string name)
    {
        menuName = name;
    }

    public abstract void DoTransition(Transition transition, Effect[] effects);
    #endregion

    #region Protected Interface
    protected virtual void OnTransitionStarted(Transition transition, Effect[] effects)
    {
        if (TransitionStarted != null)
            TransitionStarted(transition, effects);
    }
    protected virtual void OnTransitionEnded(Transition transition, Effect[] effects)
    {
        if (TransitionEnded != null)
            TransitionEnded(transition, effects);

        if (Displayed) // menu is now visible (it wasn't before)
        {
            InitialSelection.Select();
        }
    }
    #endregion

    #region Types
    public enum Transition
    {
        SHOW,       // Reveals this menu
        HIDE,       // Hides this menu
        TOGGLE,     // Toggles the show state (shown -> hide; hidden -> show)
        NONE,       // No visibilty transition.  Useful (maybe) if we add more effects
    }
    public enum Effect
    {
        // The default if one of the following is not provided is implementation defined
        ADDITIVE,   // Adds this menu without dismissing other menus
        EXCLUSIVE,  // Adds this menu WHILE dismissing other menus

        // Some Useful Effect Types
        INSTANT,    // Do transition instantly
        FADE,       // Perform a fade on the transition
        TWEEN,      // Perform a motion tween transition (implementation defined)
    }

    public delegate void TransitionStartedEventHandler(Transition transition,
            Effect[] effects);
    public delegate void TransitionEndedEventHandler(Transition transition,
            Effect[] effects);

    public event TransitionStartedEventHandler TransitionStarted;
    public event TransitionEndedEventHandler TransitionEnded;
    #endregion
}
