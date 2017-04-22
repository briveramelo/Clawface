//BRANDON RIVERA-MELO

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using MovementEffects;
using System.Linq;

public class ModUISelector : MonoBehaviour {

    [SerializeField] private GameObject modEquipCanvas;
    [SerializeField] private GameObject blasterIcon, boomerangeIcon, diceIcon, segwayIcon, geyserIcon, grapplerIcon, stunbatonIcon;
    [SerializeField] private ModInventory modInventory;
    [SerializeField] private ModManager modManager;
    [SerializeField] private float minJoystickSelectionThreshold;

    private Dictionary<ModSpot, string> equipCommands = new Dictionary<ModSpot, string>() {
        {ModSpot.ArmR, Strings.Input.Actions.EQUIP_ARM_RIGHT },
        {ModSpot.Legs, Strings.Input.Actions.EQUIP_LEGS },
        {ModSpot.ArmL, Strings.Input.Actions.EQUIP_ARM_LEFT },
    };    
    private List<ModUIElement> modUIElements;    
    private List<ModUIElement> notSelectedList=new List<ModUIElement>();

    private void Awake() {
        modUIElements=new List<ModUIElement>() {
            new ModUIElement(ModType.ArmBlaster, blasterIcon),
            new ModUIElement(ModType.Boomerang, boomerangeIcon),
            new ModUIElement(ModType.Dice, diceIcon),
            new ModUIElement(ModType.ForceSegway, segwayIcon),
            new ModUIElement(ModType.Geyser, geyserIcon),
            new ModUIElement(ModType.Grappler, grapplerIcon),
            new ModUIElement(ModType.StunBaton, stunbatonIcon),            
        };
    }

    // Update is called once per frame
    void Update () {
        if (Time.timeScale!=0) {
            CheckToEquipMod();
            if (Input.GetKeyDown(KeyCode.Keypad1)) {
                UpdateUI();
            }
        }
	}    

    private void CheckToEquipMod() {
        foreach (KeyValuePair<ModSpot, string> spotCommands in equipCommands) {
            if (InputManager.Instance.QueryAction(spotCommands.Value, ButtonMode.DOWN)) {
                OnDown();
                break;
            }
            if (InputManager.Instance.QueryAction(spotCommands.Value, ButtonMode.HELD)) {
                OnHeld();
                break;
            }
            if (InputManager.Instance.QueryAction(spotCommands.Value, ButtonMode.UP)) {
                OnUp(spotCommands.Key);
                break;
            }
        }        
    }

    private void OnDown() {
        modUIElements.ForEach(modUIElement => {
            modUIElement.Close();
        });
        HitstopManager.Instance.LerpToTimeScale(0.1f, 0.05f);
        modEquipCanvas.SetActive(true);
    }

    private void OnHeld() {
        Vector2 selectAxis = InputManager.Instance.QueryAxes(Strings.Input.Axes.LOOK);
        if (selectAxis.magnitude > minJoystickSelectionThreshold) {
            SelectMod(selectAxis.As360Angle());            
            DeselectMods(ref notSelectedList);
        }
        else {
            if (notSelectedList.Count > 0) {
                DeselectMods(ref notSelectedList);            
            }      
        }
    }

    private void OnUp(ModSpot spot) {
        if (modUIElements.Exists(elm=> elm.isSelected)) {
            modManager.EquipMod(spot, modUIElements.Find(elm => elm.isSelected).modType);
        }

        modUIElements.ForEach(modUIElement=> {
            modUIElement.Close();
        });
        HitstopManager.Instance.LerpToTimeScale(1f, 0.15f);
        modEquipCanvas.SetActive(false);
    }

    private void SelectMod(float selectionAngle) {        
        ModUIElement selectedUIElement = null;
        foreach (ModUIElement modElm in modUIElements) {
            if (modElm.uiElement.activeSelf) {
                modElm.SetIsSelected(selectionAngle);
                if (modElm.isSelected) {
                    selectedUIElement = modElm;                    
                    break;
                }
            }
        }           
        if(selectedUIElement.canBulge) {
            selectedUIElement.InitializeBulge();
            notSelectedList.Clear();
            notSelectedList.Add(selectedUIElement);
            notSelectedList = modUIElements.Except(notSelectedList).ToList();
        }
    }

    private void DeselectMods(ref List<ModUIElement> modUIElementsToDeselect) {
        modUIElementsToDeselect.ForEach(modElm=> {
            if (!modElm.isShrinking && !modElm.isAtStartScale) {
                modElm.InitializeShrink();
            }
        });
    }

