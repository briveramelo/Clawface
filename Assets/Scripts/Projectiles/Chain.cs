using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour {

    #region Public fields
    #endregion

    #region Serialized unity inspector fields
    [SerializeField] Transform start;
    [SerializeField] Transform end;
    #endregion

    #region Private fields
    Vector3 initialScale;
    #endregion

    #region Unity lifecycle
    private void Awake()
    {
        initialScale = transform.localScale;
    }

    // Use this for initialization
    void Start () {
        
    }

    private void OnDisable()
    {
        transform.position = start.position;
        transform.localScale = initialScale;
    }

    // Update is called once per frame
    void Update () {
		if(start && end)
        {
            Vector3 distanceVector = end.position - start.position;
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, distanceVector.magnitude);
            transform.LookAt(end);
            transform.position = start.position + distanceVector / 2f;
        }
	}
	#endregion
	
	#region Public methods
    #endregion
	
	#region Private methods
    #endregion
	
	#region Public structures
    #endregion
	
	#region Private structures
    #endregion
}
