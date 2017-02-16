/**
 *  @author Cornelia Schultz
 */

public interface IController {

    // Used to instruct the controller to update its internal state
    // if necessary
    void Update();

    #region Axes

        float GetLeftXAxis();
        float GetLeftYAxis();
        float GetRightXAxis();
        float GetRightYAxis();

    #endregion

    #region Primaries

        bool GetLeftPrimaryDown();
        bool GetLeftPrimaryHeld();
        bool GetLeftPrimaryUp();
        bool GetLeftPrimaryIdle();
    
        bool GetRightPrimaryDown();
        bool GetRightPrimaryHeld();
        bool GetRightPrimaryUp();
        bool GetRightPrimaryIdle();

    #endregion

    #region Secondaries
    
        bool GetLeftSecondaryDown();
        bool GetLeftSecondaryHeld();
        bool GetLeftSecondaryUp();
        bool GetLeftSecondaryIdle();

        bool GetRightSecondaryDown();
        bool GetRightSecondaryHeld();
        bool GetRigthSecondaryUp();
        bool GetRightSecondaryIdle();

    #endregion

    #region Tertiaries

        bool GetLeftTertiaryDown();
        bool GetLeftTertiary();
        bool GetLeftTertiaryUp();
        bool GetLeftTertiaryIdle();

        bool GetRigthTertiaryDown();
        bool GetRightTertiaryHeld();
        bool GetRightTertiaryUp();
        bool GetRightTertiaryIdle();

    #endregion

    #region Action Keys

        bool GetAction1Down();
        bool GetAction1Held();
        bool GetAction1Up();
        bool GetAction1Idle();

        bool GetAction2Down();
        bool GetAction2Held();
        bool GetAction2Up();
        bool GetAction2Idle();

        bool GetAction3Down();
        bool GetAction3Held();
        bool GetAction3Up();
        bool GetAction3Idle();

        bool GetAction4Down();
        bool GetAction4Held();
        bool GetAction4Up();
        bool GetAction4Idle();

    #endregion

    #region DPad

        bool GetDPadUpDown();
        bool GetDPadUpHeld();
        bool GetDPadUpUp();
        bool GetDPadUpIdle();

        bool GetDPadDownDown();
        bool GetDPadDownHeld();
        bool GetDPadDownUp();
        bool GetDPadDownIdle();

        bool GetDPadLeftDown();
        bool GetDPadLeftHeld();
        bool GetDPadLeftUp();
        bool GetDPadLeftIdle();

        bool GetDPadRightDown();
        bool GetDPadRightHeld();
        bool GetDPadRightUp();
        bool GetDPadRightIdle();

    #endregion

    #region Specials

        bool GetSelectDown();
        bool GetSelectHeld();
        bool GetSelectUp();
        bool GetSelectIdle();

        bool GetStartDown();
        bool GetStartHeld();
        bool GetStartUp();
        bool GetStartIdle();

    #endregion
}
