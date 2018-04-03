using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ModMan;
using MEC;
namespace PlayerLevelEditor
{
    [RequireComponent(typeof(Camera))]
    public class PLECameraController : RoutineRunner
    {
        #region Serialized Fields
        [SerializeField] private LevelEditor levelEditor;
        [SerializeField] private MainPLEMenu mainPLEMenu;
        [SerializeField] private AbsAnim recenterAnim;
        [SerializeField] private float maxDistanceOnRecenter;
        //[SerializeField, Range(0f,1f)] private float timeBetweenDoubleClicks;
        [SerializeField] private float panSpeedBase;
        [SerializeField] private float WASDSpeedBase;
        [SerializeField] private float zoomScrollSpeed;
        [SerializeField] private float zoomScrubSpeed;
        [SerializeField] private float zPanSpeed;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float heightOffsetSpeedMultiplier;
        [SerializeField] private float minFarClipPlain;
        #endregion

        #region Getter Helpers
        private float CameraDistanceAway { get { return Vector3.Distance(mainCamera.transform.position, Vector3.zero); } }
        private float CameraYDistanceAway { get { return mainCamera.transform.position.y; } }
        private float CameraYDistanceMuliplier { get { return heightOffsetSpeedMultiplier * CameraYDistanceAway; } }
        private float WASDSpeedAdjusted { get { return (WASDSpeedBase + CameraYDistanceMuliplier) * Time.deltaTime; } }
        private float PanSpeedAdjusted { get { return (panSpeedBase - CameraYDistanceMuliplier); } }

        private float ZPanSpeedAdjusted { get { return (zPanSpeed /* use camera offset??? */ ) * Time.deltaTime; } }
        private float RotationSpeedAdjusted { get { return rotationSpeed * Time.deltaTime; } }
        private float ScrollSpeedAdjusted { get { return zoomScrollSpeed * Time.deltaTime; } }
        private float ZoomScrubSpeedAdjusted { get { return zoomScrubSpeed; } }
        //private bool DoubleClicked { get { return Input.GetMouseButtonDown(MouseButtons.LEFT) && Time.time - lastClickTime < timeBetweenDoubleClicks; } }
        #endregion

        #region Private Fields
        float lastClickTime;

        private Camera mainCamera;
        Vector3 startScreenPosition, startCamPosition;
        private Vector3 startPosition, targetPosition;
        private Quaternion startRotation, targetRotation;

        private float levelSize;//assumes levelSize is set and unchanged after the start funciton is called
        private float yaw = 0.0f;
        private float pitch = 0.0f;
        private PlacementMenu spawnMenu;
        #endregion


        #region Unused... Delete?
        private Material lineMaterial;
        private Color lineColor = Color.red;
        private List<Vector3> vertices = new List<Vector3>();
        void Awake()
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
        }

        public void DrawLine(Vector3 p1, Vector3 p2) {
            if (p1 == null || p2 == null) {
                Debug.Log("p1 or p2 is null");
                return;
            }

            vertices.Add(p1);
            vertices.Add(p2);
        }

        public void SetLineColor(Color color) {
            lineColor = color;
        }

        #endregion



        #region Unity LifeCycle
        private void Start() {
            spawnMenu = (mainPLEMenu.GetMenu(PLEMenu.SPAWN) as PlacementMenu);
            recenterAnim.OnUpdate = UpdateCameraRecenter;

            levelSize = levelEditor.gridController.LevelSize;
            mainCamera = Camera.main;
        }

        private void Update() {
            HandleRecentering();
            HandleCameraMovement();
            HandleCameraRotation();
            HandleCameraZooming();
            HandleClippingPlanes();
        }

        private void OnApplicationQuit() {
            DestroyImmediate(lineMaterial);
        }
        #endregion

        #region Private Interface    
        private void HandleRecentering() {
            if (Input.GetKeyDown(KeyCode.F)) {
                SetupRecenter();
            }

            //if (DoubleClicked) {                
            //    SetupRecenter();
            //}
            //if (Input.GetMouseButtonDown(MouseButtons.LEFT)) {
            //    lastClickTime = Time.time;
            //}
        }

        private void SetupRecenter() {
            Transform target = levelEditor.gridController.GetFirstSelectedTile() ?? spawnMenu.SelectedItem;
            if (target != null) {
                Recenter(target);
            }
        }

