using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum LevelUnitStates
{    
    cover,
    floor,
    pit
}

public class LevelUnit : MonoBehaviour {

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
    private float speed = 0.05f;
    private GameObject blockingObject;
    private int overlappingObjects;
    #endregion

    #region serialized fields
    [SerializeField]
    private List<string> coverStateEvents;
    [SerializeField]
    private List<string> floorStateEvents;
    [SerializeField]
    private List<string> pitStateEvents;
    #endregion

    #region public variables
    public LevelUnitStates defaultState = LevelUnitStates.floor;
    #endregion

    #region unity lifecycle
    private void Awake()
    {
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

    void LateUpdate () {
        if (isTransitioning && overlappingObjects == 0)
        {
            MoveToNewPosition();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag.Equals(Strings.Tags.PLAYER) || collision.gameObject.tag.Equals(Strings.Tags.ENEMY))
        {
            if (collision.transform.position.y >= transform.position.y)
            {
                overlappingObjects++;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag.Equals(Strings.Tags.PLAYER) || collision.gameObject.tag.Equals(Strings.Tags.ENEMY))
        {
            overlappingObjects--;
            if(overlappingObjects < 0)
            {
                overlappingObjects = 0;
            }
        }
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
            transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * meshSizeY);
            if (Vector3.Distance(transform.position, newPosition) < 0.01f)
            {
                transform.position = newPosition;
                currentState = nextState;
                isTransitioning = false;
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
        blockingObject.AddComponent<NavMeshObstacle>();
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
            nextState = LevelUnitStates.cover;
            isTransitioning = true;
            ShowBlockingObject();
        }
    }

    public void TransitionToFloorState(params object[] inputs)
    {
        if (currentState != LevelUnitStates.floor)
        {
            nextState = LevelUnitStates.floor;
            isTransitioning = true;
            if (blockingObject)
            {
                blockingObject.SetActive(false);
            }
        }
    }

    public void TransitionToPitState(params object[] inputs)
    {
        if (currentState != LevelUnitStates.pit)
        {
            nextState = LevelUnitStates.pit;
            isTransitioning = true;
            ShowBlockingObject();
        }
    }

    public void DisableBlockingObject()
    {
        if (blockingObject != null)
        {
            blockingObject.SetActive(false);
        }
    }

    #endregion
}
