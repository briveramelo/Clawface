/* 
 * Author Brandon Rivera-Melo
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModManager : MonoBehaviour
{

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private Transform headSocket, leftArmSocket, rightArmSocket, legsSocket;
    [SerializeField]
    private Stats playerStats;
    [SerializeField]
    private MoveState playerMovement;
    [SerializeField] private VelocityBody velBody;
    [SerializeField]
    private PlayerStateManager stateManager;
    [SerializeField]
    private float holdThresholdTime;
    #endregion

    #region Private Fields
    private Dictionary<ModSpot, ModSocket> modSocketDictionary;
    private ModSpot modToSwap;
    private bool isOkToDropMod = true;
    private bool isOkToSwapMods = true;
    private bool isButtonHeld = false;
    private float holdTime;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    private void Start()
    {
        modSocketDictionary = new Dictionary<ModSpot, ModSocket>();
        modSocketDictionary.Add(ModSpot.Legs, new ModSocket(legsSocket));
        modSocketDictionary.Add(ModSpot.ArmL, new ModSocket(leftArmSocket));
        modSocketDictionary.Add(ModSpot.ArmR, new ModSocket(rightArmSocket));
        modToSwap = ModSpot.Default;
        holdTime = 0.0f;
    }

    private void Update()
    {
        if (isOkToDropMod)
        {
            CheckToDropMod();
        }
        SetModToSwap();
        if (isOkToSwapMods)
        {
            CheckToSwapMods();
        }
        CheckToActivateMod();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == Strings.Tags.MOD)
        {
            if (InputManager.Instance.QueryAction(Strings.Input.Actions.DROP_MODE,
                    ButtonMode.HELD))
            {                
                ModSpot commandedModSpot = GetCommandedModSpot();

                if (commandedModSpot != ModSpot.Default && modSocketDictionary[commandedModSpot].mod == null)
                {
                    CheckToAttachMod(commandedModSpot, other.GetComponent<Mod>());
                }
            }
        }
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private void CheckToActivateMod()
    {
        if (!InputManager.Instance.QueryAction(Strings.Input.Actions.DROP_MODE,
                ButtonMode.HELD) &&
            !InputManager.Instance.QueryAction(Strings.Input.Actions.SWAP_MODE,
                ButtonMode.HELD))
        {
            CommandedMod commandedMod = GetCommandedMod();
            ModSpot spot = commandedMod.modSpot;
            if (spot != ModSpot.Default)
            {
                if (modSocketDictionary[spot].mod != null)
                {
                    if (!commandedMod.isHeld && !commandedMod.wasHeld)
                    {
                        stateManager.PrimaryAttack(modSocketDictionary[spot].mod);
                    }else
                    {
                        stateManager.SecondaryAttack(modSocketDictionary[spot].mod, commandedMod.isHeld);
                    }
                }
            }
        }
    }

    private ModSpot GetCommandedModSpot()
    {
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_LEGS, ButtonMode.DOWN))
        {
            return ModSpot.Legs;
        }
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_LEFT, ButtonMode.DOWN))
        {
            return ModSpot.ArmL;
        }
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_RIGHT, ButtonMode.DOWN))
        {
            return ModSpot.ArmR;
        }
        return ModSpot.Default;
    }

    private CommandedMod GetCommandedMod()
    {
        CommandedMod commandedMod = new CommandedMod();
        if(InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_LEGS, ButtonMode.HELD))
        {            
            holdTime += Time.deltaTime;
            if (holdTime > holdThresholdTime)
            {
                commandedMod.modSpot = ModSpot.Legs;
                commandedMod.isHeld = true;
                isButtonHeld = true;
            }
        }
        else if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_LEFT, ButtonMode.HELD))
        {            
            holdTime += Time.deltaTime;
            if (holdTime > holdThresholdTime)
            {
                commandedMod.modSpot = ModSpot.ArmL;
                commandedMod.isHeld = true;
                isButtonHeld = true;
            }
        }
        else if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_RIGHT, ButtonMode.HELD))
        {            
            holdTime += Time.deltaTime;
            if (holdTime > holdThresholdTime)
            {
                commandedMod.modSpot = ModSpot.ArmR;
                commandedMod.isHeld = true;
                isButtonHeld = true;
            }
        }
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_LEGS,
            ButtonMode.UP))
        {
            holdTime = 0.0f;
            commandedMod.modSpot = ModSpot.Legs;
            if (isButtonHeld)
            {
                isButtonHeld = false;
                commandedMod.wasHeld = true;
            }
        }
        else if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_LEFT,
            ButtonMode.UP))
        {
            holdTime = 0.0f;
            commandedMod.modSpot = ModSpot.ArmL;
            if (isButtonHeld)
            {
                isButtonHeld = false;
                commandedMod.wasHeld = true;
            }
        }
        else if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_RIGHT,
            ButtonMode.UP))
        {
            holdTime = 0.0f;
            commandedMod.modSpot = ModSpot.ArmR;
            if (isButtonHeld)
            {
                isButtonHeld = false;
                commandedMod.wasHeld = true;
            }
        }
        return commandedMod;        
    }

    private void CheckToDropMod()
    {
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.DROP_MODE,
                ButtonMode.HELD))
        {            
            ModSpot spotSelected = GetCommandedModSpot();
            if (spotSelected != ModSpot.Default && modSocketDictionary[spotSelected].mod != null)
            {
                Detach(spotSelected);
            }
        }
    }

    private void SetModToSwap()
    {
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.SWAP_MODE,
                ButtonMode.HELD) && modToSwap == ModSpot.Default)
        {
            StartCoroutine(DelayIsOkToSwapMods());            
            modToSwap = GetCommandedModSpot();
            if (modToSwap != ModSpot.Default)
            {
                ModUIManager.Instance.SetUIState(modToSwap, ModUIState.SELECTED);
            }
        }
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.SWAP_MODE,
                ButtonMode.UP))
        {
            SetAllModUIToIdle();
        }
    }

    private void SetAllModUIToIdle()
    {
        foreach (ModSpot modSpot in Enum.GetValues(typeof(ModSpot)))
        {
            if (modSpot != ModSpot.Default)
            {
                ModUIManager.Instance.SetUIState(modSpot, ModUIState.IDLE);
            }
        }
        modToSwap = ModSpot.Default;
    }

    private void CheckToSwapMods()
    {
        if (modToSwap != ModSpot.Default)
        {
            ModSpot secondMod = GetCommandedModSpot();
            if (secondMod != ModSpot.Default && secondMod != modToSwap)
            {
                SwapMods(modToSwap, secondMod);
            }
        }
    }


    private void CheckToAttachMod(ModSpot commandedModSpot, Mod modToAttach)
    {
        if (modSocketDictionary[commandedModSpot].mod != modToAttach)
        {
            Attach(commandedModSpot, modToAttach);
        }
    }

    private void Attach(ModSpot spot, Mod mod, bool isSwapping = false)
    {
        if (modSocketDictionary[spot].mod != null && !isSwapping)
        {
            Detach(spot);
        }

        if (!isSwapping)
        {
            ModUIManager.Instance.AttachMod(spot, mod.getModType());
        }
        mod.setModSpot(spot);
        mod.transform.SetParent(modSocketDictionary[spot].socket);
        mod.transform.localPosition = Vector3.zero;
        mod.transform.localRotation = Quaternion.identity;
        modSocketDictionary[spot].mod = mod;        
        mod.AttachAffect(ref playerStats, velBody);
        StartCoroutine(DelayIsOkToDropMod());
    }

    private IEnumerator DelayIsOkToDropMod()
    {
        isOkToDropMod = false;
        yield return new WaitForEndOfFrame();
        isOkToDropMod = true;
    }
    private IEnumerator DelayIsOkToSwapMods()
    {
        isOkToSwapMods = false;
        yield return new WaitForEndOfFrame();
        isOkToSwapMods = true;
    }

    private void Detach(ModSpot spot, bool isSwapping = false)
    {
        if (modSocketDictionary[spot].mod != null)
        {
            if (!isSwapping)
            {
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
        if (tempSourceMod != null || tempTargetMod != null)
        {
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
    #endregion

    #region Private Structures
    private class ModSocket
    {
        public Transform socket;
        public Mod mod;
        public ModSocket(Transform i_socket)
        {
            socket = i_socket;
        }
    }

    private class CommandedMod
    {
        public ModSpot modSpot = ModSpot.Default;
        public bool isHeld = false;
        public bool wasHeld = false; 
    } 
    #endregion

}
