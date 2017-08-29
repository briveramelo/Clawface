﻿/**
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
    protected override void Awake()
    {
        base.Awake();

        player = ReInput.players.GetPlayer(0); // get the only player
    }

    #endregion

    #region Public Interface
    
    //// Query Controls
    public Vector2 QueryAxes(string axisAlias)
    {
        string axisX = axisAlias + " X";
        string axisY = axisAlias + " Y";

        return player.GetAxis2D(axisX, axisY);
    }

    public ButtonMode QueryAction(string action)
    {
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
        return Input.anyKey;
    }

    //// Haptics
    public void Vibrate(VibrationTargets target, float intensity, float duration = 0.3f)
    {
        if (target == VibrationTargets.BOTH)
        {
            player.SetVibration(0, intensity, duration);
            player.SetVibration(1, intensity, duration);
        } else
        {
            player.SetVibration((int)(target), intensity, duration, true);
        }
    }


    #endregion
}
