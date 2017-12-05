//BRANDON RIVERA-MELO

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using MovementEffects;
using System.Linq;

public class ModUISelector : MonoBehaviour {

    [SerializeField] private GameObject modEquipCanvas;//, selectionHighlighter;
    [SerializeField] private GameObject blasterIcon, boomerangeIcon, missileIcon, segwayIcon, geyserIcon, grapplerIcon, stunbatonIcon;
    [SerializeField] private ModInventory modInventory;
    [SerializeField] private ModUIManager modUIManager;
    [SerializeField] private ModManager modManager;
    [SerializeField] private float minJoystickSelectionThreshold;

    public static Dictionary<ModSpot, List<string>> equipCommands = new Dictionary<ModSpot, List<string>>() {
        /* REWIRE INPUTS!!!
        {ModSpot.ArmR, new List<string>() {Strings.Input.Actions.EQUIP_ARM_RIGHT,Strings.Input.Actions.ACTION_ARM_RIGHT } },
        //{ModSpot.Legs, new List<string>() {Strings.Input.Actions.ACTION_LEGS } },
        {ModSpot.ArmL, new List<string>() {Strings.Input.Actions.EQUIP_ARM_LEFT,Strings.Input.Actions.ACTION_ARM_LEFT} },        
        */
    };    
    private List<ModUIElement> notSelectedList=new List<ModUIElement>();
    private List<ModUIElement> modUIElements;    

    private void Awake() {
        modUIElements=new List<ModUIElement>() {
            new ModUIElement(ModType.Blaster, blasterIcon),
            new ModUIElement(ModType.Boomerang, boomerangeIcon),
            new ModUIElement(ModType.Missile, missileIcon),
            new ModUIElement(ModType.SpreadGun, segwayIcon),
            new ModUIElement(ModType.Geyser, geyserIcon),
            new ModUIElement(ModType.LightningGun, grapplerIcon),
        };
    }

    // Update is called once per frame
    void Update () {
        CheckToActivateUI();
        CheckToEquipMod();        
	}

    private void CheckToActivateUI() {
        throw new System.Exception("REWIRE INPUT!");
        /*
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTIVATE_UI, ButtonMode.DOWN)) {            
            if (modUIElements.Exists(elm=>elm.isSet)) {
                if (!modEquipCanvas.activeSelf) {                
                    OpenCanvas();
                }
                else {
                    CloseCanvas();
                }
            }
        } 
        */       
    }


    private void CheckToEquipMod() {
        if (modEquipCanvas.activeSelf) {
            CheckToSelectModUI();
            foreach (KeyValuePair<ModSpot, List<string>> spotCommands in equipCommands) {
                foreach (string command in spotCommands.Value) {
                    if (InputManager.Instance.QueryAction(command, ButtonMode.DOWN)) {
                        if (modManager.GetModSpotDictionary()[spotCommands.Key].mod == null ||
                            (modManager.GetModSpotDictionary()[spotCommands.Key].mod != null &&
                            !modManager.GetModSpotDictionary()[spotCommands.Key].mod.modEnergySettings.isActive)) {

                            modManager.EquipMod(spotCommands.Key, selectedMod);
                            Timing.RunCoroutine(DelayCanActivate(command));
                            CloseCanvas();
                        }
                        else {
                            ModUIElement modElm = modUIElements.Find(elm => elm.isSelected);
                            if (modElm != null) {
                                modElm.SetBadSelection();
                                //SFXManager.Instance.Play(SFXType.BAD_UI_SOUND);
                            }
                        }
                        return;                        
                    }
                }                
            }
        }                     
    }    

    private IEnumerator<float> DelayCanActivate(string command) {        
        while (!InputManager.Instance.QueryAction(command, ButtonMode.UP)) {
            yield return 0f;
        }
        yield return 0f;        
        modManager.SetCanActivate();
    }

    private void IndicateBadSelection() {
        //SFXManager.Instance.Play(SFXType.BAD_UI_SOUND);
    }


    private void OpenCanvas() {
        HitstopManager.Instance.LerpToTimeScale(0.1f, 0.05f);
        modEquipCanvas.SetActive(true);        
        modUIElements.ForEach(modUIElement => {
            modUIElement.Close();
        });

        ModUIElement firstElement = modUIElements.Find(elm => elm.myIndex == 0);
        if (firstElement!=null) {
            firstElement.SetSelected();        
            selectedMod = firstElement.modType;
        }

        allModSpots.ForEach(spot => {
            modUIManager.SetUIState(spot, ModUIState.IDLE);
        });
    }

    List<ModSpot> allModSpots = new List<ModSpot>() { ModSpot.ArmL, ModSpot.ArmR
    //, ModSpot.Legs
    };
    ModType selectedMod;
    private void CheckToSelectModUI() {
        Vector2 moveAxis = InputManager.Instance.QueryAxes(Strings.Input.Axes.MOVEMENT);
        Vector2 lookAxis = InputManager.Instance.QueryAxes(Strings.Input.Axes.LOOK);

        Vector2 selectAxis = lookAxis;
        if (selectAxis.magnitude > minJoystickSelectionThreshold) {
            CheckToSelectMod(selectAxis.As360Angle());            
            DeselectMods(ref notSelectedList);
        }
        else {
            if (notSelectedList.Count > 0 && !justStarting) {
                justStarting = true;
                DeselectMods(ref notSelectedList);            
            }      
        }
    }
    bool justStarting;

