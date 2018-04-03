/**
 *  @author Cornelia Schultz
 */

using UnityEngine;
using Rewired;

public class InputManager : Singleton<InputManager> {

    #region Public Accessors

    /// <summary>
    /// Access to input controls shall be handled via Rewired now.
    /// To request something directly, you can utilize Rewired to do
    /// so.  (i.e. to activate vibration effects)
    /// </summary>
    public Player Player
    {
        get
        {
            return player;
        }
    }

    #endregion

    #region Protected Constructor
    protected InputManager() { }
    #endregion

    #region Internal State
    private Player player;
    #endregion

    #region Unity Lifecycle Functions
    protected override void Start()
    {
        base.Start();
        player = ReInput.players.GetPlayer(0); // get the only player
    }

    #endregion

    #region Public Interface
    
    //// Query Controls
    public Vector2 QueryAxes(string axisAlias)
    {
        // If player isn't hooked up yet, no input.
        if (player == null)
        {
            return new Vector2(0, 0);
        }

        string axisX = axisAlias + " X";
        string axisY = axisAlias + " Y";

        return player.GetAxis2D(axisX, axisY);
    }

    public ButtonMode QueryAction(string action)
    {
        // If player isn't hooked up yet, no input.
        if (player == null)
        {
            return ButtonMode.IDLE;
        }

        if (player.GetButtonDown(action))
            return ButtonMode.DOWN;
        else if (player.GetButtonUp(action))
            return ButtonMode.UP;
        else if (player.GetButton(action))
            return ButtonMode.HELD;
        else
            return ButtonMode.IDLE;
    }
    public bool QueryAction(string action, ButtonMode mode)
    {
        // If player isn't hooked up yet, no input.
        if (player == null)
        {
            return (mode == ButtonMode.IDLE) ? true : false;
        }

        switch (mode)
        {
            case ButtonMode.DOWN:
                return player.GetButtonDown(action);
            case ButtonMode.HELD:
                return player.GetButton(action);
            case ButtonMode.UP:
                return player.GetButtonUp(action);
            case ButtonMode.IDLE:
                return !player.GetButton(action);
            default:
                throw new System.Exception("IMPOSSIBRU!");
        }
    }

    //// AnyKey Wrapper
    public bool AnyKey()
    {
        return Input.anyKey || player.GetAnyButton();
    }

    //// Haptics
    public void Vibrate(VibrationTargets target, float intensity, float duration = 0.3f)
    {
        if (!SettingsManager.Instance.Vibration)
        {
            return;
        }

        if (target == VibrationTargets.BOTH)
        {
            player.SetVibration(0, intensity, duration);
            player.SetVibration(1, intensity, duration);
        } else
        {
            player.SetVibration((int)(target), intensity, duration, true);
        }
    }

    //// HasJoystick
    public bool HasJoystick()
    {
        return player.controllers.joystickCount > 0;
    }

    public Binding QueryBinding(string action)
    {
        string keyboard = null;
        string mouse = null;
        string joystick = null;
        
        foreach (var item in player.controllers.maps.ElementMapsWithAction(ControllerType.Keyboard, action, true))
        {
            if (item.enabled)
            {
                keyboard = item.elementIdentifierName;
                break;
            }
        }
        foreach (var item in player.controllers.maps.ElementMapsWithAction(ControllerType.Mouse, action, true))
        {
            if (item.enabled)
            {
                mouse = item.elementIdentifierName;
                break;
            }
        }
        foreach (var item in player.controllers.maps.ElementMapsWithAction(ControllerType.Joystick, action, true))
        {
            if (item.enabled)
            {
                joystick = item.elementIdentifierName;
                break;
            }
        }

        return new Binding(keyboard, mouse, joystick);
    }

    #endregion

    #region Types (Public)

    public struct Binding
    {
        #region Fields (Public)

        public string keyboard;

        public string mouse;

        public string joystick;

        #endregion

        #region Constructors (Public)

        public Binding(string keyboard, string mouse, string joystick)
        {
            this.keyboard = keyboard;
            this.mouse = mouse;
            this.joystick = joystick;
        }

        #endregion
    }

    #endregion
}
