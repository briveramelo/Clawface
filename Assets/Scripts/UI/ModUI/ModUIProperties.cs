/**
 *  @author Cornelia Schultz
 */

using UnityEngine;

public class ModUIProperties : MonoBehaviour {

    //// Unity Inspector Fields
    [SerializeField]
    private ModType typeField;
    [SerializeField]
    private Sprite spriteField;

    //// Accessor Properties
    public ModType type
    {
        get { return typeField; }
    }
    public Sprite sprite
    {
        get { return spriteField; }
    }
}
