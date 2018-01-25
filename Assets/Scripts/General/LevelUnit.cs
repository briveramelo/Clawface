﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using MovementEffects;
public enum LevelUnitStates
{    
    cover,
    floor,
    pit
}

public class LevelUnit : RoutineRunner {

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
    List<Collider> overlappingColliders=new List<Collider>(10);
    private int overlappingObjects;
    private Splattable splattable;
    #endregion

    #region serialized fields
    [SerializeField]
    private List<string> coverStateEvents;
    [SerializeField]
    private List<string> floorStateEvents;
    [SerializeField]
    private List<string> pitStateEvents;
    [SerializeField] float yMoveSpeed = 0.03f;
    #endregion

    #region public variables
    public LevelUnitStates defaultState = LevelUnitStates.floor;
    #endregion

    #region unity lifecycle
    private void Awake()
    {
        splattable = GetComponent<Splattable>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer)
        {
            meshSizeY = meshRenderer.bounds.size.y;
            meshSizeZ = meshRenderer.bounds.size.z;
            meshSizeX = meshRenderer.bounds.size.x;
        }        
        currentState = defaultState;
        CalculateStatePositions();
    }

    private void Start()
    {
        RegisterToEvents();
    }

    private void OnDestroy()
    {
        if (EventSystem.Instance)
        {
            DeRegisterFromEvents();
        }
    }

    void FixedUpdate () {
        if (isTransitioning)
        {
            if (overlappingObjects == 0) {
                MoveToNewPosition();
            }
        }        
    }

    private void OnTriggerStay(Collider other) {
        if (isTransitioning) {
            if (other.gameObject.tag.Equals(Strings.Tags.PLAYER) || other.gameObject.tag.Equals(Strings.Tags.ENEMY)) {                
                if (!overlappingColliders.Contains(other)) {                    
                    overlappingObjects++;
                    overlappingColliders.Add(other);
                    Timing.RunCoroutine(WaitToRemove(other), Segment.FixedUpdate, coroutineName);
                }
            }
        }        
    }

    IEnumerator<float> WaitToRemove(Collider other) {
        yield return Timing.WaitForSeconds(.25f);
        overlappingColliders.Remove(other);        
        yield return 0f;
        overlappingObjects--;
    }
    #endregion

    #region private functions
    public void RegisterToEvents()
    {
        if (coverStateEvents != null)
        {
            foreach (string eventName in coverStateEvents)
            {
                EventSystem.Instance.RegisterEvent(eventName, TransitionToCoverState);
            }
        }

        if (floorStateEvents != null)
        {
            foreach (string eventName in floorStateEvents)
            {
                EventSystem.Instance.RegisterEvent(eventName, TransitionToFloorState);
            }
        }

        if (pitStateEvents != null)
        {
            foreach (string eventName in pitStateEvents)
            {
                EventSystem.Instance.RegisterEvent(eventName, TransitionToPitState);
            }
        }
    }

    public void DeRegisterFromEvents()
    {
        if (coverStateEvents != null)
        {
            foreach (string eventName in coverStateEvents)
            {
                EventSystem.Instance.UnRegisterEvent(eventName, TransitionToCoverState);
            }
        }

        if (floorStateEvents != null)
        {
            foreach (string eventName in floorStateEvents)
            {
                EventSystem.Instance.UnRegisterEvent(eventName, TransitionToFloorState);
            }
        }

        if (pitStateEvents != null)
        {
            foreach (string eventName in pitStateEvents)
            {
                EventSystem.Instance.UnRegisterEvent(eventName, TransitionToPitState);
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
        }else if(currentState == LevelUnitStates.floor)
        {
            floorYPosition = transform.position.y;
            coverYPosition = floorYPosition + meshSizeY;
            pitYPosition = floorYPosition - meshSizeY;
        }else if(currentState == LevelUnitStates.pit)
        {
            pitYPosition = transform.position.y;
            floorYPosition = pitYPosition + meshSizeY;
            coverYPosition = floorYPosition + meshSizeY;
        }
    }    

    private void MoveToNewPosition()
    {
        Vector3 newPosition = transform.position;
        switch (nextState)
        {
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
        if (newPosition != transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPosition, yMoveSpeed * meshSizeY);
            if (Vector3.Distance(transform.position, newPosition) < 0.01f)
            {
                transform.position = newPosition;
                currentState = nextState;
                isTransitioning = false;
                if(currentState == LevelUnitStates.floor)
                {
                    gameObject.tag = Strings.Tags.FLOOR;
                    gameObject.layer = (int)Layers.Ground;
                    HideBlockingObject();
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
    }

    private void AddEvent(ref List<string> eventNames, string eventName)
    {
        if(eventNames == null)
        {
            eventNames = new List<string>();
        }
        if (!eventNames.Contains(eventName))
        {
            eventNames.Add(eventName);
        }
    }

    private void CreateBlockingObject()
    {        
        blockingObject = new GameObject("Blocking Object");
        blockingObject.SetActive(false);        
        blockingObject.transform.SetParent(transform.parent);
        blockingObject.transform.position = new Vector3(transform.position.x, coverYPosition, transform.position.z);
        blockingObject.transform.localScale = new Vector3(meshSizeX, meshSizeY, meshSizeZ);
        blockingObject.AddComponent<BoxCollider>();
        blockingObject.AddComponent<NavMeshObstacle>().carving = true;
        blockingObject.layer = LayerMask.NameToLayer(Strings.Layers.OBSTACLE);
    }

    private void ShowBlockingObject()
    {
        if (!blockingObject)
        {
            CreateBlockingObject();
        }
        blockingObject.SetActive(true);
    }
    #endregion

    #region public functions
    public void SetCurrentState(LevelUnitStates newState)
    {
        currentState = newState;
        CalculateStatePositions();
    }

    public void ClearEvents()
    {
        if (coverStateEvents != null)
        {
            coverStateEvents.Clear();
        }
        if (floorStateEvents != null)
        {
            floorStateEvents.Clear();
        }
        if (pitStateEvents != null)
        {
            pitStateEvents.Clear();
        }
    }

    public void AddCoverStateEvent(string eventName)
    {
        AddEvent(ref coverStateEvents, eventName);
    }

    public void AddFloorStateEvent(string eventName)
    {
        AddEvent(ref floorStateEvents, eventName);
    }

    public void AddPitStateEvent(string eventName)
    {
        AddEvent(ref pitStateEvents, eventName);
    }

    public bool CheckForEvent(string eventName, LevelUnitStates state)
    {
        bool result = false;
        switch (state)
        {
            case LevelUnitStates.cover:
                if (coverStateEvents != null)
                {
                    result = coverStateEvents.Contains(eventName);
                }
                break;
            case LevelUnitStates.floor:
                if (floorStateEvents != null)
                {
                    result = floorStateEvents.Contains(eventName);
                }
                break;
            case LevelUnitStates.pit:
                if (pitStateEvents != null)
                {
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
    }

    public void TransitionToFloorState(params object[] inputs)
    {
        
        if (currentState != LevelUnitStates.floor)
        {
            splattable.PaintMask = Texture2D.whiteTexture;
            nextState = LevelUnitStates.floor;
            isTransitioning = true;
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
    }

    public void HideBlockingObject()
    {
        if (blockingObject != null)
        {
            blockingObject.SetActive(false);
        }
    }

    #endregion
}
