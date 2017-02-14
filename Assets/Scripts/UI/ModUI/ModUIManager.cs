/**
 *  @author Cornelia Schultz
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ModUIManager : Singleton<ModUIManager> {

    protected ModUIManager() { }

    //// Unity Inspector Fields
    [SerializeField]
    private GameObject UIconPrefab;

    [SerializeField]
    private Transform anchor;

    [SerializeField]
    private List<ModUIProperties> modUIProperties;

    //// Internal State
    private Dictionary<ModSpot, ModUIcon> modUIcons = new Dictionary<ModSpot, ModUIcon>();

    //// Unity State Functions
    protected override void Awake()
    {
        base.Awake();
        // UIcon Instantiation and Assignment
        foreach(ModSpot spot in Enum.GetValues(typeof(ModSpot))) {
            if (spot == ModSpot.Default) break;

            GameObject obj = Instantiate(UIconPrefab);
            obj.transform.SetParent(anchor, false);
            ModUIcon icon = obj.GetComponent<ModUIcon>();
            modUIcons.Add(spot, icon);
            icon.Relocate(spot);
        }
    }

    //// Manager Functions
    public void AttachMod(ModSpot spot, ModType type)
    {
        Assert.AreNotEqual(spot, ModSpot.Default);
        ModUIcon UIcon;
        modUIcons.TryGetValue(spot, out UIcon);
        UIcon.Attach(modUIProperties.Find((cmp) => { return cmp.type == type; }));
    }

    public void DetachMod(ModSpot spot)
    {
        Assert.AreNotEqual(spot, ModSpot.Default);
        ModUIcon UIcon;
        modUIcons.TryGetValue(spot, out UIcon);
        UIcon.Detach();
    }

    public void SwapMods(ModSpot spotA, ModSpot spotB)
    {
        Assert.AreNotEqual(spotA, spotB, "Swapping ModSpots should be different!");
        Assert.AreNotEqual(spotA, ModSpot.Default);
        Assert.AreNotEqual(spotB, ModSpot.Default);

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
        Assert.AreNotEqual(spot, ModSpot.Default);
        ModUIcon UIcon;
        modUIcons.TryGetValue(spot, out UIcon);
        UIcon.Apply(state);
    }
}
