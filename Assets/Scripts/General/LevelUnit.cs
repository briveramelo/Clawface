using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using MEC;
using ModMan;
using UnityEngine.Assertions;

public enum LevelUnitStates {
    cover,
    floor,
    pit
}
public interface ILevelTilable {
    void SetCurrentState(LevelUnitStates newState);
    void ClearEvents();
    void AddCoverStateEvent(string eventName);
    void AddFloorStateEvent(string eventName);
    void AddPitStateEvent(string eventName);
    bool CheckForEvent(string eventName, LevelUnitStates state);
    void TransitionToCoverState(params object[] inputs);
    void TransitionToFloorState(params object[] inputs);
    void TransitionToPitState(params object[] inputs);
    void HideBlockingObject();
}

public class LevelUnit : RoutineRunner, ILevelTilable {

    #region private variables
    private float meshSizeY;
    private float meshSizeZ;
    private float meshSizeX;
    private bool isTransitioning;
    private float pitYPosition;
    private float coverYPosition;
    private float floorYPosition;
    private LevelUnitStates currentState;
    private LevelUnitStates nextState;
    private GameObject blockingObject;
    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock materialPropertyBlock;
    private Color startColor, targetColor;
    private const string AlbedoTint = "_AlbedoTint";
    private string tintCoroutineName { get { return coroutineName + AlbedoTint; } }
    private List<LevelUnitStates> levelUnitStates = new List<LevelUnitStates>();
    private Splattable splattable;
    #endregion

    #region serialized fields
    [SerializeField] private List<string> coverStateEvents = new List<string>();
    [SerializeField] private List<string> floorStateEvents = new List<string>();
    [SerializeField] private List<string> pitStateEvents = new List<string>();
    [SerializeField] float yMoveSpeed = 0.03f;

    [SerializeField] AbsAnim colorShiftAnim;
    [SerializeField] Color riseColor, flatColor, fallColor;

    
    
    #endregion

    #region public variables
    public LevelUnitStates defaultState = LevelUnitStates.floor;
    #endregion

    #region unity lifecycle
    private void Awake()
    {
        splattable = GetComponent<Splattable>();
        meshRenderer = GetComponent<MeshRenderer>();
        Assert.IsNotNull(splattable);
        if (meshRenderer)
        {
            meshSizeX = meshRenderer.bounds.size.x;
            meshSizeY = meshRenderer.bounds.size.y;
            meshSizeZ = meshRenderer.bounds.size.z;
            materialPropertyBlock = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(materialPropertyBlock);
            Color emptyColor = new Color(0f, 0f, 0f, 0f);
            if (riseColor.IsAboutEqual(emptyColor) ) {
                riseColor = Color.cyan.ChangeAlpha(.3f);
                flatColor = Color.black.ChangeAlpha(.8f);
                fallColor = Color.red.ChangeAlpha(.3f);
            }
        }
        colorShiftAnim.OnUpdate = OnColorChange;
        currentState = defaultState;
        CalculateStatePositions();
    }

    private void OnEnable() {
        RegisterToEvents();
    }

    protected override void OnDisable() {
        base.OnDisable();
        if (EventSystem.Instance) {
            DeRegisterFromEvents();
        }
    }

    void FixedUpdate() {
        if (isTransitioning) {
            if (CanStartTransition()) {
                MoveToNewPosition();
            }
        }
    }
    #endregion

    #region private functions
    private bool CanStartTransition()
    {
        string[] masks = { Strings.Layers.ENEMY, Strings.Layers.ENEMY_BODY, Strings.Layers.MODMAN };
        return !Physics.CheckBox(transform.position, Vector3.one * 0.5f, Quaternion.identity, LayerMask.GetMask(masks));
    }

    

    void RegisterEvents(ref List<string> eventNames, EventSystem.FunctionPrototype func) {
        if (eventNames != null) {
            foreach (string eventName in eventNames) {
                EventSystem.Instance.RegisterEvent(eventName, func);
            }
        }
    }

    void UnregisterEvents(ref List<string> eventNames, EventSystem.FunctionPrototype func) {
        if (eventNames != null) {
            foreach (string eventName in eventNames) {
                EventSystem.Instance.UnRegisterEvent(eventName, func);
            }
            eventNames.Clear();
        }
    }

