/**
 *  @author Cornelia Schultz
 */

using UnityEngine;

public abstract class IController {
    
    // Used to instruct the controller to update its internal state
    // if necessary
    public abstract void Update();

    #region Protected Interface

        // A simple Helper.. which is no longer used. :P
        public bool GetKeyHelper(ButtonMode mode, KeyCode code)
        {
            switch(mode)
            {
                case ButtonMode.DOWN:
                    return Input.GetKeyDown(code);
                case ButtonMode.HELD:
                    return Input.GetKey(code);
                case ButtonMode.UP:
                    return Input.GetKeyUp(code);
                case ButtonMode.IDLE:
                    return !Input.GetKey(code);
                default:
                    throw new System.Exception("IMPOSSIBRU!");
            }
        }

        // This one has taken over :P
        public ButtonMode GetModeHelper(KeyCode code)
        {
            if (Input.GetKeyDown(code))
                return ButtonMode.DOWN;
            else if (Input.GetKeyUp(code))
                return ButtonMode.UP;
            else if (Input.GetKey(code))
                return ButtonMode.HELD;
            else
                return ButtonMode.IDLE;
        }

    #endregion

    #region Axes

        public abstract float GetLeftXAxis();
        public abstract float GetLeftYAxis();
        public abstract float GetRightXAxis();
        public abstract float GetRightYAxis();

    #endregion

    #region Primaries

        public abstract ButtonMode GetLeftPrimary();
        public abstract bool GetLeftPrimary(ButtonMode mode);

        public abstract ButtonMode GetRightPrimary();
        public abstract bool GetRightPrimary(ButtonMode mode);

    #endregion

    #region Secondaries

        public abstract ButtonMode GetLeftSecondary();
        public abstract bool GetLeftSecondary(ButtonMode mode);

        public abstract ButtonMode GetRightSecondary();
        public abstract bool GetRightSecondary(ButtonMode mode);

    #endregion

    #region Tertiaries

        public abstract ButtonMode GetLeftTertiary();
        public abstract bool GetLeftTertiary(ButtonMode mode);

        public abstract ButtonMode GetRightTertiary();
        public abstract bool GetRightTertiary(ButtonMode mode);

    #endregion

    #region Action Keys

        public abstract ButtonMode GetAction1();
        public abstract bool GetAction1(ButtonMode mode);

        public abstract ButtonMode GetAction2();
        public abstract bool GetAction2(ButtonMode mode);

        public abstract ButtonMode GetAction3();
        public abstract bool GetAction3(ButtonMode mode);

        public abstract ButtonMode GetAction4();
        public abstract bool GetAction4(ButtonMode mode);

    #endregion

    #region DPad

        public abstract ButtonMode GetDPadUp();
        public abstract bool GetDPadUp(ButtonMode mode);

        public abstract ButtonMode GetDPadDown();
        public abstract bool GetDPadDown(ButtonMode mode);

        public abstract ButtonMode GetDPadLeft();
        public abstract bool GetDPadLeft(ButtonMode mode);

        public abstract ButtonMode GetDPadRight();
        public abstract bool GetDPadRight(ButtonMode mode);

    #endregion

    #region Specials

        public abstract ButtonMode GetSelect();
        public abstract bool GetSelect(ButtonMode mode);

        public abstract ButtonMode GetStart();
        public abstract bool GetStart(ButtonMode mode);

    #endregion

    #region Haptics

        public abstract void Vibrate(VibrationTargets target, float intensity);

    #endregion
}
