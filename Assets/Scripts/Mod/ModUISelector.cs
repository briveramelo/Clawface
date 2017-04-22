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
    [SerializeField] private float minJoystickSelectionThreshold;

    private List<string> equipCommands = new List<string>() {
        Strings.Input.Actions.EQUIP_ARM_RIGHT,
        Strings.Input.Actions.EQUIP_LEGS,
        Strings.Input.Actions.EQUIP_ARM_LEFT,
    };
    private List<string> buttonCommands = new List<string>() {
        Strings.Input.Actions.EQUIP_ARM_RIGHT,
        Strings.Input.Actions.EQUIP_LEGS,
        Strings.Input.Actions.EQUIP_ARM_LEFT,
    };
    private List<ModUIElement> modUIElements;
    private ModType selectedModType;
    private List<ModUIElement> modUIBulgeList=new List<ModUIElement>();

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
        equipCommands.ForEach(command=> {
            if (InputManager.Instance.QueryAction(command, ButtonMode.DOWN)) {
                OnDown();
                return;
            }
            if (InputManager.Instance.QueryAction(command, ButtonMode.HELD)) {
                OnHeld();
                return;
            }
            if (InputManager.Instance.QueryAction(command, ButtonMode.UP)) {
                OnUp();
                return;
            }
        });
    }

    private void OnDown() {
        modEquipCanvas.SetActive(true);
    }

    private void OnHeld() {
        Vector2 selectAxis = InputManager.Instance.QueryAxes(Strings.Input.Axes.LOOK);
        if (selectAxis.magnitude > minJoystickSelectionThreshold) {
            float selectionAngle = selectAxis.As360Angle();
            if (!modUIElements.Any(modElm=>modElm.isSelected)) {
                modUIElements.ForEach(modElm=>modElm.SetIsSelected(selectionAngle));
            }
            ModUIElement selectedUIElement = modUIElements.Find(modElm=>modElm.isSelected);
            selectedModType = selectedUIElement.modType;
            if(selectedUIElement.canBulge) {
                selectedUIElement.InitializeBulge();
                modUIBulgeList.Clear();
                modUIBulgeList.Add(selectedUIElement);
                List<ModUIElement> modElms = modUIElements.Except(modUIBulgeList).ToList();
                modElms.ForEach(modElm=> {
                    if (modElm.canShrink) {
                        Debug.Log(modElm.modType + " can shrink");
                        modElm.InitializeShrink();
                    }
                });
            }                                                   
        }
    }

    private void OnUp() {
        modUIElements.ForEach(modUIElement=> {
            modUIElement.Close();
        });
        modEquipCanvas.SetActive(false);
    }

    private void HandleUIScaling(ref ModUIElement modUIElement, float selectionAngle) {                        
        if (modUIElement.canBulge) {
            modUIElement.InitializeBulge();
        }            
        else if (modUIElement.canShrink){
            modUIElement.InitializeShrink();
        }                    
    }

    public ModType GetSelectedMod() {
        return selectedModType;
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

    private class ModUIElement {
        public GameObject uiElement;
        public ModType modType;
        public Range360 range;
        public float myAngle;
        public Vector2 myPosition;
        public bool isBulging;
        private bool wasSelected;
        public bool canBulge { get { return !isBulging && !wasSelected && isSelected;} }
        public bool canShrink { get { return !isShrinking && !isSelected && wasSelected;} }
        public bool isShrinking;
        public bool isRescaling { get {return isBulging || isShrinking; } }
        public bool isSelected;

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
            Debug.Log(modType + " " + range.Min + " " + range.Max + " " + range.Middle);
            myAngle = range.Middle;
            myPosition = myAngle.AsVector2();
            //Debug.Log(modType + " " + myAngle + " " + myPosition);
            uiElement.SetActive(true);            
            uiElement.transform.localPosition = myPosition * modUIRadius;
            coroutineString = modType + "modUI";
        }

        public void SetIsSelected(float angle) {
            isSelected = range.IsInRange(angle);         
        }

        public void Close() {
            isSelected=false;
            uiElement.transform.localScale = startScale;
        }

        private IEnumerator<float> InternalUpdate() {
            while(true) {
                wasSelected=isSelected;
                yield return 0f;
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
                return angle<max && angle>=min;
            }
            return angle<=max && angle>min;
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
}
