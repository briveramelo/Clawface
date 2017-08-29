/**
 *  @author Cornelia Schultz
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ModUIManager : MonoBehaviour {


    #region Serialized Unity Fields
    #pragma warning disable 0649 // Turn off never assigned warning
        [SerializeField]
        private ModUIcon leftArm, rightArm;

        [SerializeField]
        private Transform anchor;

        [SerializeField]
        private List<ModUIProperties> modUIProperties;
    #pragma warning restore 0649 // Turn on never assigned warning
    #endregion

    #region Public Interface
        public void AttachMod(ModSpot spot, ModType type)
        {
            Assert.AreNotEqual(spot, ModSpot.Default);
            ModUIcon UIcon = SelectBySpot(spot);
            UIcon.Attach(modUIProperties.Find((cmp) => { return cmp.type == type; }));
        }

        public void DetachMod(ModSpot spot)
        {
            Assert.AreNotEqual(spot, ModSpot.Default);
            ModUIcon UIcon = SelectBySpot(spot);
            UIcon.Detach();
        }

        public void SetUIState(ModSpot spot, ModUIState state)
        {
            Assert.AreNotEqual(spot, ModSpot.Default);
            ModUIcon UIcon = SelectBySpot(spot);
            UIcon.Apply(state);
        }
    #endregion

    #region Protected Interface
        protected ModUIManager() { }
    #endregion

    #region Private Interface

    private ModUIcon SelectBySpot(ModSpot spot)
    {
        switch (spot)
        {
            case ModSpot.ArmL:
                return leftArm;
            case ModSpot.ArmR:
                return rightArm;
        }

        return null; // we didn't find a match
    }

    #endregion
}
