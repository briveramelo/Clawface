/**
*  @author Cornelia Schultz
*/

using UnityEngine;

public class KeyboardController : IController {

    #region Constants

        static readonly string KEYBOARD_VERTICAL = "KEYBOARD_VERTICAL";
        static readonly string KEYBOARD_HORIZONTAL = "KEYBOARD_HORIZONTAL";
        static readonly string MOUSE_VERTICAL = "MOUSE_VERTICAL";
        static readonly string MOUSE_HORIZONTAL = "MOUSE_HORIZONTAL";

        static readonly KeyCode RIGHT_PRIMARY = KeyCode.Mouse1;
        static readonly KeyCode LEFT_PRIMARY = KeyCode.Mouse0;
        static readonly KeyCode RIGHT_SECONDARY = KeyCode.Space;
        static readonly KeyCode LEFT_SECONDARY = KeyCode.LeftShift;
        static readonly KeyCode RIGHT_TERTIARY = KeyCode.E;
        static readonly KeyCode LEFT_TERTIARY = KeyCode.Q;

        static readonly KeyCode ACTION_1 = KeyCode.Alpha1;
        static readonly KeyCode ACTION_2 = KeyCode.Alpha2;
        static readonly KeyCode ACTION_3 = KeyCode.Alpha3;
        static readonly KeyCode ACTION_4 = KeyCode.Alpha4;

        static readonly string KEYBOARD_HORIZONTAL_ALTERNATE = "KEYBOARD_HORIZONTAL_ALTERNATE";
        static readonly string KEYBOARD_VERTICAL_ALTERNATE = "KEYBOARD_VERTICAL_ALTERNATE";

        static readonly KeyCode SELECT = KeyCode.Return;
        static readonly KeyCode START = KeyCode.Escape;

    #endregion

    #region Internal State

        private ButtonMode navLeft = ButtonMode.IDLE;
        private ButtonMode navRight = ButtonMode.IDLE;
        private ButtonMode navUp = ButtonMode.IDLE;
        private ButtonMode navDown = ButtonMode.IDLE;

    #endregion

    public override void Update()
    {
        float alternateHorizontal = Input.GetAxis(KEYBOARD_HORIZONTAL_ALTERNATE);
        float alternateVertical = Input.GetAxis(KEYBOARD_VERTICAL_ALTERNATE);

        navLeft = AxisToDPad(navLeft, alternateHorizontal, false);
        navRight = AxisToDPad(navRight, alternateHorizontal, true);
        navUp = AxisToDPad(navUp, alternateVertical, true);
        navDown = AxisToDPad(navDown, alternateVertical, false);
    }

    #region Axes

        public override float GetLeftXAxis()
        {
            return Input.GetAxis(KEYBOARD_HORIZONTAL);
        }
        public override float GetLeftYAxis()
        {
            return Input.GetAxis(KEYBOARD_VERTICAL);
        }
        public override float GetRightXAxis()
        {
            Vector3 mouse = Input.mousePosition -
                new Vector3(Screen.width / 2.0F, Screen.height / 2.0F, 0.0F);
            return Vector3.Dot(mouse, new Vector3(1.0F, 0.0F, 0.0F));
        }
        public override float GetRightYAxis()
        {
            Vector3 mouse = Input.mousePosition -
                new Vector3(Screen.width / 2.0F, Screen.height / 2.0F, 0.0F);
            return Vector3.Dot(mouse, new Vector3(0.0F, 1.0F, 0.0F));
        }

    #endregion

    #region Primaries
    
        public override ButtonMode GetLeftPrimary()
        {
            return GetModeHelper(LEFT_PRIMARY);
        }
        public override bool GetLeftPrimary(ButtonMode mode)
        {
            return GetKeyHelper(mode, LEFT_PRIMARY);
        }
    
        public override ButtonMode GetRightPrimary()
        {
            return GetModeHelper(RIGHT_PRIMARY);
        }
        public override bool GetRightPrimary(ButtonMode mode)
        {
            return GetKeyHelper(mode, RIGHT_PRIMARY);
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
            return navUp;
        }
        public override bool GetDPadUp(ButtonMode mode)
        {
            return navUp == mode;
        }

        public override ButtonMode GetDPadDown()
        {
            return navDown;
        }
        public override bool GetDPadDown(ButtonMode mode)
        {
            return navDown == mode;
        }
    
        public override ButtonMode GetDPadLeft()
        {
            return navLeft;
        }
        public override bool GetDPadLeft(ButtonMode mode)
        {
            return navLeft == mode;
        }
    
        public override ButtonMode GetDPadRight()
        {
            return navRight;
        }
        public override bool GetDPadRight(ButtonMode mode)
        {
            return navRight == mode;
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
            // We can't do this... :P
        }

    #endregion
}