    public void DeRegisterEvent(string eventName)
    {        
        TryUnRegister(ref coverStateEvents, eventName, TransitionToPitState);
        TryUnRegister(ref floorStateEvents, eventName, TransitionToPitState);
        TryUnRegister(ref pitStateEvents, eventName, TransitionToPitState);
    }

    void TryUnRegister(ref List<string> eventNames, string eventName, EventSystem.FunctionPrototype func) {
        if (eventNames != null) {
            if (eventNames.Contains(eventName)) {
                eventNames.Remove(eventName);
                EventSystem.Instance.UnRegisterEvent(eventName, func);
            }
        }
    }

    private void CalculateStatePositions()
    {
        if(currentState == LevelUnitStates.cover)
        {
            coverYPosition = transform.position.y;
            floorYPosition = coverYPosition - meshSizeY;
            pitYPosition = floorYPosition - meshSizeY;
        }
        else if (currentState == LevelUnitStates.floor) {
            floorYPosition = transform.position.y;
            coverYPosition = floorYPosition + meshSizeY;
            pitYPosition = floorYPosition - meshSizeY;
        }
        else if (currentState == LevelUnitStates.pit) {
            pitYPosition = transform.position.y;
            floorYPosition = pitYPosition + meshSizeY;
            coverYPosition = floorYPosition + meshSizeY;
        }
    }

    private void MoveToNewPosition() {
        Vector3 newPosition = transform.position;
        switch (nextState) {
            case LevelUnitStates.cover:
                newPosition = new Vector3(transform.position.x, coverYPosition, transform.position.z);
                break;
            case LevelUnitStates.floor:
                newPosition = new Vector3(transform.position.x, floorYPosition, transform.position.z);
                break;
            case LevelUnitStates.pit:
                newPosition = new Vector3(transform.position.x, pitYPosition, transform.position.z);
                break;
        }
        if (newPosition != transform.position) {
            transform.position = Vector3.MoveTowards(transform.position, newPosition, yMoveSpeed * meshSizeY);
            if (Vector3.Distance(transform.position, newPosition) < 0.01f) {
                transform.position = newPosition;
                currentState = nextState;
                isTransitioning = false;
                if (currentState == LevelUnitStates.floor) {
                    gameObject.tag = Strings.Tags.FLOOR;
                    gameObject.layer = (int)Layers.Ground;
                    HideBlockingObject();
                }
            }
            switch (nextState) {
                case LevelUnitStates.cover:
                    gameObject.tag = Strings.Tags.WALL;
                    gameObject.layer = (int)Layers.Obstacle;
                    ShowBlockingObject();
                    break;
                case LevelUnitStates.pit:
                    gameObject.tag = Strings.Tags.FLOOR;
                    gameObject.layer = (int)Layers.Ground;
                    ShowBlockingObject();
                    break;
                default:
                    break;
            }
        }
    }

    private void AddEvent(ref List<string> eventNames, string eventName) {
        if (eventNames == null) {
            eventNames = new List<string>();
        }
        if (!eventNames.Contains(eventName)) {
            eventNames.Add(eventName);
        }
    }

    private void CreateBlockingObject() {
        blockingObject = new GameObject(Strings.BLOCKINGOBJECT);
        blockingObject.transform.SetParent(transform.parent);
        blockingObject.transform.position = new Vector3(transform.position.x, coverYPosition, transform.position.z);
        blockingObject.transform.localScale = new Vector3(meshSizeX, meshSizeY, meshSizeZ);
        blockingObject.AddComponent<BoxCollider>();
        blockingObject.AddComponent<NavMeshObstacle>().carving = true;
        blockingObject.layer = LayerMask.NameToLayer(Strings.Layers.OBSTACLE);
        blockingObject.SetActive(false);
    }

    private void ShowBlockingObject() {
        if (!blockingObject) {
            CreateBlockingObject();
        }

        if (!blockingObject.activeSelf) {
            blockingObject.SetActive(true);
        }
    }    

    void OnColorChange(float progress) {
        Color newColor = startColor + (targetColor - startColor) * progress;
        meshRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetColor(AlbedoTint, newColor);
        meshRenderer.SetPropertyBlock(materialPropertyBlock);
    }
    #endregion

