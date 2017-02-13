/* 
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
        public bool isActive;
        public ModSocket(Transform i_socket) {
            socket = i_socket;
        }
    }

    [SerializeField] Transform headSocket, leftArmSocket, rightArmSocket, legsSocket;
    [SerializeField] Stats playerStats;

    [SerializeField]
    private PlayerMovement playerMovement;

    private Dictionary<ModSpot, ModSocket> modSocketDictionary;
    private ModSpot modToSwap = ModSpot.Default;
    private ModSpot lastModToSwap;
    private ModQueueState modQueueState = ModQueueState.Use;

    private float triggerThreshold =0.95f;
    private float lastRightArm;
    private float lastLeftArm;
    private bool isOkToInteract = true;

    // Use this for initialization
    void Start()
    {
        modSocketDictionary = new Dictionary<ModSpot, ModSocket>() {
            {ModSpot.Head, new ModSocket(headSocket) },
            {ModSpot.Legs, new ModSocket(legsSocket) },
            {ModSpot.ArmL, new ModSocket(leftArmSocket) },
            {ModSpot.ArmR, new ModSocket(rightArmSocket) },
        };        
    }

    void Update() {                
        CheckToDropMod();        
        CheckToSwapMods();        
        CheckToActivateMod();                

        lastRightArm = Input.GetAxis(Strings.RIGHTARM);
        lastLeftArm = Input.GetAxis(Strings.LEFTARM);
    }

    private void OnTriggerStay(Collider other) {
        if (other.tag == Strings.MOD) {            
            ModSpot commandedModSpot = GetCommandedModSpotDown();
            if (commandedModSpot != ModSpot.Default && isOkToInteract) {
                if (modQueueState != ModQueueState.Swap) {
                    Mod modToAttach = other.GetComponent<Mod>();

                    if (modQueueState == ModQueueState.Drop && modSocketDictionary[commandedModSpot].mod != null) { //swap with ground by default                        
                        ResetQueueStates();
                        Detach(commandedModSpot);
                        Attach(commandedModSpot, modToAttach);
                        StartCoroutine(DelayInteraction());
                    }
                    else if(modSocketDictionary[commandedModSpot].mod == null) {
                        Attach(commandedModSpot, modToAttach);
                        StartCoroutine(DelayInteraction());
                    }
                }
            }            
        }
    }

    private IEnumerator DelayInteraction() {
        isOkToInteract = false;
        yield return new WaitForEndOfFrame();
        isOkToInteract = true;
    }

    private void CheckToActivateMod()
    {
        if (isOkToInteract) {
            if (modQueueState == ModQueueState.Use) {
                SetCommandedModSpots();
                foreach (KeyValuePair<ModSpot, ModSocket> modSocket in modSocketDictionary) {                                        
                    if (modSocket.Value.isActive) {
                        if (modSocketDictionary[modSocket.Key].mod != null) {
                            playerMovement.PlayAnimation(modSocketDictionary[modSocket.Key].mod);
                        }
                    }                    
                }
            }
        }
    }

    private void CheckToDropMod()
    {
        if (Input.GetButtonDown(Strings.DROP)) {            
            if (GetModsEquippedCount()>0) {
                ResetQueueStates();
                if (modQueueState == ModQueueState.Use || modQueueState == ModQueueState.Swap) {                    
                    modQueueState = ModQueueState.Drop;
                    //TODO include function call to MODUIMANAGER to display you're in a DROP state                        
                }                
            }
        }

        if (modQueueState == ModQueueState.Drop) {
            ModSpot spotSelected = GetCommandedModSpotDown();
            if (spotSelected != ModSpot.Default && modSocketDictionary[spotSelected].mod != null && isOkToInteract) {
                Detach(spotSelected);
                ResetQueueStates();
                StartCoroutine(DelayInteraction());
            }            
        }
    }

    private void CheckToSwapMods() {
        if (Input.GetButtonDown(Strings.SWAP)) {
            if (GetModsEquippedCount() > 0) {
                ResetQueueStates();
                if (modQueueState == ModQueueState.Use || modQueueState == ModQueueState.Drop) {
                    modQueueState = ModQueueState.Swap;
                    //TODO include function call to MODUIMANAGER to display you're in a SWAP state
                }
            }
        }

        if (modQueueState == ModQueueState.Swap) {
            if (modToSwap == ModSpot.Default) { //set first mod to swap
                modToSwap = GetCommandedModSpotDown();
                if (modToSwap != ModSpot.Default) {
                    ModUIManager.Instance.SetUIState(modToSwap, ModUIState.SELECTED);                    
                }                
            }
            else { //set second mod to swap
                ModSpot secondMod = GetCommandedModSpotDown();
                if (secondMod != ModSpot.Default && secondMod != modToSwap) {
                    SwapMods(modToSwap, secondMod);
                    ResetQueueStates();
                    StartCoroutine(DelayInteraction());
                }
            }
        }
    }

    private void ResetQueueStates() {
        SetAllModUIToIdle();
        modToSwap = ModSpot.Default;
        modQueueState = ModQueueState.Use;
    } 

    void SetAllModUIToIdle() {
        foreach (ModSpot modSpot in Enum.GetValues(typeof(ModSpot))) {
            if (modSpot!=ModSpot.Default) {
                ModUIManager.Instance.SetUIState(modSpot, ModUIState.IDLE);
            }
        }
    }
    
    private void Attach(ModSpot spot, Mod mod, bool isSwapping=false)
    {        
        if (!isSwapping) {
            ModUIManager.Instance.AttachMod(spot, mod.getModType());
        }
        mod.setModSpot(spot);
        mod.transform.SetParent(modSocketDictionary[spot].socket);
        mod.transform.localPosition = Vector3.zero;
        mod.transform.localRotation = Quaternion.identity;
        modSocketDictionary[spot].mod = mod;        
        mod.AttachAffect(ref playerStats, ref playerMovement);
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
    }


    private ModSpot GetCommandedModSpotDown() {
        if (Input.GetButtonDown(Strings.HEAD) || Input.GetButton(Strings.LEFTBUMPER)) {
            return ModSpot.Head;
        }
        if (Input.GetButtonDown(Strings.LEGS) || Input.GetButton(Strings.RIGHTBUMPER)) {
            return ModSpot.Legs;
        }
        if (Input.GetAxis(Strings.LEFTARM) > triggerThreshold && lastLeftArm <= triggerThreshold) {
            return ModSpot.ArmL;
        }
        if (Input.GetAxis(Strings.RIGHTARM) > triggerThreshold && lastRightArm <= triggerThreshold) {
            return ModSpot.ArmR;
        }
        return ModSpot.Default;
    }

    
    private void SetCommandedModSpots() {
        modSocketDictionary[ModSpot.Head].isActive = Input.GetButton(Strings.HEAD) || Input.GetButton(Strings.LEFTBUMPER);
        modSocketDictionary[ModSpot.Legs].isActive = Input.GetButton(Strings.LEGS) || Input.GetButton(Strings.RIGHTBUMPER);
        modSocketDictionary[ModSpot.ArmL].isActive = Input.GetAxis(Strings.LEFTARM) > triggerThreshold;
        modSocketDictionary[ModSpot.ArmR].isActive = Input.GetAxis(Strings.RIGHTARM) > triggerThreshold;      
    }

    private int GetModsEquippedCount() {
        int numModsEquipped = 0;
        foreach (KeyValuePair<ModSpot, ModSocket> modSocket in modSocketDictionary) {
            if (modSocket.Value.mod != null) {
                numModsEquipped++;                
            }
        }
        return numModsEquipped;
    }

    private enum ModQueueState {
        Use=0,
        Drop=1,
        Swap=2
    }
}
