using UnityEngine;
using UnityEngine.UI;

public class ElevatorUI : MonoBehaviour {

    // An enum to make things a little easier for now
    public enum Floor
    {
        ONE,
        TWO,
        THREE
    }

    //// Unity Inspector Fields
    [SerializeField]
    private Image floor3, floor2, floor1;

    //// Constants
    private static Color UNSELECTED = new Color(0x32 / 255.0F, 0xD2 / 255.0F, 0xBE / 255.0F);
    private static Color SELECTED = new Color(0x32 / 255.0F, 0x83 / 255.0F, 0xD2 / 255.0F);

    //// Elevator UI Public Interface
    public void SelectFloor(Floor floor)
    {
        floor3.color = floor == Floor.THREE ? SELECTED : UNSELECTED;
        floor2.color = floor == Floor.TWO ? SELECTED : UNSELECTED;
        floor1.color = floor == Floor.ONE ? SELECTED : UNSELECTED;
    }
}
