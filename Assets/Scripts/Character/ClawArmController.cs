using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ClawArmController : MonoBehaviour {

    #region serialized fields
    [SerializeField] private Transform start;
    [SerializeField] private GameObject arm;
    [SerializeField] private Transform end;
    [SerializeField] private Transform clawRoot;
    [SerializeField] private ClawAnimationHandler clawAnimationHandler;
    #endregion

    public bool IsExtending { get; private set; }
    public bool IsRetracting { get; private set; }

    #region private fields
    private float meshSizeZ;
    private GameObject target;
    private float extendTime;
    private float retractTime;

    private float ExtendSpeed {
        get {
            return Vector3.Distance(end.position, target.transform.position) / extendTime;
        }
    }

    private float RetractSpeed {
        get {
            return Vector3.Distance(end.position, start.position) / retractTime;
        }
    }
    #endregion

    #region unity lifecycle    
    // Use this for initialization
    void Awake() {
        Assert.IsNotNull(start);
        Assert.IsNotNull(arm);
        Assert.IsNotNull(end);
        Assert.IsNotNull(clawAnimationHandler);
        meshSizeZ = arm.GetComponent<MeshRenderer>().bounds.size.z;
    }

    private void Start() {
        clawAnimationHandler.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        Assert.IsFalse(IsExtending && IsRetracting);

        if (IsExtending) {
            ExtendArm();
        }

        if (IsRetracting) {
            RetractArm();
        }

        ResizeArm();
    }
    #endregion

    #region Public functions
    public void StartExtension(GameObject target, float clawExtensionTime, float clawRetractionTime) {
        this.target = target;
        extendTime = clawExtensionTime;
        retractTime = clawRetractionTime;
        clawAnimationHandler.gameObject.SetActive(true);
        IsExtending = true;
    }

    public void ResetClawArm() {
        IsExtending = false;
        IsRetracting = false;
        target = null;
        clawAnimationHandler.gameObject.SetActive(false);
        end.position = start.position;
        end.forward = start.forward;
    }

    public void ExtendClawToDistance(float radius, bool enableCollider = false) {
        clawAnimationHandler.gameObject.SetActive(true);
        end.position = start.position + start.forward * radius;
    }

    public Vector3 GetEndPosition() {
        return end.position;
    }
    #endregion

    #region Private functions
    private void StartRetraction() {
        IsExtending = false;
        IsRetracting = true;
    }

    private void ResizeArm() {
        arm.transform.position = (start.position + end.position) / 2f;
        float zScale = Vector3.Distance(start.position, end.position) / meshSizeZ;
        arm.transform.localScale = new Vector3(1.0f, 1.0f, zScale);
        if (end.position != start.position) {
            arm.transform.forward = (end.position - start.position).normalized;
        }
    }

    private void ExtendArm() {
        if (!target.activeSelf) {
            EventSystem.Instance.TriggerEvent(Strings.Events.CAPTURE_ENEMY, end);
        }
        else {
            Vector3 directionToTarget = (target.transform.position - end.position).normalized;
            end.position += directionToTarget * ExtendSpeed * Time.deltaTime;
            end.LookAt(target.transform.position);

            extendTime -= Time.deltaTime;
            if (Vector3.Dot(directionToTarget, end.forward) <= 0 || extendTime <= 0) {
                end.position = target.transform.position;
                Vector3 toTargetFromBody = (target.transform.position - clawRoot.position).normalized;
                end.rotation = Quaternion.LookRotation(toTargetFromBody, Vector3.up);
                EventSystem.Instance.TriggerEvent(Strings.Events.CAPTURE_ENEMY, end);
                StartRetraction();
            }
        }
    }

    private void RetractArm() {
        Vector3 directionToFace = (start.position - end.position).normalized;
        end.position += directionToFace * RetractSpeed * Time.deltaTime;
        target.transform.position = end.position;
        retractTime -= Time.deltaTime;
        bool isArmPointingBack = Vector3.Dot(directionToFace, end.forward) >= 0;
        bool isTimeUp = retractTime <= 0;
        if (isArmPointingBack || isTimeUp) {
            IsRetracting = false;
            end.position = start.position;
            EventSystem.Instance.TriggerEvent(Strings.Events.ARM_ANIMATION_COMPLETE);
        }
    }
    #endregion

}