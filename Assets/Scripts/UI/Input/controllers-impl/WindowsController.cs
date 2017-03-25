/**
 *  @author Cornelia Schultz
 *  
 *  See: http://wiki.unity3d.com/index.php?title=Xbox360Controller
 */
using UnityEngine;

#if UNITY_STANDALONE_WIN
    using XInputWrapper;
#endif

public class XBox360Controller : IController
{

    #region Constants

        static readonly string LEFT_AXIS_VERTICAL = "WINDOWS_LEFT_AXIS_VERTICAL";
        static readonly string LEFT_AXIS_HORIZONTAL = "WINDOWS_LEFT_AXIS_HORIZONTAL";
        static readonly string RIGHT_AXIS_VERTICAL = "WINDOWS_RIGHT_AXIS_VERTICAL";
        static readonly string RIGHT_AXIS_HORIZONTAL = "WINDOWS_RIGHT_AXIS_HORIZONTAL";
        static readonly string RIGHT_TRIGGER = "WINDOWS_RIGHT_TRIGGER_AXIS";
        static readonly string LEFT_TRIGGER = "WINDOWS_LEFT_TRIGGER_AXIS";

        static readonly KeyCode LEFT_SECONDARY = KeyCode.JoystickButton4;
        static readonly KeyCode RIGHT_SECONDARY = KeyCode.JoystickButton5;
        static readonly KeyCode LEFT_TERTIARY = KeyCode.JoystickButton8;
        static readonly KeyCode RIGHT_TERTIARY = KeyCode.JoystickButton9;

        static readonly KeyCode ACTION_1 = KeyCode.JoystickButton0;
        static readonly KeyCode ACTION_2 = KeyCode.JoystickButton1;
        static readonly KeyCode ACTION_3 = KeyCode.JoystickButton2;
        static readonly KeyCode ACTION_4 = KeyCode.JoystickButton3;

        static readonly string DPAD_VERTICAL = "WINDOWS_DPAD_VERTICAL_AXIS";
        static readonly string DPAD_HORIZONTAL = "WINDOWS_DPAD_HORIZONTAL_AXIS";

        static readonly KeyCode SELECT = KeyCode.JoystickButton6;
        static readonly KeyCode START = KeyCode.JoystickButton7;

    #endregion

    #region Internal State

        private ButtonMode triggerLeft;
        private ButtonMode triggerRight;
        private ButtonMode dPadLeft;
        private ButtonMode dPadRight;
        private ButtonMode dPadUp;
        private ButtonMode dPadDown;

    #endregion