    private void CloseCanvas() {        
        modUIElements.ForEach(modUIElement => {
            modUIElement.Close();
        });

        HitstopManager.Instance.LerpToTimeScale(1f, 0.15f);
        allModSpots.ForEach(spot => {
            modUIManager.SetUIState(spot, ModUIState.IDLE);
        });
        modEquipCanvas.SetActive(false);
    }

    private void CheckToSelectMod(float selectionAngle) {        
        foreach (ModUIElement modElm in modUIElements) {
            if (modElm.uiElement.activeSelf) {
                modElm.SetIsSelected(selectionAngle);
                if (modElm.isSelected) {
                    if (modElm.canBulge) {
                        SelectMod(modElm);
                    }
                    break;
                }
            }
        }           
    }

    private void SelectMod(ModUIElement modElm) {
        selectedMod = modElm.modType;
        
        //selectionHighlighter.transform.localPosition = modElm.selectHighlighterPosition;
        //selectionHighlighter.transform.localRotation = modElm.selectHighlighterRotation;
        modElm.SetSelected();
        notSelectedList.Clear();
        notSelectedList.Add(modElm);
        notSelectedList = modUIElements.Except(notSelectedList).ToList();
        if (selectedMod!= modUIElements[0].modType) {
            justStarting = false;
        }
    }
    

    private void DeselectMods(ref List<ModUIElement> modUIElementsToDeselect) {
        modUIElementsToDeselect.ForEach(modElm=> {
            if (!modElm.isShrinking && !modElm.isAtStartScale) {
                modElm.SetUnSelected();
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
        public Range360 range;
        public ModType modType = ModType.None;
        public float myAngle;
        public int myIndex=-1;
        public bool isBulging;

        public Quaternion selectHighlighterRotation { get { return Quaternion.AngleAxis(myAngle, Vector3.forward); } }
        public Vector2 myPosition { get { return myAngle.AsVector2() * modUIRadius; } }
        public Vector3 selectHighlighterPosition { get { return myAngle.AsVector2() * selectionRadius; } }
        public bool isSet { get { return uiElement.activeSelf; } }
        public bool canBulge { get { return !isBulging && isSelected && !wasSelected; } }
        public bool canShrink { get { return !isShrinking && !isSelected && wasSelected;} }
        public bool isShrinking;
        public bool isRescaling { get {return isBulging || isShrinking; } }
        public bool isSelected;
        public bool isAtStartScale { get { return uiElement.transform.localScale.IsAboutEqual(startScale); } }


        private Vector3 maxScale = Vector3.one * 2f;
        private Vector3 endScale = Vector3.one * 1.75f;
        private Vector3 startScale = Vector3.one;
        private string coroutineString;
        private bool wasSelected;

        private const float modUIRadius = 177f;
        private const float selectionRadius = 169f;

        public ModUIElement(ModType modType, GameObject uiElement) {
            this.modType=modType;
            this.uiElement=uiElement;
            Timing.RunCoroutine(InternalUpdate(), Segment.LateUpdate);
        }

        public void Set(ModType modType, int numMods, int myIndex) {
            this.modType = modType;
            this.myIndex = myIndex;
            float window = (360/numMods);
            range.Min = window*(myIndex - 0.5f);
            range.Max = window*(myIndex + 0.5f);
            myAngle = range.Middle;            
            uiElement.SetActive(true);
            uiElement.transform.localPosition = myPosition;
            coroutineString = modType + "modUI";
        }

        public void SetIsSelected(float angle) {
            isSelected = range.IsInRange(angle);         
        }       

        public void SetBadSelection() {
            uiElement.transform.localPosition = myPosition;
            Timing.KillCoroutines(coroutineString);
            Timing.RunCoroutine(Shake());
        }        
                
        public void SetSelected() {
            isSelected = true;
            uiElement.transform.localPosition = myPosition;
            Timing.KillCoroutines(coroutineString);
            Timing.RunCoroutine(Bulge());
        }
        
        public void SetUnSelected() {
            uiElement.transform.localPosition = myPosition;
            isSelected = false;
            Timing.KillCoroutines(coroutineString);
            Timing.RunCoroutine(LerpToScale(startScale, 0.2f));
        }
        public void Close() {
            uiElement.transform.localPosition = myPosition;
            isSelected = false;
            Timing.KillCoroutines(coroutineString);
            uiElement.transform.localScale = startScale;
        }

        private IEnumerable<float> ScaleToStart() {
            isShrinking=true;
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(LerpToScale(startScale, 0.1f)));
            isShrinking=false;
        }
        private IEnumerator<float> Bulge() {
            isBulging = true;
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(LerpToScale(maxScale, 0.3f), coroutineString));
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(LerpToScale(endScale, 0.2f), coroutineString));
            isBulging = false;
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
        private IEnumerator<float> Shake() {
            Vector3 startPosition = myPosition;
            float shakeRadius = 8f;
            float twitchTime = .3f;
            while (twitchTime > 0) {
                uiElement.transform.localPosition = startPosition + Random.onUnitSphere * shakeRadius;
                twitchTime -= Time.fixedUnscaledDeltaTime;
                yield return Timing.WaitForOneFrame;
            }
            uiElement.transform.localPosition = myPosition;
        }
        private IEnumerator<float> InternalUpdate() {
            while (true) {
                yield return 0f;
                wasSelected = isSelected;
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
