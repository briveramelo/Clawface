/**
 *  @author Cornelia Schultz
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ModUIManager : MonoBehaviour {

    public static ModUIManager Instance;


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
        Instance = this;
        // SEE ABOVE TODO:
        modUIcons.Add(ModSpot.Head, UIcon1);
        modUIcons.Add(ModSpot.ArmL, UIcon2);
        modUIcons.Add(ModSpot.ArmR, UIcon3);
        modUIcons.Add(ModSpot.Legs, UIcon4);

        UIcon1.Relocate(ModSpot.Head);
        UIcon2.Relocate(ModSpot.ArmL);
        UIcon3.Relocate(ModSpot.ArmR);
        UIcon4.Relocate(ModSpot.Legs);
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
        Assert.AreNotEqual(spotA, spotB, "Swapping ModSpots should be different!");

        ModUIcon UIconA, UIconB;
        modUIcons.TryGetValue(spotA, out UIconA);
        modUIcons.TryGetValue(spotB, out UIconB);

        modUIcons.Remove(spotA);
        modUIcons.Remove(spotB);

        modUIcons.Add(spotB, UIconA);
        modUIcons.Add(spotA, UIconB);

        UIconA.Relocate(spotB);
        UIconB.Relocate(spotA);
    }

    public void SetUIState(ModSpot spot, ModUIState state)
    {
        ModUIcon UIcon;
        modUIcons.TryGetValue(spot, out UIcon);
        UIcon.Apply(state);
    }
}
