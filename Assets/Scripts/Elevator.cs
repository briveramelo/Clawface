//Elevator created by Lai
//Use keyboard A to tirgger
//Player has to have Rigidbody with Gravity

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{

    public float moveTime = 10.0f;
    public float moveSpeed = 1.0f;

    private bool isMoving = false;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.A) && isMoving == false) 
        {
            StartCoroutine(moveElevator());
        }
    }


    IEnumerator moveElevator()
    {
        float time = moveTime;
        isMoving = true;

        while(time >= 0.0f)
        {
            transform.Translate(moveSpeed * Vector3.up * Time.fixedDeltaTime);
            time -= Time.fixedDeltaTime;
            yield return 0;
        }

        yield return new WaitForSeconds(2);
        time = moveTime;

        while (time >= 0.0f)
        {
            transform.Translate(moveSpeed * Vector3.down * Time.fixedDeltaTime);
            time -= Time.fixedDeltaTime;
            yield return 0;
        }

        time = moveTime;
        isMoving = false;
    }
}
