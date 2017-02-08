using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Televator : MonoBehaviour, ITriggerable
{
    //// Unity Inspector Fields
    [SerializeField]
    private Transform player, arenaTarget, armoryTarget, trialsTarget;
    [SerializeField]
    private TelevatorUI televatorUI;

    //// Private State
    private bool ready = false;
    private bool changed = false;
    private TelevatorUI.Floor selected = TelevatorUI.Floor.ARMORY;

    //// ITriggerable Interface
    public void Activate()
    {
        switch (selected)
        {
            case TelevatorUI.Floor.ARENA:
                player.position = arenaTarget.position;
                break;
            case TelevatorUI.Floor.ARMORY:
                player.position = armoryTarget.position;
                break;
            case TelevatorUI.Floor.TRIALS:
                player.position = trialsTarget.position;
                break;
            default:
                throw new SystemException("IMPOSSIBRU!");
        }
    }

    public void Deactivate()
    {
        // Do nothing
    }

    public void Notify()
    {
        televatorUI.SetVisible(true);
        televatorUI.SelectFloor(selected);
        ready = true;
    }

    public void Wait()
    {
        televatorUI.SetVisible(false);
        ready = false;
    }

    //// Unity State Functions
    private void Update()
    {
        if (ready)
        {
            if (Input.GetAxis(Strings.DPAD_Y) < 0 && !changed)
            {
                selected = IncrementFloor(selected);
            } else if (Input.GetAxis(Strings.DPAD_Y) > 0 && !changed)
            {
                selected = DecrementFloor(selected);
            } else if (Input.GetAxis(Strings.DPAD_Y) == 0)
            {
                changed = false;
                return;
            }

            changed = true;
            televatorUI.SelectFloor(selected);
        }
    }

    //// Private Functions
    private TelevatorUI.Floor IncrementFloor(TelevatorUI.Floor floor)
    {
        switch (floor)
        {
            case TelevatorUI.Floor.ARENA:
                return TelevatorUI.Floor.ARENA;
            case TelevatorUI.Floor.ARMORY:
                return TelevatorUI.Floor.TRIALS;
            case TelevatorUI.Floor.TRIALS:
                return TelevatorUI.Floor.ARENA;
            default:
                throw new SystemException("IMPOSSIBRU!");
        }
    }
    private TelevatorUI.Floor DecrementFloor(TelevatorUI.Floor floor)
    {
        switch (floor)
        {
            case TelevatorUI.Floor.ARENA:
                return TelevatorUI.Floor.TRIALS;
            case TelevatorUI.Floor.ARMORY:
                return TelevatorUI.Floor.ARMORY;
            case TelevatorUI.Floor.TRIALS:
                return TelevatorUI.Floor.ARMORY;
            default:
                throw new SystemException("IMPOSSIBRU!");
        }
    }
}
