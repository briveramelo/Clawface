/**
 *  @author Cornelia Schultz
 */

using UnityEngine;
using UnityEngine.UI;

public class ModUIcon : MonoBehaviour {

    #region Private Constants
        private const float MOVE_TIME = 0.15F;
    #endregion

    #region Serlialized Unity Fields
    #pragma warning disable 0649 // Turn off never assigned warning

        [SerializeField]
        private GameObject self;
        [SerializeField]
        private Image icon;

        [SerializeField]
        private Image highlightRing;

        // These fields are more of "constants" than parameters used by an individual icon.
        [SerializeField]
        private Sprite defaultArmLeftSprite,
            defaultArmRightSprite, defaultLegsSprite;
        [SerializeField]
        private Vector3 armLeftVector, armRightVector, legsVector;

#pragma warning restore 0649 // Turn on never assigned warning
    #endregion

    #region Private Fields

        private ModUIProperties activeProperties;
        private ModSpot currentSpot;
        private ModUIState currentState;

    #endregion

    #region Public Interface

        public void Attach(ModUIProperties properties)
        {
            activeProperties = properties;
            UpdateAppearance();
        }

        public void Detach()
        {
            activeProperties = null;
            UpdateAppearance();
        }

        public void Relocate(ModSpot spot)
        {
            currentSpot = spot;
            UpdateAppearance();
            UpdateLocation();
        }

        public void Apply(ModUIState state)
        {
            currentState = state;
            UpdateAppearance();
        }

    #endregion

    #region Private Interface

        private void UpdateAppearance()
        {
            // Assign a ModIcon to display
            if (activeProperties != null)
            {
                icon.sprite = activeProperties.sprite;
            }
            else
            {
                switch (currentSpot)
                {
                    case ModSpot.ArmL:
                        icon.sprite = defaultArmLeftSprite;
                        break;
                    case ModSpot.ArmR:
                        icon.sprite = defaultArmRightSprite;
                        break;
                    //case ModSpot.Legs:
                    //    icon.sprite = defaultLegsSprite;
                    //    break;
                    default:
                        throw new System.SystemException("IMPOSSIBRU!!!");
                }
            }

            // Apply the ModUIState
            currentState.ApplyHiglightRing(ref highlightRing);
        }
        private void UpdateLocation()
        {
            switch (currentSpot)
            {
                case ModSpot.ArmL:
                    iTween.MoveTo(self, iTween.Hash(
                        "position", armLeftVector,
                        "isLocal", true,
                        "time", MOVE_TIME
                        ));
                    break;
                case ModSpot.ArmR:
                    iTween.MoveTo(self, iTween.Hash(
                        "position", armRightVector,
                        "isLocal", true,
                        "time", MOVE_TIME
                        ));
                    break;
                //case ModSpot.Legs:
                //    iTween.MoveTo(self, iTween.Hash(
                //        "position", legsVector,
                //        "isLocal", true,
                //        "time", MOVE_TIME
                //        ));
                //    break;
                default:
                    throw new System.SystemException("IMPOSSIBRU!!!");
            }
        }

    #endregion
}