    public override void Update()
    {
        triggerLeft = Transition(triggerLeft, Input.GetAxis(LEFT_TRIGGER));
        triggerRight = Transition(triggerRight, Input.GetAxis(RIGHT_TRIGGER));

        // These are more complicated
        float dPadHorizontal = Input.GetAxis(DPAD_HORIZONTAL);
        float dPadVertical = Input.GetAxis(DPAD_VERTICAL);

        dPadLeft = TransitionDPad(dPadLeft, dPadHorizontal, false);
        dPadRight = TransitionDPad(dPadRight, dPadHorizontal, true);
        dPadUp = TransitionDPad(dPadUp, dPadVertical, true);
        dPadDown = TransitionDPad(dPadDown, dPadVertical, false);
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
    private ButtonMode TransitionDPad(ButtonMode mode, float axis, bool positive)
    {
        switch(mode)
        {
            case ButtonMode.UP:
                if (axis == 0)
                {
                    return ButtonMode.IDLE;
                }
                else if (positive)
                {
                    return (axis > 0) ? ButtonMode.DOWN : ButtonMode.IDLE;
                }
                else
                {
                    return (axis < 0) ? ButtonMode.DOWN : ButtonMode.IDLE;
                }
            case ButtonMode.DOWN:
                if (axis == 0)
                {
                    return ButtonMode.UP;
                }
                else if (positive)
                {
                    return (axis > 0) ? ButtonMode.HELD : ButtonMode.UP;
                }
                else
                {
                    return (axis < 0) ? ButtonMode.HELD : ButtonMode.UP;
                }
            case ButtonMode.HELD:
                if (axis == 0)
                {
                    return ButtonMode.UP;
                }
                else if (positive)
                {
                    return (axis > 0) ? ButtonMode.HELD : ButtonMode.UP;
                }
                else
                {
                    return (axis < 0) ? ButtonMode.HELD : ButtonMode.UP;
                }
            case ButtonMode.IDLE:
                if (axis == 0)
                {
                    return ButtonMode.IDLE;
                }
                else if (positive)
                {
                    return (axis > 0) ? ButtonMode.DOWN : ButtonMode.IDLE;
                }
                else
                {
                    return (axis < 0) ? ButtonMode.DOWN : ButtonMode.IDLE;
                }
            default:
                throw new System.Exception("IMPOSSIBRU!");
        }
    }

    #region Axes

        public override float GetLeftXAxis()
        {
            return Input.GetAxis(LEFT_AXIS_HORIZONTAL);
        }
        public override float GetLeftYAxis()
        {
            return Input.GetAxis(LEFT_AXIS_VERTICAL);
        }
        public override float GetRightXAxis()
        {
            return Input.GetAxis(RIGHT_AXIS_HORIZONTAL);
        }
        public override float GetRightYAxis()
        {
            return Input.GetAxis(RIGHT_AXIS_VERTICAL);
        }

    #endregion

    #region Primaries
    
        public override ButtonMode GetLeftPrimary()
        {
            return triggerLeft;
        }
        public override bool GetLeftPrimary(ButtonMode mode)
        {
            return triggerLeft == mode;
        }
    
        public override ButtonMode GetRightPrimary()
        {
            return triggerRight;
        }
        public override bool GetRightPrimary(ButtonMode mode)
        {
            return triggerRight == mode;
        }

    #endregion

    #region Secondaries
    
        public override ButtonMode GetLeftSecondary()
        {
            return GetModeHelper(LEFT_SECONDARY);
        }
        public override bool GetLeftSecondary(ButtonMode mode)
        {
            return GetKeyHelper(mode, LEFT_SECONDARY);
        }
    
        public override ButtonMode GetRightSecondary()
        {
            return GetModeHelper(RIGHT_SECONDARY);
        }
        public override bool GetRightSecondary(ButtonMode mode)
        {
            return GetKeyHelper(mode, RIGHT_SECONDARY);
        }

    #endregion

    #region Tertiaries
    
        public override ButtonMode GetLeftTertiary()
        {
            return GetModeHelper(LEFT_TERTIARY);
        }
        public override bool GetLeftTertiary(ButtonMode mode)
        {
            return GetKeyHelper(mode, LEFT_TERTIARY);
        }

        public override ButtonMode GetRightTertiary()
        {
            return GetModeHelper(RIGHT_TERTIARY);
        }
        public override bool GetRightTertiary(ButtonMode mode)
        {
            return GetKeyHelper(mode, RIGHT_TERTIARY);
        }

    #endregion

    #region Action Keys
    
        public override ButtonMode GetAction1()
        {
            return GetModeHelper(ACTION_1);
        }
        public override bool GetAction1(ButtonMode mode)
        {
            return GetKeyHelper(mode, ACTION_1);
        }
    
        public override ButtonMode GetAction2()
        {
            return GetModeHelper(ACTION_2);
        }
        public override bool GetAction2(ButtonMode mode)
        {
            return GetKeyHelper(mode, ACTION_2);
        }
    
        public override ButtonMode GetAction3()
        {
            return GetModeHelper(ACTION_3);
        }
        public override bool GetAction3(ButtonMode mode)
        {
            return GetKeyHelper(mode, ACTION_3);
        }
    
        public override ButtonMode GetAction4()
        {
            return GetModeHelper(ACTION_4);
        }
        public override bool GetAction4(ButtonMode mode)
        {
            return GetKeyHelper(mode, ACTION_4);
        }

    #endregion

    #region DPad
    
        public override ButtonMode GetDPadUp()
        {
            return dPadUp;
        }
        public override bool GetDPadUp(ButtonMode mode)
        {
            return dPadUp == mode;
        }

        public override ButtonMode GetDPadDown()
        {
            return dPadDown;
        }
        public override bool GetDPadDown(ButtonMode mode)
        {
            return dPadDown == mode;
        }
    
        public override ButtonMode GetDPadLeft()
        {
            return dPadLeft;
        }
        public override bool GetDPadLeft(ButtonMode mode)
        {
            return dPadLeft == mode;
        }
    
        public override ButtonMode GetDPadRight()
        {
            return dPadRight;
        }
        public override bool GetDPadRight(ButtonMode mode)
        {
            return dPadRight == mode;
        }

    #endregion

    #region Specials
    
        public override ButtonMode GetSelect()
        {
            return GetModeHelper(SELECT);
        }
        public override bool GetSelect(ButtonMode mode)
        {
            return GetKeyHelper(mode, SELECT);
        }
    

        public override ButtonMode GetStart()
        {
            return GetModeHelper(START);
        }
        public override bool GetStart(ButtonMode mode)
        {
            return GetKeyHelper(mode, START);
        }

    #endregion

    #region Haptics

        public override void Vibrate(VibrationTargets target, float intensity)
        {
            #if UNITY_STANDALONE_WIN
                switch(target)
                {
                    case VibrationTargets.LEFT:
                        Wrapper.VibrateLeft(0, intensity);
                        break;
                    case VibrationTargets.RIGHT:
                        Wrapper.VibrateRight(0, intensity);
                        break;
                    case VibrationTargets.BOTH:
                        Wrapper.VibrateLeft(0, intensity);
                        Wrapper.VibrateRight(0, intensity);
                        break;
                }
            #endif
        }

    #endregion
}
