/**
 *  @author Cornelia Schultz
 */

public interface IController {

    #region Axes

        float GetMovementXAxis();
        float GetMovementYAxis();
        float GetLookXAxis();
        float GetLookYAxis();

    #endregion

    #region Primaries

        bool GetLeftPrimaryDown();
        bool GetLeftPrimaryHeld();
        bool GetLeftPrimaryUp();
    
        bool GetRightPrimaryDown();
        bool GetRightPrimaryHeld();
        bool GetRightPrimaryUp();

    #endregion

    #region Secondaries
    
        bool GetLeftSecondaryDown();
        bool GetLeftSecondaryHeld();
        bool GetLeftSecondaryUp();

        bool GetRightSecondaryDown();
        bool GetRightSecondaryHeld();
        bool GetRigthSecondaryUp();

    #endregion

    #region Tertiaries

        bool GetLeftTertiaryDown();
        bool GetLeftTertiary();
        bool GetLeftTertiaryUp();

        bool GetRigthTertiaryDown();
        bool GetRightTertiaryHeld();
        bool GetRightTertiaryUp();

    #endregion

    #region Action Keys

        bool GetAction1Down();
        bool GetAction1Held();
        bool GetAction1Up();

        bool GetAction2Down();
        bool GetAction2Held();
        bool GetAction2Up();

        bool GetAction3Down();
        bool GetAction3Held();
        bool GetAction3Up();

        bool GetAction4Down();
        bool GetAction4Held();
        bool GetAction4Up();

    #endregion

    #region DPad

        bool GetDPadUpDown();
        bool GetDPadUpHeld();
        bool GetDPadUpUp();

        bool GetDPadDownDown();
        bool GetDPadDownHeld();
        bool GetDPadDownUp();

        bool GetDPadLeftDown();
        bool GetDPadLeftHeld();
        bool GetDPadLeftUp();

        bool GetDPadRightDown();
        bool GetDPadRightHeld();
        bool GetDPadRightUp();

    #endregion

    #region Specials

        bool GetBackDown();
        bool GetBackHeld();
        bool GetBackUp();

        bool GetStartDown();
        bool GetStartHeld();
        bool GetStartUp();

    #endregion
}
