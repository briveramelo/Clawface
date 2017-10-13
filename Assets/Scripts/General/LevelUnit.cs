using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelUnitStates
{    
    cover,
    floor,
    pit
}

public class LevelUnit : MonoBehaviour {

    #region private variables
    private float meshHeight;
    private bool isTransitioning;
    private float pitYPosition;
    private float coverYPosition;
    private float floorYPosition;
    private LevelUnitStates currentState;
    private LevelUnitStates nextState;
    private float speed = 0.05f;
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
    public LevelUnitStates defaultState;
    #endregion

    #region unity lifecycle
    private void Awake()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer)
        {
            meshHeight = meshRenderer.bounds.size.y;
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
        if (isTransitioning)
        {
            MoveToNewPosition();
        }
    }
    #endregion

    #region private functions
    private void RegisterToEvents()
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
    
    private void DeRegisterFromEvents()
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
            floorYPosition = coverYPosition - meshHeight;
            pitYPosition = floorYPosition - meshHeight;
        }else if(currentState == LevelUnitStates.floor)
        {
            floorYPosition = transform.position.y;
            coverYPosition = floorYPosition + meshHeight;
            pitYPosition = floorYPosition - meshHeight;
        }else if(currentState == LevelUnitStates.pit)
        {
            pitYPosition = transform.position.y;
            floorYPosition = pitYPosition + meshHeight;
            coverYPosition = floorYPosition + meshHeight;
        }
    }

    public void TransitionToCoverState(params object[] inputs)
    {
        if (currentState != LevelUnitStates.cover)
        {
            nextState = LevelUnitStates.cover;
            isTransitioning = true;
            Debug.Log(name + " transitioning to " + LevelUnitStates.cover.ToString());
        }
    }

    public void TransitionToFloorState(params object[] inputs)
    {
        if(currentState != LevelUnitStates.floor)
        {
            nextState = LevelUnitStates.floor;
            isTransitioning = true;
            Debug.Log(name + " transitioning to " + LevelUnitStates.floor.ToString());
        }
    }

    public void TransitionToPitState(params object[] inputs)
    {
        if (currentState != LevelUnitStates.pit)
        {
            nextState = LevelUnitStates.pit;
            isTransitioning = true;
            Debug.Log(name + " transitioning to " + LevelUnitStates.pit.ToString());
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
            transform.position = Vector3.MoveTowards(transform.position, newPosition, speed);
            if (Vector3.Distance(transform.position, newPosition) < 0.01f)
            {
                transform.position = newPosition;
                currentState = nextState;
                isTransitioning = false;
            }
        }
    }

    private void AddEvent(List<string> eventNames, string eventName)
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
        AddEvent(coverStateEvents, eventName);
    }

    public void AddFloorStateEvent(string eventName)
    {
        AddEvent(floorStateEvents, eventName);
    }

    public void AddPitStateEvent(string eventName)
    {
        AddEvent(pitStateEvents, eventName);
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
    #endregion

}