        private void Recenter(Transform target) {
            Timing.KillCoroutines(coroutineName);
            
            startPosition = transform.position;
            startRotation = transform.rotation;
            targetPosition = target.position - Vector3.ClampMagnitude(target.position - startPosition, maxDistanceOnRecenter);
            targetRotation = Quaternion.LookRotation(target.position - startPosition, Vector3.up);
            recenterAnim.Animate(coroutineName);
        }
        private void UpdateCameraRecenter(float progress) {
            transform.position = startPosition + progress * (targetPosition - startPosition);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, progress);
        }

        private void HandleCameraMovement() {
            HandleCameraWASD();
            HandleCameraPanning();
        }

        private void HandleCameraWASD() {
            if (Input.GetKey(KeyCode.W)) {
                transform.Translate(WASDSpeedAdjusted * transform.TransformDirection(Vector3.up));
            }

            if (Input.GetKey(KeyCode.A)) {
                transform.Translate(WASDSpeedAdjusted * transform.TransformDirection(Vector3.left));
            }

            if (Input.GetKey(KeyCode.S)) {
                transform.Translate(WASDSpeedAdjusted * transform.TransformDirection(Vector3.down));
            }

            if (Input.GetKey(KeyCode.D)) {
                transform.Translate(WASDSpeedAdjusted * transform.TransformDirection(Vector3.right));
            }

        }

        private void HandleCameraPanning() {
            if (Input.GetMouseButtonDown(MouseButtons.MIDDLE) || (Input.GetMouseButtonDown(MouseButtons.LEFT) && Input.GetKey(KeyCode.Space))) {
                startScreenPosition = Input.mousePosition;
                startCamPosition = transform.position;
            }

            if (Input.GetMouseButton(MouseButtons.MIDDLE) || (Input.GetMouseButton(MouseButtons.LEFT) && Input.GetKey(KeyCode.Space))) {
                Vector3 screenDiff = Input.mousePosition - startScreenPosition;
                screenDiff.z = screenDiff.y;
                screenDiff.y = 0;
                screenDiff = transform.TransformDirection(screenDiff);
                screenDiff.y = 0;
                transform.position = startCamPosition + screenDiff * PanSpeedAdjusted;
            }
        }

        private void HandleCameraZooming() {            
            if (Input.GetAxis("Mouse ScrollWheel") < 0) {
                transform.Translate(Vector3.forward * ScrollSpeedAdjusted);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0) {
                transform.Translate(Vector3.back * ScrollSpeedAdjusted);
            }

            if (Input.GetMouseButtonDown(MouseButtons.LEFT) && Input.GetKey(KeyCode.Z)) {
                startScreenPosition = Input.mousePosition;
                startCamPosition = transform.position;
            }
            if (Input.GetMouseButton(MouseButtons.LEFT) && Input.GetKey(KeyCode.Z)) {
                Vector3 screenDiff = Input.mousePosition - startScreenPosition;
                float shift = Vector3.Dot(Vector3.right, screenDiff);
                transform.position = startCamPosition + transform.forward * shift * ZoomScrubSpeedAdjusted;
            }
        }
        
        private void HandleCameraRotation() {
            if (Input.GetMouseButtonDown(MouseButtons.LEFT)) {
                pitch = transform.eulerAngles.x;
                yaw = transform.eulerAngles.y;
            }

            if (Input.GetMouseButton(MouseButtons.LEFT) && Input.GetKey(KeyCode.LeftAlt)) {
                float yawDelta = RotationSpeedAdjusted * Input.GetAxis("Mouse X");
                float pitchDelta = -RotationSpeedAdjusted * Input.GetAxis("Mouse Y");

                Transform target = levelEditor.gridController.GetFirstSelectedTile() ?? spawnMenu.SelectedItem;
                if (target != null) {
                    transform.LookAt(target);
                    transform.RotateAround(target.position, transform.right, pitchDelta);
                    transform.RotateAround(target.position, transform.up, yawDelta);
                }
                else {
                    yaw += yawDelta;
                    pitch += pitchDelta;
                    transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
                }
            }
        }

        private void HandleClippingPlanes() {
            float xDistanceAway = (levelSize * 5f * Mathf.Sqrt(2f)) + CameraDistanceAway;
            mainCamera.farClipPlane = minFarClipPlain + xDistanceAway;
        }
        #endregion

    }
}