    #region public functions
    public Color RiseColor { get { return riseColor; } }
    public Color FlatColor { get { return flatColor; } }
    public Color FallColor { get { return fallColor; } }
    public Color CurrentStateColor {
        get {
            switch (currentState) {
                case LevelUnitStates.cover: return riseColor;
                case LevelUnitStates.floor: return flatColor;
                case LevelUnitStates.pit: return fallColor;
            }
            return flatColor;
        }
    }

    public void SetCurrentState(LevelUnitStates newState) {
        currentState = newState;
        CalculateStatePositions();
    }

    public void RegisterToEvents() {

        RegisterEvents(ref coverStateEvents, TransitionToCoverState);
        RegisterEvents(ref floorStateEvents, TransitionToFloorState);
        RegisterEvents(ref pitStateEvents, TransitionToPitState);
    }

    public void DeRegisterFromEvents() {
        UnregisterEvents(ref coverStateEvents, TransitionToCoverState);
        UnregisterEvents(ref floorStateEvents, TransitionToFloorState);
        UnregisterEvents(ref pitStateEvents, TransitionToPitState);

        levelUnitStates.Clear();
    }

    public void ClearEvents() {
        if (coverStateEvents != null) {
            coverStateEvents.Clear();
        }
        if (floorStateEvents != null) {
            floorStateEvents.Clear();
        }
        if (pitStateEvents != null) {
            pitStateEvents.Clear();
        }
    }

    public void AddCoverStateEvent(string eventName) {
        levelUnitStates.Add(LevelUnitStates.cover);
        AddEvent(ref coverStateEvents, eventName);
    }

    public void AddFloorStateEvent(string eventName) {
        levelUnitStates.Add(LevelUnitStates.floor);
        AddEvent(ref floorStateEvents, eventName);
    }

    public void AddPitStateEvent(string eventName) {
        levelUnitStates.Add(LevelUnitStates.pit);
        AddEvent(ref pitStateEvents, eventName);
    }

    public bool CheckForEvent(string eventName, LevelUnitStates state) {
        bool result = false;
        switch (state) {
            case LevelUnitStates.cover:
                if (coverStateEvents != null) {
                    result = coverStateEvents.Contains(eventName);
                }
                break;
            case LevelUnitStates.floor:
                if (floorStateEvents != null) {
                    result = floorStateEvents.Contains(eventName);
                }
                break;
            case LevelUnitStates.pit:
                if (pitStateEvents != null) {
                    result = pitStateEvents.Contains(eventName);
                }
                break;
        }
        return result;
    }
    
    public void TransitionToCoverState(params object[] inputs)
    {
        if (currentState != LevelUnitStates.cover)
        {
            splattable.PaintMask = Texture2D.blackTexture;
            nextState = LevelUnitStates.cover;
            isTransitioning = true;
        }
        bool shouldChangeColor = (bool)inputs[0];
        if (shouldChangeColor) {
            TriggerColorShift(LevelUnitStates.cover);
        }
    }
    
    public void TransitionToFloorState(params object[] inputs)
    {
        if (currentState != LevelUnitStates.floor)
        {
            splattable.PaintMask = Texture2D.whiteTexture;
            nextState = LevelUnitStates.floor;
            isTransitioning = true;
        }
        bool shouldChangeColor = (bool)inputs[0];
        if (shouldChangeColor) {
            TriggerColorShift(LevelUnitStates.floor);
        }
    }
    
    public void TransitionToPitState(params object[] inputs)
    {
        if (currentState != LevelUnitStates.pit)
        {
            splattable.PaintMask = Texture2D.blackTexture;
            nextState = LevelUnitStates.pit;
            isTransitioning = true;
        }
        bool shouldChangeColor = (bool)inputs[0];
        if (shouldChangeColor) {
            TriggerColorShift(LevelUnitStates.pit);
        }
    }

    public void HideBlockingObject() {
        if (blockingObject != null) {
            blockingObject.SetActive(false);
        }
    }

    void TriggerColorShift(LevelUnitStates newState) {
        Timing.KillCoroutines(tintCoroutineName);
        startColor = materialPropertyBlock.GetVector(AlbedoTint);
        switch (newState) {
            case LevelUnitStates.cover: targetColor = riseColor; break;
            case LevelUnitStates.floor: targetColor = flatColor; break;
            case LevelUnitStates.pit: targetColor = fallColor; break;
        }
        colorShiftAnim.Animate(tintCoroutineName);
    }

    #endregion
}
