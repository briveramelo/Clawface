//Elevator created by Lai
//Use keyboard A to tirgger
//Player has to have Rigidbody with Gravity

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    enum State
    {
        Up = 0,
        Moving = 1, 
        Down = 2,
    }

    public float moveSpeed = 1.0f;

    private State m_state = State.Down;
    private Vector3 initPosition = Vector3.zero;
    private Vector3 maxPosition = Vector3.zero;

    // Use this for initialization
    void Start ()
    {
        initPosition = transform.position;
        maxPosition = transform.GetChild(0).transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.A) && m_state == State.Down) 
        {
            StartCoroutine(moveUp());
        }

        if (Input.GetKey(KeyCode.S) && m_state == State.Up)
        {
            StartCoroutine(moveDown());
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

        yield return new WaitForSeconds(2);
        m_state = State.Down;
    }
}
