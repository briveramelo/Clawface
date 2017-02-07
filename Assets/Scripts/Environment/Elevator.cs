//Elevator created by Lai
//Use keyboard A and S to tirgger

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour, ITriggerable
{
    enum State
    {
        Up = 0,
        Moving = 1, 
        Down = 2,
    }

    public float moveSpeed = 1.0f;
    public GameObject MaxPointObject;

    private State m_state = State.Down;
    private Vector3 initPosition = Vector3.zero;
    private Vector3 maxPosition = Vector3.zero;
    private GameObject playerObject;

    // Use this for initialization
    void Start ()
    {
        initPosition = transform.position;
        maxPosition = MaxPointObject.transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
   
    }

    void OnTriggerStay(Collider other)
    {
        if (Input.GetKey(KeyCode.A) && m_state == State.Down)
        {
            playerObject = other.gameObject;
            Activate();
        }
        else if (Input.GetKey(KeyCode.S) && m_state == State.Up)
        {
            playerObject = other.gameObject;
            Activate();
        }
    }

    IEnumerator moveUp()
    {
        m_state = State.Moving;

        while (transform.position.y <= maxPosition.y)
        {
            transform.Translate(moveSpeed * Vector3.up * Time.fixedDeltaTime);
            yield return 0;
        }

        playerObject.transform.parent = null;
        yield return new WaitForSeconds(2);
        m_state = State.Up;
    }


    IEnumerator moveDown()
    {
        m_state = State.Moving;

        while (transform.position.y >= initPosition.y)
        {
            transform.Translate(moveSpeed * Vector3.down * Time.fixedDeltaTime);
            yield return 0;
        }

        playerObject.transform.parent = null;
        yield return new WaitForSeconds(2);
        m_state = State.Down;
    }

    public void Activate()
    {
        playerObject.transform.parent = transform;

        if (m_state == State.Down)
        {
            StartCoroutine(moveUp());
        }

        if (m_state == State.Up)
        {
            StartCoroutine(moveDown());
        }
    }

    public void Deactivate() { }
    public void Notify() { }
    public void Wait() { }
}
