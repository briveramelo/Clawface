/* 
 * Author Brandon Rivera-Melo
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModManager : MonoBehaviour
{

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private Transform headSocket, leftArmSocket, rightArmSocket, legsSocket;
    [SerializeField]
    private Stats playerStats;
    [SerializeField] private VelocityBody velBody;
    [SerializeField]
    private PlayerStateManager stateManager;
    #endregion

    #region Private Fields
    private Dictionary<ModSpot, ModSocket> modSocketDictionary;
    private List<ModSpot> allModSpots;
    private ModSpot modToSwap;
    private bool isOkToDropMod = true;
    private bool isOkToSwapMods = true;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        modSocketDictionary = new Dictionary<ModSpot, ModSocket>(){
            {ModSpot.Legs, new ModSocket(legsSocket) },
            {ModSpot.ArmL, new ModSocket(leftArmSocket) },
            {ModSpot.ArmR, new ModSocket(rightArmSocket) },
        };
        allModSpots = new List<ModSpot>() {
            ModSpot.ArmL, ModSpot.ArmR, ModSpot.Legs
        };
        modToSwap = ModSpot.Default;

        AnalyticsManager.Instance.SetModManager(this);

    }

    private void Update()
    {
        CheckToPickUpMod();

        if (isOkToDropMod){
            CheckToDropMod();
        }
        SetModToSwap();
        if (isOkToSwapMods){
            CheckToSwapMods();
        }
        CheckToChargeAndFireMods();
    }    

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Strings.Tags.MOD){
            other.GetComponent<Mod>().ActivateModCanvas();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == Strings.Tags.MOD){
            other.GetComponent<Mod>().DeactivateModCanvas();
        }
    }
    #endregion

    #region Public Methods
    public Dictionary<ModSpot, ModSocket> GetModSpotDictionary()
    {
        return modSocketDictionary;
    }
    #endregion

    #region Private Methods
    private void CheckToPickUpMod() {
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.DROP_MODE, ButtonMode.HELD)){
            ModSpot commandedModSpot = GetCommandedModSpot(ButtonMode.DOWN);
            if (commandedModSpot != ModSpot.Default && modSocketDictionary[commandedModSpot].mod == null){
                List<Collider> cols = Physics.OverlapSphere(transform.position, 2.25f).ToList();
                Mod modToAttach=null;
                float shortestDistanceAway=10f;
                cols.ForEach(other => {
                    if (other.tag == Strings.Tags.MOD){                        
                        if (!IsHoldingMod(other.transform)) {
                            float distanceAway = Vector3.Distance(transform.position, other.transform.position);
                            if (distanceAway<shortestDistanceAway) {
                                modToAttach = other.GetComponent<Mod>();
                                shortestDistanceAway=distanceAway;
                            }
                        }
                    }
                });
                if(modToAttach!=null) {
                    if (modSocketDictionary[commandedModSpot].mod != modToAttach){
                        Attach(commandedModSpot, modToAttach);
                    }
                }
            }
        }
    }
    private void CheckToChargeAndFireMods(){
        if (!InputManager.Instance.QueryAction(Strings.Input.Actions.DROP_MODE, ButtonMode.HELD) &&
            !InputManager.Instance.QueryAction(Strings.Input.Actions.SWAP_MODE, ButtonMode.HELD)){

            CheckForModInput((ModSpot spot)=> { modSocketDictionary[spot].mod.BeginCharging();}, ButtonMode.DOWN);
            CheckForModInput((ModSpot spot)=> { modSocketDictionary[spot].mod.RunCharging();}, ButtonMode.HELD);
            CheckForModInput((ModSpot spot)=> {
                stateManager.Attack(modSocketDictionary[spot].mod);
                modSocketDictionary[spot].mod.Activate();               
            }, ButtonMode.UP);                    
        }
    }

    private void CheckForModInput(Action<ModSpot> onComplete, ButtonMode mode) {
        List<ModSpot> chargeSpots = GetCommandedModSpots(mode);
        if (!chargeSpots.Contains(ModSpot.Default)) {
            chargeSpots.ForEach(spot => {
                if (modSocketDictionary[spot].mod != null){
                    onComplete(spot);                    
                }
            });
        }
    }

    private ModSpot GetCommandedModSpot(ButtonMode mode){
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_LEGS, mode)){
            return ModSpot.Legs;
        }
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_LEFT, mode)){
            return ModSpot.ArmL;
        }
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_RIGHT, mode)){
            return ModSpot.ArmR;
        }
        return ModSpot.Default;
    }

    private List<ModSpot> GetCommandedModSpots(ButtonMode mode) {
        List<ModSpot> modSpots = new List<ModSpot>();
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_LEGS, mode)){
            modSpots.Add(ModSpot.Legs);
        }
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_LEFT, mode)){
            modSpots.Add(ModSpot.ArmL);
        }
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_RIGHT, mode)){
            modSpots.Add(ModSpot.ArmR);
        }

        if (modSpots.Count==0) {
            modSpots.Add(ModSpot.Default);
        }
        return modSpots;
    }

    private void CheckToDropMod(){
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.DROP_MODE, ButtonMode.HELD)){            
            ModSpot spotSelected = GetCommandedModSpot(ButtonMode.DOWN);
            if (spotSelected != ModSpot.Default && modSocketDictionary[spotSelected].mod != null){
                if (!modSocketDictionary[spotSelected].mod.modEnergySettings.isInUse) {
                    Detach(spotSelected);
                }
            }
        }
    }

    private void SetModToSwap(){
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.SWAP_MODE, ButtonMode.HELD) && 
            modToSwap == ModSpot.Default){

            StartCoroutine(DelayIsOkToSwapMods());            
            modToSwap = GetCommandedModSpot(ButtonMode.DOWN);
            if (modToSwap != ModSpot.Default){
                ModUIManager.Instance.SetUIState(modToSwap, ModUIState.SELECTED);
            }
        }
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.SWAP_MODE, ButtonMode.UP)){
            SetAllModUIToIdle();
        }
    }

    private void SetAllModUIToIdle(){
        foreach (ModSpot modSpot in Enum.GetValues(typeof(ModSpot))){
            if (modSpot != ModSpot.Default){
                ModUIManager.Instance.SetUIState(modSpot, ModUIState.IDLE);
            }
        }
        modToSwap = ModSpot.Default;
    }

    private void CheckToSwapMods(){
        if (modToSwap != ModSpot.Default){
            ModSpot secondMod = GetCommandedModSpot(ButtonMode.DOWN);
            if (secondMod != ModSpot.Default && secondMod != modToSwap){
                Mod mod1= modSocketDictionary[modToSwap].mod;
                if (mod1!=null) {
                    if (mod1.modEnergySettings.isInUse ||mod1.modEnergySettings.isCharging) {
                        return;
                    }
                }
                Mod mod2= modSocketDictionary[secondMod].mod;
                if (mod2!=null) {
                    if (mod2.modEnergySettings.isInUse ||mod2.modEnergySettings.isCharging) {
                        return;
                    }
                }
                SwapMods(modToSwap, secondMod);                
            }
        }
    }

    private void Attach(ModSpot spot, Mod mod, bool isSwapping = false){
       
        if (modSocketDictionary[spot].mod != null && !isSwapping){
            Detach(spot);
        }

        if (!isSwapping){
            ModUIManager.Instance.AttachMod(spot, mod.getModType());
        }
        mod.setModSpot(spot);
        mod.transform.SetParent(modSocketDictionary[spot].socket);
        mod.transform.localPosition = Vector3.zero;
        mod.transform.localRotation = Quaternion.identity;
        modSocketDictionary[spot].mod = mod;        
        mod.AttachAffect(ref playerStats, velBody);
        StartCoroutine(DelayIsOkToDropMod());
        mod.DeactivateModCanvas();
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

    private void Detach(ModSpot spot, bool isSwapping = false){
        if (modSocketDictionary[spot].mod != null){
            if (!isSwapping){
                ModUIManager.Instance.DetachMod(spot);
                AnalyticsManager.Instance.DropMod();
            }
            modSocketDictionary[spot].mod.transform.SetParent(null);
            modSocketDictionary[spot].mod.DetachAffect();
            modSocketDictionary[spot].mod = null;
        }
    }

    private void SwapMods(ModSpot sourceSpot, ModSpot targetSpot){
        Mod tempSourceMod = null;
        Mod tempTargetMod = null;
        AnalyticsManager.Instance.SwapMods();
        if (modSocketDictionary[sourceSpot].mod != null)
        {
            tempSourceMod = modSocketDictionary[sourceSpot].mod;
            Detach(sourceSpot, true);
        }
        if (modSocketDictionary[targetSpot].mod != null){
            tempTargetMod = modSocketDictionary[targetSpot].mod;
            Detach(targetSpot, true);
        }
        if (tempSourceMod != null){
            Attach(targetSpot, tempSourceMod, true);
        }
        if (tempTargetMod != null){
            Attach(sourceSpot, tempTargetMod, true);
        }
        if (tempSourceMod != null || tempTargetMod != null){
            ModUIManager.Instance.SwapMods(sourceSpot, targetSpot);
        }
        SetAllModUIToIdle();
    }
    #endregion

    #region Public Structures
    public class ModSocket
    {
        public Transform socket;
        public Mod mod;
        public ModSocket(Transform i_socket){
            socket = i_socket;
        }
    }
    #endregion

    #region Private Structures


    private class CommandedMod{
        public ModSpot modSpot = ModSpot.Default;
        public bool isHeld = false;
        public bool wasHeld = false;
        public float holdTime = 0.0f; 
    } 

    private bool IsHoldingMod(Transform otherMod) {
        foreach (KeyValuePair<ModSpot, ModSocket> modSocket in modSocketDictionary) {
            if (modSocket.Value.socket==otherMod.parent) {
                return true;
            }
        }
        return false;        
    }
    #endregion

}
