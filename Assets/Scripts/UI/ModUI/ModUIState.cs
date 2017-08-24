/**
 *  @author Cornelia Schultz
 */

using UnityEngine;
using UnityEngine.UI;

public struct ModUIState {

    #region Public Constants

        // @TODO : This is probably not the best way to do this...
        public static readonly ModUIState IDLE = new ModUIState();
        public static readonly ModUIState SELECTED =
            (new ModUIState()).SetHighlightColor(true, Color.blue);
        public static readonly ModUIState SWAPPING =
            (new ModUIState()).SetHighlightColor(true, Color.yellow);
        public static readonly ModUIState ACTIVATED =
            (new ModUIState()).SetHighlightColor(true, Color.red);

    #endregion

    #region Private Fields

        // Highlight Ring State
        private bool highlightRingEnabled;
        private Color highlightRingColor;

    #endregion

    #region Public Interface

        //// Builder Pattern-like Creation of ModUIState objects
        public ModUIState SetHighlightColor(bool enabled, Color color)
        {
            highlightRingEnabled = enabled;
            highlightRingColor = color;
            return this;
        }

        //// Apply state to appropriate objects
        public void ApplyHiglightRing(ref Image highlightRing)
        {
            highlightRing.enabled = highlightRingEnabled;
            highlightRing.color = highlightRingColor;
        }

    #endregion
}
