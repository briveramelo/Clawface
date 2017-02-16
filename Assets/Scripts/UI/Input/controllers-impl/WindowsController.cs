/**
 *  @author Cornelia Schultz
 *  
 *  See: http://wiki.unity3d.com/index.php?title=Xbox360Controller
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsController : IController
{

    #region Constants

        static readonly string LEFT_AXIS_VERTICAL = "WINDOWS_LEFT_AXIS_VERTICAL";
        static readonly string LEFT_AXIS_HORIZONTAL = "WINDOWS_LEFT_AXIS_HORIZONTAL";
        static readonly string RIGHT_AXIS_VERTICAL = "WINDOWS_RIGHT_AXIS_VERTICAL";
        static readonly string RIGHT_AXIS_HORIZONTAL = "WINDOWS_RIGHT_AXIS_HORIZONTAL";
        static readonly string RIGHT_TRIGGER = "WINDOWS_RIGHT_TRIGGER_AXIS";
        static readonly string LEFT_TRIGGER = "WINDOWS_LEFT_TRIGGER_AXIS";

        static readonly KeyCode LeftSecondary = KeyCode.JoystickButton4;
        static readonly KeyCode RightSecondary = KeyCode.JoystickButton5;
        static readonly KeyCode LeftTertiary = KeyCode.JoystickButton8;
        static readonly KeyCode RightTertiary = KeyCode.JoystickButton9;

        static readonly KeyCode Action1 = KeyCode.JoystickButton0;
        static readonly KeyCode Action2 = KeyCode.JoystickButton1;
        static readonly KeyCode Action3 = KeyCode.JoystickButton2;
        static readonly KeyCode Action4 = KeyCode.JoystickButton3;

        static readonly string DPAD_VERTICAL = "WINDOWS_DPAD_VERTICAL_AXIS";
        static readonly string DPAD_HORIZONTAL = "WINDOWS_DPAD_HORIZONTAL_AXIS";

        static readonly KeyCode Select = KeyCode.JoystickButton6;
        static readonly KeyCode Start = KeyCode.JoystickButton7;

    #endregion

    #region Internal State

    private ButtonMode triggerLeft;
        private ButtonMode triggerRight;

    #endregion

    public void Update()
    {
        triggerLeft = Transition(triggerLeft, Input.GetAxis(LEFT_TRIGGER));
        triggerRight = Transition(triggerRight, Input.GetAxis(RIGHT_TRIGGER));
    }
    private ButtonMode Transition(ButtonMode mode, float axis)
    {
        switch (mode)
        {
            case ButtonMode.UP:
                return axis == 0 ? ButtonMode.IDLE : ButtonMode.DOWN;
            case ButtonMode.DOWN:
                return axis != 0 ? ButtonMode.HELD : ButtonMode.UP;
            case ButtonMode.HELD:
                return axis != 0 ? ButtonMode.HELD : ButtonMode.UP;
            case ButtonMode.IDLE:
                return axis == 0 ? ButtonMode.IDLE : ButtonMode.DOWN;
            default:
                throw new System.SystemException("IMPOSSIBRU!");
        }
    }

    #region Axes

        public float GetLeftXAxis()
        {
            return Input.GetAxis(LEFT_AXIS_HORIZONTAL);
        }
        public float GetLeftYAxis()
        {
            return Input.GetAxis(LEFT_AXIS_VERTICAL);
        }
        public float GetRightXAxis()
        {
            return Input.GetAxis(RIGHT_AXIS_HORIZONTAL);
        }
        public float GetRightYAxis()
        {
            return Input.GetAxis(RIGHT_AXIS_VERTICAL);
        }

    #endregion

    #region Primaries

        public bool GetLeftPrimaryDown()
        {
            return triggerLeft == ButtonMode.DOWN;
        }
        public bool GetLeftPrimaryHeld()
        {
            return triggerLeft == ButtonMode.HELD;
        }
        public bool GetLeftPrimaryUp()
        {
            return triggerLeft == ButtonMode.UP;
        }
        public bool GetLeftPrimaryIdle()
        {
            return triggerLeft == ButtonMode.IDLE;
        }

        public bool GetRightPrimaryDown()
        {
            return triggerRight == ButtonMode.DOWN;
        }
        public bool GetRightPrimaryHeld()
        {
            return triggerRight == ButtonMode.HELD;
        }
        public bool GetRightPrimaryUp()
        {
            return triggerRight == ButtonMode.UP;
        }
        public bool GetRightPrimaryIdle()
        {
            return triggerRight == ButtonMode.IDLE;
        }

    #endregion

    #region Secondaries

    public bool GetLeftSecondaryDown()
    {
        return Input.GetKeyDown(LeftSecondary);
    }
    public bool GetLeftSecondaryHeld()
    {
        return Input.GetKey(LeftSecondary);
    }
    public bool GetLeftSecondaryUp()
    {
        return Input.GetKeyUp(LeftSecondary);
    }
    public bool GetLeftSecondaryIdle()
    {
        return !Input.GetKey(LeftSecondary);
    }

    public bool GetRightSecondaryDown()
    {
        return Input.GetKeyDown(RightSecondary);
    }
    public bool GetRightSecondaryHeld()
    {
        return Input.GetKey(RightSecondary);
    }
    public bool GetRigthSecondaryUp()
    {
        return Input.GetKeyUp(RightSecondary);
    }
    public bool GetRightSecondaryIdle()
    {
        return !Input.GetKey(RightSecondary);
    }

    #endregion

    #region Tertiaries

    public bool GetLeftTertiaryDown();
    public bool GetLeftTertiary();
    public bool GetLeftTertiaryUp();

    public bool GetRigthTertiaryDown();
    public bool GetRightTertiaryHeld();
    public bool GetRightTertiaryUp();

    #endregion

    #region Action Keys

    public bool GetAction1Down();
    public bool GetAction1Held();
    public bool GetAction1Up();

    public bool GetAction2Down();
    public bool GetAction2Held();
    public bool GetAction2Up();

    public bool GetAction3Down();
    public bool GetAction3Held();
    public bool GetAction3Up();

    public bool GetAction4Down();
    public bool GetAction4Held();
    public bool GetAction4Up();

    #endregion

    #region DPad

    public bool GetDPadUpDown();
    public bool GetDPadUpHeld();
    public bool GetDPadUpUp();

    public bool GetDPadDownDown();
    public bool GetDPadDownHeld();
    public bool GetDPadDownUp();

    public bool GetDPadLeftDown();
    public bool GetDPadLeftHeld();
    public bool GetDPadLeftUp();

    public bool GetDPadRightDown();
    public bool GetDPadRightHeld();
    public bool GetDPadRightUp();

    #endregion

    #region Specials

    public bool GetBackDown();
    public bool GetBackHeld();
    public bool GetBackUp();

    public bool GetStartDown();
    public bool GetStartHeld();
    public bool GetStartUp();

    #endregion
}
