/* 
 * Author Brandon Rivera-Melo
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModManager : MonoBehaviour
{
    class ModSocket {
        public Transform socket;
        public Mod mod;
        public ModSocket(Transform i_socket) {
            socket = i_socket;
        }
    }

    [SerializeField] Transform headSocket, leftArmSocket, rightArmSocket, legsSocket;
    [SerializeField] Stats playerStats;

    Dictionary<ModSpot, ModSocket> modSocketDictionary;
    ModSpot modToSwap;
    bool isOkToDropMod, isOkToSwapMods;    

    // Use this for initialization
    void Start()
    {
        modSocketDictionary = new Dictionary<ModSpot, ModSocket>();
        modSocketDictionary.Add(ModSpot.Head, new ModSocket(headSocket));
        modSocketDictionary.Add(ModSpot.Legs, new ModSocket(legsSocket));
        modSocketDictionary.Add(ModSpot.ArmL, new ModSocket(leftArmSocket));
        modSocketDictionary.Add(ModSpot.ArmR, new ModSocket(rightArmSocket));
        modToSwap = ModSpot.Default;
    }

    void Update() {
        if (isOkToDropMod) {
            CheckToDropMod();
        }
        SetModToSwap();
        if (isOkToSwapMods) {
            CheckToSwapMods();
        }
    }

    private ModSpot GetCommandedModSpot()
    {        
        if (Input.GetButtonDown(Strings.UP))
        {
            return ModSpot.Head;
        }
        if (Input.GetButtonDown(Strings.DOWN))
        {
            return ModSpot.Legs;
        }
        if (Input.GetButtonDown(Strings.LEFT))
        {
            return ModSpot.ArmL;
        }
        if (Input.GetButtonDown(Strings.RIGHT))
        {
            return ModSpot.ArmR;
        }        
        return ModSpot.Default;
    }

    void CheckToDropMod()
    {
        ModSpot selectedMod = GetCommandedModSpot();
        if (Input.GetButton(Strings.PREPARETOPICKUPORDROP)) {
            ModSpot spotSelected = GetCommandedModSpot();
            if (spotSelected != ModSpot.Default && modSocketDictionary[spotSelected].mod != null) {
                Detach(spotSelected);
            }
        }
    }

    private void SetModToSwap() {
        if (Input.GetButton(Strings.PREPARETOSWAP) && modToSwap==ModSpot.Default){
            modToSwap = GetCommandedModSpot();
            StartCoroutine(DelayIsOkToSwapMods());            
        }
        if (Input.GetButtonUp(Strings.PREPARETOSWAP)) {
            modToSwap = ModSpot.Default;
        }
    }

    private void CheckToSwapMods() {
        if (modToSwap!=ModSpot.Default) {
            ModSpot secondMod = GetCommandedModSpot();
            if (secondMod != ModSpot.Default) {
                SwapMods(modToSwap, secondMod);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == Strings.MOD)
        {
            CheckToAttachMod(other.GetComponent<Mod>());
        }
    }

    private void CheckToAttachMod(Mod modToAttach) {
        if (Input.GetButton(Strings.PREPARETOPICKUPORDROP)) {
            ModSpot commandedModSpot = GetCommandedModSpot();

            if (commandedModSpot != ModSpot.Default &&
                modSocketDictionary[commandedModSpot].mod != modToAttach) {

                Attach(commandedModSpot, modToAttach);
            }
        }
    }

    private void Attach(ModSpot spot, Mod mod)
    {
        if (modSocketDictionary[spot].mod != null)
        {
            Detach(spot);            
        }

        ModUIManager.Instance.AttachMod(spot, mod.getModType());
        mod.transform.SetParent(modSocketDictionary[spot].socket);
        mod.transform.localPosition = Vector3.zero;
        mod.transform.localRotation = Quaternion.identity;
        modSocketDictionary[spot].mod = mod;
        mod.AttachAffect(ref playerStats);
        StartCoroutine(DelayIsOkToDropMod());
    }

    IEnumerator DelayIsOkToDropMod() {
        isOkToDropMod = false;
        yield return new WaitForEndOfFrame();
        isOkToDropMod = true;
    }
    IEnumerator DelayIsOkToSwapMods() {
        isOkToSwapMods = false;
        yield return new WaitForEndOfFrame();
        isOkToSwapMods = true;
    }

    private void Detach(ModSpot spot)
    {
        if (modSocketDictionary[spot].mod != null)
        {
            ModUIManager.Instance.DetachMod(spot);
            modSocketDictionary[spot].mod.transform.SetParent(null);
            modSocketDictionary[spot].mod.DetachAffect();
            modSocketDictionary[spot].mod = null;
        }
    }

    private void SwapMods(ModSpot sourceSpot, ModSpot targetSpot)
    {
        ModUIManager.Instance.SwapMods(sourceSpot, targetSpot);
        Mod tempSourceMod = null;
        Mod tempTargetMod = null;
        if (modSocketDictionary[sourceSpot].mod != null)
        {
            tempSourceMod = modSocketDictionary[sourceSpot].mod;
            Detach(sourceSpot);
        }
        if (modSocketDictionary[targetSpot].mod != null)
        {
            tempTargetMod = modSocketDictionary[targetSpot].mod;
            Detach(targetSpot);
        }
        if (tempSourceMod != null)
        {
            Attach(targetSpot, tempSourceMod);            
        }
        if (tempTargetMod != null)
        {
            Attach(sourceSpot, tempTargetMod);            
        }
        modToSwap = ModSpot.Default;
    }

    private ModSpot GetModSpot(Mod mod)
    {
        ModSpot spot = ModSpot.Default;
        foreach (KeyValuePair<ModSpot, ModSocket> pair in modSocketDictionary)
        {
            if (pair.Value.mod == mod)
            {
                spot = pair.Key;
                break;
            }
        }
        return spot;
    }
}
