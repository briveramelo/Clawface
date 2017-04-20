using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using MovementEffects;

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
    private List<ModUIElement> modUIElements;
    private ModType selectedModType;

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
		CheckToEquipMod();
        if (Input.GetKeyDown(KeyCode.Keypad1)) {
            UpdateUI();
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
            modUIElements.ForEach(modUIElement=> {
                HandleUIScaling(ref modUIElement, selectionAngle);
                SetSelectedMod(ref modUIElement, selectionAngle);
            });
        }
    }

    private void OnUp() {
        modEquipCanvas.SetActive(false);
    }

    private void HandleUIScaling(ref ModUIElement modUIElement, float selectionAngle) {        
        string routineString = modUIElement.modType + "modui";
        if (!modUIElement.isBulging && modUIElement.IsSelected(selectionAngle)) {
            Timing.RunCoroutine(modUIElement.Bulge(routineString), routineString);
        }
        else {
            if(modUIElement.isBulging) {
                Timing.KillCoroutines(routineString);
                modUIElement.Rescale();
            }
        }
    }

    private void SetSelectedMod(ref ModUIElement modUIElement, float selectionAngle) {
        if (modUIElement.IsSelected(selectionAngle)) {
            selectedModType = modUIElement.modType;
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
        public Range range;
        public float myAngle;
        public Vector2 myPosition;
        public bool isBulging;

        private const float modUIRadius = 2f;

        public ModUIElement(ModType modType, GameObject uiElement) {
            this.modType=modType;
            this.uiElement=uiElement;
        }

        public void Set(ModType modType, int numMods, int myIndex) {
            this.modType = modType;
            range.min = (360/numMods)*myIndex;
            range.max = (360/numMods)*(myIndex+1);
            myAngle = range.middle;
            myPosition = myAngle.AsVector2();

            
            uiElement.transform.localPosition = myPosition * modUIRadius;
        }

        public bool IsSelected(float angle) {
            if(angle<range.max && angle>range.min) {
                return true;
            }
            return false;
        }

        private Vector3 maxScale=Vector3.one * 2f;
        private Vector3 endScale=Vector3.one * 1.8f;
        private Vector3 startScale = Vector3.one;
        public IEnumerator<float> Bulge(string coroutineString) {
            isBulging=true;
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(LerpToScale(maxScale), coroutineString));
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(LerpToScale(endScale), coroutineString));
            isBulging=false;
        }

        public void Rescale() {
            Timing.RunCoroutine(LerpToScale(startScale));
        }

        private IEnumerator<float> LerpToScale(Vector3 targetScale) {
            while (Vector3.Distance(uiElement.transform.localScale, targetScale)>.02f) {
                uiElement.transform.localScale = Vector3.Lerp(uiElement.transform.localScale, targetScale, 0.1f);
                yield return 0f;
            }
        }
    }

    private struct Range {
        public float min;
        public float max;
        public float middle { get { return (max-min)/2;} }
    }
}
