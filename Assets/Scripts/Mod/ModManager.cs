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
    [SerializeField]
    private MoveState playerMovement;
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
    public void SendModDictionaryToAnalytics()
    {
        if (modSocketDictionary[ModSpot.ArmL].mod != null)
        {
            AnalyticsManager.Instance.modDictionary["armL"] = modSocketDictionary[ModSpot.ArmL].mod.getModType().ToString();
        }
        else
        {
            AnalyticsManager.Instance.modDictionary["armL"] = "null";
        }

        if (modSocketDictionary[ModSpot.ArmR].mod != null)
        {
            AnalyticsManager.Instance.modDictionary["armR"] = modSocketDictionary[ModSpot.ArmR].mod.getModType().ToString();
        }
        else
        {
            AnalyticsManager.Instance.modDictionary["armR"] = "null";
        }

        if (modSocketDictionary[ModSpot.Legs].mod != null)
        {
            AnalyticsManager.Instance.modDictionary["legs"] = modSocketDictionary[ModSpot.Legs].mod.getModType().ToString();
        }
        else
        {
            AnalyticsManager.Instance.modDictionary["legs"] = "null";
        }
    }
    #endregion

    #region Private Methods
    private void CheckToPickUpMod() {
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.DROP_MODE, ButtonMode.HELD)){
            ModSpot commandedModSpot = GetCommandedModSpot(ButtonMode.DOWN);
            if (commandedModSpot != ModSpot.Default && modSocketDictionary[commandedModSpot].mod == null){
                List<Collider> cols = Physics.OverlapSphere(transform.position, 2.25f).ToList();
                bool foundMod = false;
                cols.ForEach(other => {
                    if (!foundMod && other.tag == Strings.Tags.MOD){                
                        Mod modToAttach = other.GetComponent<Mod>();
                        if (modSocketDictionary[commandedModSpot].mod != modToAttach){
                            Attach(commandedModSpot, modToAttach);
                            foundMod = true;
                        }
                    }
                });
            }
        }
    }
    private void CheckToChargeAndFireMods(){
        if (!InputManager.Instance.QueryAction(Strings.Input.Actions.DROP_MODE, ButtonMode.HELD) &&
            !InputManager.Instance.QueryAction(Strings.Input.Actions.SWAP_MODE, ButtonMode.HELD)){

            List<ModSpot> chargeSpots = GetCommandedModSpots(ButtonMode.HELD);
            if (!chargeSpots.Contains(ModSpot.Default)) {
                chargeSpots.ForEach(spot => {
                    if (modSocketDictionary[spot].mod != null){
                        modSocketDictionary[spot].mod.UpdateChargeTime(Time.deltaTime);
                    }
                });
            }

            CheckToFireMod();

            allModSpots.ForEach(spot=>{
                if (!chargeSpots.Contains(spot) && modSocketDictionary[spot].mod!=null) {
                    if (modSocketDictionary[spot].mod.modEnergySettings.isCharging) {
                        modSocketDictionary[spot].mod.ResetChargeTime();
                    }
                }
            });
        }
    }

    private void CheckToFireMod() {
        ModSpot fireSpot = GetCommandedModSpot(ButtonMode.UP);        
        if (fireSpot != ModSpot.Default){
            Mod mod = modSocketDictionary[fireSpot].mod;
            if (mod != null) {
                stateManager.Attack(mod);
            }
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
                Detach(spotSelected);
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
        SendModDictionaryToAnalytics();
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
            }
            modSocketDictionary[spot].mod.transform.SetParent(null);
            modSocketDictionary[spot].mod.DetachAffect();
            modSocketDictionary[spot].mod = null;
        }
        SendModDictionaryToAnalytics();
    }

    private void SwapMods(ModSpot sourceSpot, ModSpot targetSpot){
        Mod tempSourceMod = null;
        Mod tempTargetMod = null;
        if (modSocketDictionary[sourceSpot].mod != null){
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
        SendModDictionaryToAnalytics();
    }
    #endregion

    #region Private Structures
    private class ModSocket{
        public Transform socket;
        public Mod mod;
        public ModSocket(Transform i_socket){
            socket = i_socket;
        }
    }

    private class CommandedMod{
        public ModSpot modSpot = ModSpot.Default;
        public bool isHeld = false;
        public bool wasHeld = false;
        public float holdTime = 0.0f; 
    } 
    #endregion

}
