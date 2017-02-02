using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModManager : MonoBehaviour
{

    Dictionary<ModSpot, Mod> modDictionary;
    Mod currentModInReach;
    ModSpot modToSwapOrDrop;    

    // Use this for initialization
    void Start()
    {
        modDictionary = new Dictionary<ModSpot, Mod>();
        modDictionary.Add(ModSpot.Head, null);
        modDictionary.Add(ModSpot.Legs, null);
        modDictionary.Add(ModSpot.ArmL, null);
        modDictionary.Add(ModSpot.ArmR, null);
        currentModInReach = null;
        modToSwapOrDrop = ModSpot.Default;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPickupCommand();
        CheckForDropCommand();
    }

    void CheckForPickupCommand()
    {
        if (Input.GetButton(Strings.PREPARETOPICKUP))
        {
            if (Input.GetButton(Strings.UP))
            {
                Attach(ModSpot.Head);
            }
            if (Input.GetButton(Strings.DOWN))
            {
                Attach(ModSpot.Legs);
            }
            if (Input.GetButton(Strings.LEFT))
            {
                Attach(ModSpot.ArmL);
            }
            if (Input.GetButton(Strings.RIGHT))
            {
                Attach(ModSpot.ArmR);
            }
        }
    }

    void CheckForDropCommand()
    {
        if (Input.GetButton(Strings.PREPARETODROP))
        {
            ModSpot spotSelected = ModSpot.Default;
            if (Input.GetButton(Strings.UP))
            {
                spotSelected = ModSpot.Head;
            }
            if (Input.GetButton(Strings.DOWN))
            {
                spotSelected = ModSpot.Legs;
            }
            if (Input.GetButton(Strings.LEFT))
            {
                spotSelected = ModSpot.ArmL;
            }
            if (Input.GetButton(Strings.RIGHT))
            {
                spotSelected = ModSpot.ArmR;
            }
            if (modToSwapOrDrop != ModSpot.Default && spotSelected == ModSpot.Default)
            {
                if (modToSwapOrDrop != spotSelected)
                {
                    SwitchMods(modToSwapOrDrop, spotSelected);
                }
                else
                {
                    Detach(modToSwapOrDrop);
                }
                modToSwapOrDrop = ModSpot.Default;
            }else
            {
                modToSwapOrDrop = spotSelected;
            }
        }
        if (Input.GetButtonUp(Strings.PREPARETODROP))
        {
            modToSwapOrDrop = ModSpot.Default;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == Strings.MOD)
        {
            currentModInReach = other.gameObject.GetComponent<Mod>();
        }
    }

    private void Attach(ModSpot spot)
    {
        Attach(spot, currentModInReach);
        currentModInReach = null;
    }

    private void Attach(ModSpot spot, Mod mod)
    {
        //TODO: Code for attaching mod to body
        Mod modInSpot;
        modDictionary.TryGetValue(spot, out modInSpot);
        if (modInSpot != null)
        {
            Detach(spot);            
        }
        modDictionary.Add(spot, mod);
        mod.AttachAffect();
    }

    private void Detach(ModSpot spot)
    {
        //TODO: Code for detaching mod to body
        Mod modInSpot;
        modDictionary.TryGetValue(spot, out modInSpot);
        if (modInSpot != null)
        {
            modInSpot.DetachAffect();
            modDictionary.Add(spot, null);
        }
    }

    private void SwitchMods(ModSpot sourceSpot, ModSpot targetSpot)
    {
        Mod sourceMod;
        modDictionary.TryGetValue(sourceSpot, out sourceMod);
        if(sourceMod != null)
        {
            Mod targetMod;
            modDictionary.TryGetValue(targetSpot, out targetMod);
            if (targetMod != null)
            {
                Detach(sourceSpot);
                Detach(targetSpot);
                Attach(sourceSpot, targetMod);
                Attach(targetSpot, sourceMod);
            }
        }
        modToSwapOrDrop = ModSpot.Default;
    }

    private ModSpot GetModSpot(Mod mod)
    {
        ModSpot spot = ModSpot.Default;
        foreach (KeyValuePair<ModSpot, Mod> pair in modDictionary)
        {
            if (pair.Value == mod)
            {
                spot = pair.Key;
                break;
            }
        }
        return spot;
    }
}
