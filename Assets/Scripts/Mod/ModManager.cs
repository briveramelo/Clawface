﻿/* 
 * Author Brandon Rivera-Melo
 */
using System;
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

    [SerializeField]
    private PlayerMovement playerMovement;

    Dictionary<ModSpot, ModSocket> modSocketDictionary;
    ModSpot modToSwap;
    ModSpot lastModToSwap;
    bool isOkToDropMod = true;
    bool isOkToSwapMods = true;

    [SerializeField]
    float triggerThreshold;

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
        CheckToActivateMod();
    }

    private void CheckToActivateMod()
    {
        if(!Input.GetButton(Strings.PREPARETOPICKUPORDROP) && !Input.GetButton(Strings.PREPARETOSWAP))
        {
            ModSpot spot = GetCommandedModSpot();
            if (spot != ModSpot.Default)
            {                
                if (modSocketDictionary[spot].mod != null)
                {
                    playerMovement.PlayAnimation(modSocketDictionary[spot].mod);
                }
            }
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
        if (Input.GetButtonDown(Strings.LEFT) || Input.GetAxis(Strings.LEFTTRIGGER) != triggerThreshold)
        {
            return ModSpot.ArmL;
        }
        if (Input.GetButtonDown(Strings.RIGHT) || Input.GetAxis(Strings.RIGHTTRIGGER) != triggerThreshold)
        {
            return ModSpot.ArmR;
        }        
        return ModSpot.Default;
    }

    void CheckToDropMod()
    {   
        if (Input.GetButton(Strings.PREPARETOPICKUPORDROP)) {
            ModSpot spotSelected = GetCommandedModSpot();            
            if (spotSelected != ModSpot.Default && modSocketDictionary[spotSelected].mod != null) {
                Detach(spotSelected);
            }
        }
    }

    private void SetModToSwap() {
        if (Input.GetButton(Strings.PREPARETOSWAP) && modToSwap==ModSpot.Default){
            StartCoroutine(DelayIsOkToSwapMods());
            modToSwap = GetCommandedModSpot();
            if (modToSwap!=ModSpot.Default) {
                ModUIManager.Instance.SetUIState(modToSwap, ModUIState.SELECTED);
            }
        }
        if (Input.GetButtonUp(Strings.PREPARETOSWAP)) {
            SetAllModUIToIdle();            
        }
    }

    void SetAllModUIToIdle() {
        foreach (ModSpot modSpot in Enum.GetValues(typeof(ModSpot))) {
            if (modSpot!=ModSpot.Default) {
                ModUIManager.Instance.SetUIState(modSpot, ModUIState.IDLE);
            }
        }
        modToSwap = ModSpot.Default;
    }

    private void CheckToSwapMods() {
        if (modToSwap!=ModSpot.Default) {
            ModSpot secondMod = GetCommandedModSpot();
            if (secondMod != ModSpot.Default && secondMod != modToSwap) {
                SwapMods(modToSwap, secondMod);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == Strings.MOD)
        {
            if (Input.GetButton(Strings.PREPARETOPICKUPORDROP))
            {
                ModSpot commandedModSpot = GetCommandedModSpot();

                if (commandedModSpot != ModSpot.Default && modSocketDictionary[commandedModSpot].mod==null){
                    CheckToAttachMod(commandedModSpot, other.GetComponent<Mod>());
                }
            }
        }
    }

    private void CheckToAttachMod(ModSpot commandedModSpot, Mod modToAttach) {
        if (modSocketDictionary[commandedModSpot].mod != modToAttach) {
            Attach(commandedModSpot, modToAttach);
        }
    }

    private void Attach(ModSpot spot, Mod mod, bool isSwapping=false)
    {
        if (modSocketDictionary[spot].mod != null && !isSwapping)
        {
            Detach(spot);            
        }

        if (!isSwapping) {
            ModUIManager.Instance.AttachMod(spot, mod.getModType());
        }
        mod.setModSpot(spot);
        mod.transform.SetParent(modSocketDictionary[spot].socket);
        mod.transform.localPosition = Vector3.zero;
        mod.transform.localRotation = Quaternion.identity;
        modSocketDictionary[spot].mod = mod;        
        mod.AttachAffect(ref playerStats, ref playerMovement);
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

    private void Detach(ModSpot spot, bool isSwapping=false)
    {
        if (modSocketDictionary[spot].mod != null)
        {
            if (!isSwapping) {
                ModUIManager.Instance.DetachMod(spot);
            }
            modSocketDictionary[spot].mod.transform.SetParent(null);
            modSocketDictionary[spot].mod.DetachAffect();
            modSocketDictionary[spot].mod = null;
        }
    }

    private void SwapMods(ModSpot sourceSpot, ModSpot targetSpot)
    {
        Mod tempSourceMod = null;
        Mod tempTargetMod = null;
        if (modSocketDictionary[sourceSpot].mod != null)
        {
            tempSourceMod = modSocketDictionary[sourceSpot].mod;
            Detach(sourceSpot, true);
        }
        if (modSocketDictionary[targetSpot].mod != null)
        {
            tempTargetMod = modSocketDictionary[targetSpot].mod;
            Detach(targetSpot, true);
        }
        if (tempSourceMod != null)
        {
            Attach(targetSpot, tempSourceMod, true);            
        }
        if (tempTargetMod != null)
        {
            Attach(sourceSpot, tempTargetMod, true);            
        }
        if (tempSourceMod != null || tempTargetMod!=null) {
            ModUIManager.Instance.SwapMods(sourceSpot, targetSpot);
        }
        SetAllModUIToIdle();        
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
