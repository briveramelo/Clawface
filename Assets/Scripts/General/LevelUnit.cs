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
    Cover,
    Floor,
    Pit
}
public interface ILevelTilable {
    void ClearNamedStateEventsLists();
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
    private bool isBeginningTransition;
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
    private string TintCoroutineName { get { return coroutineName + AlbedoTint; } }
    private List<LevelUnitStates> levelUnitStates = new List<LevelUnitStates>();
    private Splattable splattable;
    private static string[] masks = { Strings.Layers.ENEMY, Strings.Layers.ENEMY_BODY, Strings.Layers.MODMAN };
    #endregion

    #region serialized fields
    [SerializeField] private List<string> coverStateEvents = new List<string>();
    [SerializeField] private List<string> floorStateEvents = new List<string>();
    [SerializeField] private List<string> pitStateEvents = new List<string>();
    [SerializeField] float yMoveSpeed = 0.03f;

    [SerializeField] AbsAnim colorShiftAnim;
    [SerializeField] Color riseColor, flatColor, fallColor;

    private Vector3 pitPosition, flatPosition, risePosition, targetPosition;
    
    #endregion

    #region public variables
    public LevelUnitStates defaultState = LevelUnitStates.Floor;
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
        InitializeStatePositions();
        SetBlockColor(CurrentStateColor);
    }

    private void OnEnable() {
        RegisterToNamedStateEvents();
        EventSystem.Instance.RegisterEvent(Strings.Events.CALL_NEXT_WAVE_PLE, TransitionToWave);
    }

    protected override void OnDisable() {
        base.OnDisable();
        if (EventSystem.Instance) {
            DeRegisterFromNamedStateEvents();
            EventSystem.Instance.UnRegisterEvent(Strings.Events.CALL_NEXT_WAVE_PLE, TransitionToWave);
        }
    }

    void FixedUpdate() {
        if (isTransitioning) {
            HandleTransition();
        }
    }
    #endregion

    #region Public Interface
    #region public functions
    public Color RiseColor { get { return riseColor; } }
    public Color FlatColor { get { return flatColor; } }
    public Color FallColor { get { return fallColor; } }
    public Color CurrentStateColor { get { return GetStateColor(currentState); } }

    public void DeRegisterEvent(string eventName) {
        TryUnRegister(ref pitStateEvents, eventName, TransitionToPitState);
        TryUnRegister(ref floorStateEvents, eventName, TransitionToFloorState);
        TryUnRegister(ref coverStateEvents, eventName, TransitionToCoverState);
    }

    public void HideBlockingObject() {
        if (blockingObject != null) {
            blockingObject.SetActive(false);
        }
    }

    public void RegisterToNamedStateEvents() {
        RegisterEvents(ref coverStateEvents, TransitionToCoverState);
        RegisterEvents(ref floorStateEvents, TransitionToFloorState);
        RegisterEvents(ref pitStateEvents, TransitionToPitState);
    }

    public void DeRegisterFromNamedStateEvents() {
        UnregisterEvents(ref coverStateEvents, TransitionToCoverState);
        UnregisterEvents(ref floorStateEvents, TransitionToFloorState);
        UnregisterEvents(ref pitStateEvents, TransitionToPitState);

        levelUnitStates.Clear();
    }

    public void ClearNamedStateEventsLists() {
        coverStateEvents.Clear();
        floorStateEvents.Clear();
        pitStateEvents.Clear();
    }
    public void SetLevelUnitStates(List<LevelUnitStates> newLevelStates) {
        levelUnitStates.Clear();
        newLevelStates.ForEach(state => {
            levelUnitStates.Add(state);
        });
    }

    public void AddNamedStateEvent(LevelUnitStates state, string eventName) {
        levelUnitStates.Add(state);
        List<string> stateEvents = GetStateEvents(state);
        AddEvent(ref stateEvents, eventName);
    }

    public void TryTransitionToState(LevelUnitStates state, bool wasToldToChangeColor) {
        switch (state) {
            case LevelUnitStates.Cover: TransitionToCoverState(wasToldToChangeColor); break;
            case LevelUnitStates.Floor: TransitionToFloorState(wasToldToChangeColor); break;
            case LevelUnitStates.Pit: TransitionToPitState(wasToldToChangeColor); break;
        }
    }

    public bool CheckForEvent(string eventName, LevelUnitStates state) {
        bool result = false;
        result = GetStateEvents(state).Contains(eventName);
        return result;
    }

    public void TransitionToCoverState(params object[] inputs) {
        bool wasToldToChangeColor = (bool)inputs[0];
        TryTransitionToState(LevelUnitStates.Cover, Texture2D.blackTexture, wasToldToChangeColor);
    }

    public void TransitionToFloorState(params object[] inputs) {
        bool wasToldToChangeColor = (bool)inputs[0];
        TryTransitionToState(LevelUnitStates.Floor, Texture2D.whiteTexture, wasToldToChangeColor);
    }

    public void TransitionToPitState(params object[] inputs) {
        bool wasToldToChangeColor = (bool)inputs[0];
        TryTransitionToState(LevelUnitStates.Pit, Texture2D.blackTexture, wasToldToChangeColor);
    }

    #endregion
    #endregion

    #region Private Interface

    private void TransitionToWave(params object[] parameters) {
        int currentWaveIndex = (int)parameters[0];
        TryTransitionToState(levelUnitStates[currentWaveIndex], true);
    }
    private bool CanTransition { get { return !Physics.CheckBox(transform.position, Vector3.one * 0.5f, Quaternion.identity, LayerMask.GetMask(masks)); } }            

    private void TryUnRegister(ref List<string> eventNames, string eventName, EventSystem.FunctionPrototype func) {
        if (eventNames != null) {
            if (eventNames.Contains(eventName)) {
                eventNames.Remove(eventName);
                EventSystem.Instance.UnRegisterEvent(eventName, func);
            }
        }
    }

    private void InitializeStatePositions()
    {
        floorYPosition = transform.position.y;
        coverYPosition = floorYPosition + meshSizeY;
        pitYPosition = floorYPosition - meshSizeY;

        pitPosition = new Vector3(transform.position.x, pitYPosition, transform.position.z);
        flatPosition = new Vector3(transform.position.x, floorYPosition, transform.position.z);
        risePosition = new Vector3(transform.position.x, coverYPosition, transform.position.z);
    }

    private void HandleTransition() {
        if (CanTransition) {
            if (isBeginningTransition) {
                isBeginningTransition = false;
                BeginTransition();
            }
            MoveToNewPosition();
        }
    }

    private void MoveToNewPosition() {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, yMoveSpeed * meshSizeY);
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
            FinishTransition(targetPosition);
        }
    }

    private void BeginTransition() {
        switch (nextState) {
            case LevelUnitStates.Cover:
                gameObject.tag = Strings.Tags.WALL;
                gameObject.layer = (int)Layers.Obstacle;
                ShowBlockingObject();
                break;
            case LevelUnitStates.Pit:
                gameObject.tag = Strings.Tags.FLOOR;
                gameObject.layer = (int)Layers.Ground;
                ShowBlockingObject();
                break;
        }
    }

    private void FinishTransition(Vector3 finalPosition) {
        transform.position = finalPosition;
        currentState = nextState;
        isTransitioning = false;
        if (currentState == LevelUnitStates.Floor) {
            gameObject.tag = Strings.Tags.FLOOR;
            gameObject.layer = (int)Layers.Ground;
            HideBlockingObject();
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

    private void OnColorChange(float progress) {
        Color newColor = startColor + (targetColor - startColor) * progress;
        SetBlockColor(newColor);
    }

    private void SetBlockColor(Color newColor) {
        materialPropertyBlock.SetColor(AlbedoTint, newColor);
        meshRenderer.SetPropertyBlock(materialPropertyBlock);
    }



    private void TriggerColorShift(LevelUnitStates newState) {
        Timing.KillCoroutines(TintCoroutineName);
        startColor = CurrentColor;
        targetColor = GetStateColor(newState);
        colorShiftAnim.Animate(TintCoroutineName);
    }

    private Color CurrentColor { get { return materialPropertyBlock.GetVector(AlbedoTint); } }

    private Color GetStateColor(LevelUnitStates state) {
        switch (state) {
            case LevelUnitStates.Cover: return riseColor;
            case LevelUnitStates.Floor: return flatColor;
            case LevelUnitStates.Pit: return fallColor;
        }
        return flatColor;
    }
    private List<string> GetStateEvents(LevelUnitStates state) {
        switch (state) {
            case LevelUnitStates.Pit: return pitStateEvents;
            case LevelUnitStates.Floor: return floorStateEvents;
            case LevelUnitStates.Cover: return coverStateEvents;
        }
        return null;
    }
    private Vector3 GetStatePosition(LevelUnitStates state) {
        switch (state) {
            case LevelUnitStates.Cover: return risePosition;
            case LevelUnitStates.Floor: return flatPosition;
            case LevelUnitStates.Pit: return pitPosition;
        }
        return flatPosition;
    }

    private void RegisterEvents(ref List<string> eventNames, EventSystem.FunctionPrototype func) {
        if (eventNames != null) {
            foreach (string eventName in eventNames) {
                EventSystem.Instance.RegisterEvent(eventName, func);
            }
        }
    }

    private void UnregisterEvents(ref List<string> eventNames, EventSystem.FunctionPrototype func) {
        if (eventNames != null) {
            foreach (string eventName in eventNames) {
                EventSystem.Instance.UnRegisterEvent(eventName, func);
            }
            eventNames.Clear();
        }
    }

    private void TryTransitionToState(LevelUnitStates newState, Texture2D paintMaskTexture, bool wasToldToChangeColor) {
        if ((isTransitioning && currentState==newState) || currentState != newState) {
            splattable.PaintMask = Texture2D.blackTexture;
            nextState = newState;
            targetPosition = GetStatePosition(nextState);
            isBeginningTransition = true;
            isTransitioning = true;
        }
        bool shouldChangeColor = wasToldToChangeColor && !CurrentColor.IsAboutEqual(GetStateColor(newState));
        if (shouldChangeColor) {
            TriggerColorShift(newState);
        }
    }
    #endregion

    
}
