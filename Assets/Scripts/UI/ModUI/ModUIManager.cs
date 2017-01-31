/**
 *  @author Cornelia Schultz
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ModUIManager : MonoBehaviour {

    // TEMPORARY ENUM
    public enum ModType
    {
        BATON,
        BLASTER,
        SEGWEY,
        FINGERPRINT
    }

    //// Unity Inspector Fields
    // @TODO : This should be a custom inspector to the Dictionary below.
    [SerializeField]
    private ModUIcon UIcon1, UIcon2, UIcon3, UIcon4;

    [SerializeField]
    private List<ModUIProperties> modUIProperties;

    //// Internal State
    private Dictionary<ModSpot, ModUIcon> modUIcons = new Dictionary<ModSpot, ModUIcon>();

    //// Unity State Functions

    void Awake()
    {
        // SEE ABOVE TODO:
        modUIcons.Add(ModSpot.Head, UIcon1);
        modUIcons.Add(ModSpot.Arm_L, UIcon2);
        modUIcons.Add(ModSpot.Arm_R, UIcon3);
        modUIcons.Add(ModSpot.Legs, UIcon4);
    }

    //// Manager Functions

    public void AttachMod(ModSpot spot, ModType type)
    {
        ModUIcon UIcon;
        modUIcons.TryGetValue(spot, out UIcon);
        UIcon.Attach(modUIProperties.Find((cmp) => { return cmp.type == type; }));
    }

    public void DetachMod(ModSpot spot)
    {
        ModUIcon UIcon;
        modUIcons.TryGetValue(spot, out UIcon);
        UIcon.Detach();
    }

    public void SwapMods(ModSpot spotA, ModSpot spotB)
    {
        Assert.AreNotEqual<ModSpot>(spotA, spotB, "Swapping ModSpots should be different!");

        ModUIcon UIconA, UIconB;
        modUIcons.TryGetValue(spotA, out UIconA);
        modUIcons.TryGetValue(spotB, out UIconB);

        modUIcons.Add(spotB, UIconA);
        modUIcons.Add(spotA, UIconB);

        UIconA.animator.SetTrigger(GetTrigger(spotB));
        UIconB.animator.SetTrigger(GetTrigger(spotA));
    }

    public void SetUIState(ModSpot spot, ModUIState state)
    {
        ModUIcon UIcon;
        modUIcons.TryGetValue(spot, out UIcon);
        UIcon.Apply(state);
    }

    //// Privates

    private string GetTrigger(ModSpot spot)
    {
        switch(spot)
        {
            case ModSpot.Head:
                return "MOVE_TO_HEAD";
            case ModSpot.Arm_L:
                return "MOVE_TO_LEFT";
            case ModSpot.Arm_R:
                return "MOVE_TO_RIGHT";
            case ModSpot.Legs:
                return "MOVE_TO_LEGS";
        }

        throw new System.Exception("IMPOSSIBRU!!");
    }
}
