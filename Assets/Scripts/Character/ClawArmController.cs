using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ClawArmController : MonoBehaviour {

    #region serialized fields
    [SerializeField]
    private Transform start;
    [SerializeField]
    private GameObject arm;
    [SerializeField]
    private Transform end;
    [SerializeField]
    private ClawAnimationHandler clawAnimationHandler;    
    #endregion

    #region private fields
    private float meshSizeZ;
    private GameObject target;
    private bool extending;
    private bool retracting;
    private float extendIncrement;
    private float retractIncrement;
    #endregion

    #region unity lifecycle
    // Use this for initialization
    void Start () {
        Assert.IsNotNull(start);
        Assert.IsNotNull(arm);
        Assert.IsNotNull(end);
        Assert.IsNotNull(clawAnimationHandler);
        meshSizeZ = arm.GetComponent<MeshRenderer>().bounds.size.z;
        clawAnimationHandler.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        Assert.IsFalse(extending && retracting);

        if (extending)
        {
            ExtendArm();
        }

        if (retracting)
        {
            RetractArm();
        }

        ResizeArm();
    }
    #endregion

    #region Public functions
    public void StartExtension(GameObject target, float clawExtensionTime)
    {        
        this.target = target;
        extending = true;
        clawAnimationHandler.gameObject.SetActive(true);
        //clawAnimationHandler.StartAnimation();
        extendIncrement = 1 / (clawExtensionTime / Time.fixedDeltaTime);
    }

    public void StartRetraction(float clawRetractionTime)
    {
        extending = false;
        retracting = true;
        retractIncrement = 1 / (clawRetractionTime / Time.fixedDeltaTime);
    }

    public void ResetClawArm()
    {
        clawAnimationHandler.gameObject.SetActive(false);
        target = null;
        extending = false;
        retracting = false;
    }
    #endregion

    #region Private functions
    private void ResizeArm()
    {
        arm.transform.position = (start.position + end.position) / 2f;
        float zScale = Vector3.Distance(start.position, end.position) / meshSizeZ;
        arm.transform.localScale = new Vector3(1.0f, 1.0f, zScale);
        arm.transform.forward = (end.position - start.position).normalized;
    }

    private void ExtendArm()
    {        
        if(Vector3.Distance(end.position, target.transform.position) <= 0.1f)
        {
            end.position = target.transform.position;
            EventSystem.Instance.TriggerEvent(Strings.Events.ARM_EXTENDED, end);
            //clawAnimationHandler.FinishAnimation();
        }
        else
        {            
            end.position = Vector3.Lerp(end.position, target.transform.position, extendIncrement);
        }
    }

    private void RetractArm()
    {        
        if(Vector3.Distance(end.position, start.position) <= 0.1f)
        {
            end.position = start.position;
            EventSystem.Instance.TriggerEvent(Strings.Events.ARM_ANIMATION_COMPLETE);
        }
        else
        {
            end.position = Vector3.Lerp(end.position, start.position, retractIncrement);
        }
    }
    #endregion

}