using UnityEngine;
using UnityEngine.UI;

public class TelevatorUI : MonoBehaviour {

    // An enum to make things a little easier for now
    public enum Floor
    {
        ARMORY,
        TRIALS,
        ARENA
    }

    //// Unity Inspector Fields
    [SerializeField]
    private Image arena, trials, armory;

    //// Constants
    private static Color UNSELECTED = new Color(0x32 / 255.0F, 0xD2 / 255.0F, 0xBE / 255.0F);
    private static Color SELECTED = new Color(0x32 / 255.0F, 0x83 / 255.0F, 0xD2 / 255.0F);

    //// Elevator UI Public Interface
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void SelectFloor(Floor floor)
    {
        arena.color = floor == Floor.ARENA ? SELECTED : UNSELECTED;
        trials.color = floor == Floor.TRIALS ? SELECTED : UNSELECTED;
        armory.color = floor == Floor.ARMORY ? SELECTED : UNSELECTED;
    }
}