    public void UpdateUI() {        
        List<ModType> availableModTypes = modInventory.GetAvailableModTypes();
        int modTypeCount = availableModTypes.Count;
        int i=0;
        availableModTypes.ForEach(modType=> {
            ModUIElement modAngle = modUIElements.Find(ma=>ma.modType==modType);
            modAngle.Set(modType,modTypeCount,i);
            i++;
        });
    }

    #region Private Structures
    private class ModUIElement {
        public GameObject uiElement;
        public ModType modType;
        public Range360 range;
        public float myAngle;
        public Vector2 myPosition;
        public bool isBulging;
        private bool wasSelected;
        public bool canBulge { get { return !isBulging && isSelected && !wasSelected; } }
        public bool canShrink { get { return !isShrinking && !isSelected && wasSelected;} }
        public bool isShrinking;
        public bool isRescaling { get {return isBulging || isShrinking; } }
        public bool isSelected;
        public bool isAtStartScale { get { return uiElement.transform.localScale.IsAboutEqual(startScale); } }

        private const float modUIRadius = 200f;
        private string coroutineString;

        public ModUIElement(ModType modType, GameObject uiElement) {
            this.modType=modType;
            this.uiElement=uiElement;
            Timing.RunCoroutine(InternalUpdate(), Segment.LateUpdate);
        }

        public void Set(ModType modType, int numMods, int myIndex) {
            this.modType = modType;
            float window = (360/numMods);
            range.Min = window*(myIndex - 0.5f);
            range.Max = window*(myIndex + 0.5f);
            myAngle = range.Middle;
            myPosition = myAngle.AsVector2();
            uiElement.SetActive(true);            
            uiElement.transform.localPosition = myPosition * modUIRadius;
            coroutineString = modType + "modUI";
        }

        public void SetIsSelected(float angle) {
            isSelected = range.IsInRange(angle);         
        }

        public void Close() {
            isSelected=false;
            Timing.KillCoroutines(coroutineString);
            uiElement.transform.localScale = startScale;
        }

        private IEnumerator<float> InternalUpdate() {
            while(true) {
                yield return 0f;
                wasSelected=isSelected;
            }
        }

        private Vector3 maxScale=Vector3.one * 3f;
        private Vector3 endScale=Vector3.one * 2.5f;
        private Vector3 startScale = Vector3.one;

        public void InitializeBulge() {
            Timing.KillCoroutines(coroutineString);
            Timing.RunCoroutine(Bulge());
        }
        private IEnumerator<float> Bulge() {
            isBulging=true;
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(LerpToScale(maxScale, 0.3f), coroutineString));
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(LerpToScale(endScale, 0.2f), coroutineString));
            isBulging =false;            
        }

        public void InitializeShrink() {
            isSelected = false;
            Timing.KillCoroutines(coroutineString);
            Timing.RunCoroutine(LerpToScale(startScale, 0.2f));
        }
        private IEnumerable<float> Shrink() {
            isShrinking=true;
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(LerpToScale(startScale, 0.1f)));
            isShrinking=false;
        }

        private IEnumerator<float> LerpToScale(Vector3 targetScale, float lerpFraction) {            
            float distance = 10f;
            while (Vector3.Distance(uiElement.transform.localScale, targetScale)>.03f) {
                distance = Vector3.Distance(uiElement.transform.localScale, targetScale);
                uiElement.transform.localScale = Vector3.Lerp(uiElement.transform.localScale, targetScale, lerpFraction);
                yield return 0f;
            }
            uiElement.transform.localScale = targetScale;
        }
    }

    private struct Range360 {
        private float min;
        private float max;

        public float Middle {
            get {
                float regularDifference = (min + (max-min)/2);
                if (min<max) {
                    return regularDifference;
                }
                return regularDifference - 180f;
            }
        }
        public bool IsInRange(float angle) {
            if (min<max) {
                return angle<=max && angle>=min;
            }
            return angle<=max || angle>=min;
        }


        public float Min { get { return min;}
            set {                
                min = BindInRange360(value);
            }
        }
        public float Max{ get { return max;}
            set {                
                max = BindInRange360(value);
            }
        }

        private float BindInRange360(float value) {
            if (value<0) {
                value=360+value;
            }
            else if (value>360) {
                value=value-360;
            }
            return value;
        }
    }
    #endregion
}
