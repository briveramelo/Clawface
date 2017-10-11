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
    private LevelUnitStates currentState = LevelUnitStates.floor;
    private LevelUnitStates nextState;
    #endregion

    #region serialized fields
    [HideInInspector]
    private float speed = 0.01f;
    #endregion

    #region public variables
    #endregion

    #region unity lifecycle
    private void Awake()
    {
        MeshFilter meshfilter = GetComponent<MeshFilter>();
        if (meshfilter)
        {
            meshHeight = meshfilter.mesh.bounds.size.y;
        }
        CalculateStatePositions();
    }    

    void LateUpdate () {
        if (isTransitioning)
        {
            MoveToNewPosition();

        }
    }
    #endregion

    #region private functions
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
        }
    }

    public void TransitionToFloorState(params object[] inputs)
    {
        if(currentState != LevelUnitStates.floor)
        {
            nextState = LevelUnitStates.floor;
            isTransitioning = true;
        }
    }

    public void TransitionToPitState(params object[] inputs)
    {
        if (currentState != LevelUnitStates.pit)
        {
            nextState = LevelUnitStates.pit;
            isTransitioning = true;
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
    #endregion

    #region public functions
    public void SetCurrentState(LevelUnitStates newState)
    {
        currentState = newState;
        CalculateStatePositions();
    }
    #endregion
}
