using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Door : MonoBehaviour
{
    enum Component
    {
        UpperComponent = 0, 
        LowerComponent = 1,
    }
    public float maxHeight = 150.0f;
    public float moveSpeed = 1.0f;

    private GameObject MovePart;
    private bool isOpening = false;

    public bool isOpen { get { return isOpening; } }

    // Use this for initialization
    void Start()
    {
        MovePart = transform.GetChild((int)Component.LowerComponent).gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter()
    {
        if (isOpening == false)
            StartCoroutine(openDoor());
    }

    public IEnumerator openDoor()
    {
        isOpening = true;

        for (float i = 0; i < maxHeight; i += 1.0f)
        {
            MovePart.transform.Translate(Vector3.up * moveSpeed * Time.fixedDeltaTime);
            yield return 0;
        }

        yield return new WaitForSeconds(2);

        for (float i = 0; i < maxHeight; i += 1.0f)
        {
            MovePart.transform.Translate(Vector3.down * moveSpeed * Time.fixedDeltaTime);
            yield return 0;
        }

        isOpening = false;
    }


}
