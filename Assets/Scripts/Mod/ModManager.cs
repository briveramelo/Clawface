/* 
 * Author Brandon Rivera-Melo
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModManager : MonoBehaviour
{

    #region Public Statics

    // Probably not the best way to hook this up..
    // but seems to be the best way for how we have
    // the mod manager and player hooked up.
    // Maybe....
    // TODO - add "GameManager" singleton that coordinates important things like mods and level
    public static ModType leftArmOnLoad = ModType.None;
    public static ModType rightArmOnLoad = ModType.None;
    public static bool assignFromPool = true;

    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private Transform leftArmSocket, rightArmSocket;
    [SerializeField]
    private Stats playerStats;
    [SerializeField] private VelocityBody velBody;
    [SerializeField] private ModInventory modInventory;
    [SerializeField] private float modPickupRadius;
    [SerializeField] private ModType[] modPool;
    #endregion

    #region Private Fields
    private Dictionary<ModSpot, ModSocket> modSocketDictionary;
    private List<ModSpot> allModSpots;
    private ModSpot modToSwap;    
    private bool isOkToSwapMods = true;
    List<Mod> overlapMods = new List<Mod>();
    private bool canActivate=true;
    private bool isDead;
    #endregion

    #region Unity Lifecycle

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, modPickupRadius);
    }

    private void Start()
    {
        modSocketDictionary = new Dictionary<ModSpot, ModSocket>(){
            {ModSpot.ArmR, new ModSocket(rightArmSocket) },            
            {ModSpot.ArmL, new ModSocket(leftArmSocket) },
        };
        allModSpots = new List<ModSpot>() {
            ModSpot.ArmL, ModSpot.ArmR
        };
        modToSwap = ModSpot.Default;

        AnalyticsManager.Instance.SetModManager(this);

        modInventory = GetComponent<ModInventory>();
        Debug.Assert(modInventory);

        if (assignFromPool) {
            AttachRandomMods();
        } else {
            AttachMods(leftArmOnLoad, rightArmOnLoad);
        }
        isDead = false;
    }

    private void OnEnable()
    {
        EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_KILLED, PlayerDead);
    }

    private void OnDisable()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_KILLED, PlayerDead);
        }
    }

    private void Update()
    {
        CheckToCollectMod();
        CheckToChargeAndFireMods();
        if (Input.GetKeyDown(KeyCode.M))
        {
            Mod rightMod = rightArmSocket.GetComponentInChildren<Mod>();
            Mod leftMod = leftArmSocket.GetComponentInChildren<Mod>();
            if (rightMod)
            {
                rightMod.gameObject.SetActive(false);
            }
            if (leftMod)
            {
                leftMod.gameObject.SetActive(false);
            }
            modSocketDictionary[ModSpot.ArmL].mod = null;
            modSocketDictionary[ModSpot.ArmR].mod = null;
            AttachRandomMods();
        }
    }
    #endregion

    #region Public Methods
    public Dictionary<ModSpot, ModSocket> GetModSpotDictionary()
    {
        return modSocketDictionary;
    }

    public void EquipMod(ModSpot spot, ModType type) {        
        canActivate = false;
        Mod modToEquip = modInventory.GetMod(type, spot);
        if (modToEquip!=null) {
            Attach(spot, modToEquip);
        }
    }

    public void SetCanActivate() {
        canActivate = true;
    }
    #endregion

    #region Private Methods


    private void AttachRandomMods()
    {
        if (modPool != null && modPool.Length > 0)
        {
            ModType rightHandModType = modPool[UnityEngine.Random.Range(0, modPool.Length)];
            ModType leftHandModType = modPool[UnityEngine.Random.Range(0, modPool.Length)];
            AttachMods(leftHandModType, rightHandModType);
        } else { // fallback to defaults
            AttachMods(leftArmOnLoad, rightArmOnLoad);
        }
    }

    private void AttachMods(ModType left, ModType right)
    {
        GameObject rightHandMod = InstantiateMod(right);
        GameObject leftHandMod = InstantiateMod(left);
        InitializeAndAttachMod(rightHandMod);
        InitializeAndAttachMod(leftHandMod);
    }

    private GameObject InstantiateMod(ModType modType)
    {
        switch (modType)
        {
            case ModType.Blaster:
                return Instantiate(modInventory.blaster);
            case ModType.Boomerang:
                return Instantiate(modInventory.boomerang);
            case ModType.Missile:
                return Instantiate(modInventory.missile);
            case ModType.SpreadGun:
                return Instantiate(modInventory.segway);
            case ModType.Geyser:
                return Instantiate(modInventory.geyser);
            case ModType.LightningGun:
                return Instantiate(modInventory.grappler);
            default:
                return null;
        }
    }

    private void CheckToCollectMod() {
        Physics.OverlapSphere(transform.position, modPickupRadius).ToList().ForEach(other => {
            if (other.tag == Strings.Tags.MOD)
            {
                InitializeAndAttachMod(other.gameObject);
            }
        });
    }

    private void InitializeAndAttachMod(GameObject other)
    {
        if (!IsHoldingMod(other.transform))
        {
            Mod mod = other.GetComponent<Mod>();
            if (mod != null)
            {
                modInventory.CollectMod(mod.getModType());
                foreach (KeyValuePair<ModSpot, ModSocket> modSpotSocket in modSocketDictionary)
                {
                    if (modSpotSocket.Value.mod == null)
                    {
                        mod = modInventory.GetMod(mod.getModType(), modSpotSocket.Key);
                        if (mod != null)
                        {
                            Attach(modSpotSocket.Key, mod);
                        }
                        break;
                    }
                }
                Destroy(other.gameObject);
            }
        }
    }

    private void CheckToChargeAndFireMods(){        
        if (canActivate && !isDead)
        {
            CheckForModInput((ModSpot spot) =>
            {
                modSocketDictionary[spot].mod.Activate();
            }, ButtonMode.HELD);
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

    private void PlayerDead(params object[] parameters)
    {
        isDead = true;
    }
    
    private ModSpot GetCommandedModSpot(ButtonMode mode){        
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.FIRE_LEFT, mode))
        {
            return ModSpot.ArmL;
        }
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.FIRE_RIGHT, mode))
        {
            return ModSpot.ArmR;
        }
        return ModSpot.Default;
    }

    private List<ModSpot> GetCommandedModSpots(ButtonMode mode) {
        List<ModSpot> modSpots = new List<ModSpot>();        
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.FIRE_LEFT, mode))
        {
            modSpots.Add(ModSpot.ArmL);
        }
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.FIRE_RIGHT, mode))
        {
            modSpots.Add(ModSpot.ArmR);
        }

        if (modSpots.Count==0) {
            modSpots.Add(ModSpot.Default);
        }
        return modSpots;        
    }

    private void Attach(ModSpot spot, Mod mod, bool isSwapping = false){
        mod.gameObject.SetActive(true);
        if (modSocketDictionary[spot].mod != null && !isSwapping){
            Detach(spot);
        }

        if (!isSwapping){
            //modUIManager.AttachMod(spot, mod.getModType());
        }
        mod.setModSpot(spot);
        mod.transform.SetParent(modSocketDictionary[spot].socket);
        mod.transform.localPosition = Vector3.zero;
        mod.transform.localRotation = Quaternion.identity;
        mod.transform.localScale = Vector3.one;
        modSocketDictionary[spot].mod = mod;        
        mod.AttachAffect(ref playerStats, velBody);
    }

    private void Detach(ModSpot spot, bool isSwapping = false){
        if (modSocketDictionary[spot].mod != null){
            
            modSocketDictionary[spot].mod.transform.SetParent(modInventory.GetModParent(modSocketDictionary[spot].mod.getModType()));
            modSocketDictionary[spot].mod.gameObject.SetActive(false);
            modSocketDictionary[spot].mod.DetachAffect();
            modSocketDictionary[spot].mod = null;
        }
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
