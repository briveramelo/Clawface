/**
 *  @author Cornelia Schultz
 */

using UnityEngine;

public class ModUIProperties : MonoBehaviour {

    #region Serialized Unity Fields
    #pragma warning disable 0649 // Turn off never assigned warning

        [SerializeField]
        private ModType typeField;
        [SerializeField]
        private Sprite spriteField;

#pragma warning restore 0649 // Turn on never assigned warning
    #endregion

    #region Public Fields

        public ModType type
        {
            get { return typeField; }
        }
        public Sprite sprite
        {
            get { return spriteField; }
        }

    #endregion
}
