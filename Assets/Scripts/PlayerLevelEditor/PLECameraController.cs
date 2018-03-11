using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ModMan;

namespace PlayerLevelEditor
{
    [RequireComponent(typeof(Camera))]
    public class PLECameraController : MonoBehaviour
    {
        [SerializeField] private PlayerLevelEditorGrid gridController;
        [SerializeField] private float minFarClipPlain=20f;
        [SerializeField] private float rotationSpeed = 4f * 60f;        
        [SerializeField] private float panSpeed = -0.15f * 60f;
        [SerializeField] private float WASDSpeedBase = 1f * 60f;
        [SerializeField] private float zoomScrollSpeed = 5f * 60f;
        [SerializeField] private float zoomScrubSpeed = 0.2f * 60f;
        [SerializeField] private float zPanSpeed = 0.5f * 60f;
        [SerializeField] private float heightOffsetSpeedMultiplier;

        private float CameraDistanceAway { get { return Vector3.Distance(mainCamera.transform.position, Vector3.zero); } }
        private float CameraYDistanceAway { get { return mainCamera.transform.position.y; } }
        private float CameraYDistanceMuliplier { get { return heightOffsetSpeedMultiplier * CameraYDistanceAway; } }
        private float WASDSpeedAdjusted { get { return (WASDSpeedBase + CameraYDistanceMuliplier) * Time.deltaTime; } }
        private float PanSpeedAdjusted { get { return (panSpeed - CameraYDistanceMuliplier); } }

        private float ZPanSpeedAdjusted { get { return (zPanSpeed /* use camera offset??? */ ) * Time.deltaTime; } }
        private float RotationSpeedAdjusted { get { return rotationSpeed * Time.deltaTime; } }
        private float ScrollSpeedAdjusted { get { return zoomScrollSpeed * Time.deltaTime; } }
        private float ZoomScrubSpeedAdjusted { get { return zoomScrubSpeed * Time.deltaTime; } }


        private Camera mainCamera;
        Vector3 startScreenPosition, startCamPosition;

        private float levelSize;//assumes levelSize is set and unchanged after the start funciton is called
        private float yaw = 0.0f;
        private float pitch = 0.0f;

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
        
        void Start()
        {
            levelSize = gridController.LevelSize;
            mainCamera = Camera.main;
        }

        void Update()
        {
            HandleCameraMovement();
            HandleCameraRotation();
            HandleCameraZooming();
            HandleClippingPlanes();
        }

        void OnApplicationQuit()
        {
            DestroyImmediate(lineMaterial);
        }

        


        
        void HandleCameraMovement() {
            HandleCameraWASD();
            HandleCameraPanning();
        }

        private void HandleCameraWASD() {
            if (Input.GetKey(KeyCode.W)) {
                transform.Translate(WASDSpeedAdjusted * Vector3.forward);
            }

            if (Input.GetKey(KeyCode.S)) {
                transform.Translate(WASDSpeedAdjusted * Vector3.back);
            }

            if (Input.GetKey(KeyCode.D)) {
                transform.Translate(WASDSpeedAdjusted * Vector3.right);
            }

            if (Input.GetKey(KeyCode.A)) {
                transform.Translate(WASDSpeedAdjusted * Vector3.left);
            }
        }

        void HandleCameraPanning() {
            if (Input.GetMouseButtonDown(MouseButtons.MIDDLE) || (Input.GetMouseButtonDown(MouseButtons.LEFT) && Input.GetKey(KeyCode.Space))) {
                startScreenPosition = Input.mousePosition;
                startCamPosition = mainCamera.transform.position;
            }

            if (Input.GetMouseButton(MouseButtons.MIDDLE) || (Input.GetMouseButton(MouseButtons.LEFT) && Input.GetKey(KeyCode.Space))) {
                Vector3 screenDiff = Input.mousePosition - startScreenPosition;
                float yShift = screenDiff.y;
                screenDiff.z = screenDiff.y;
                screenDiff.y = 0;
                screenDiff = mainCamera.transform.TransformDirection(screenDiff);
                screenDiff.y = 0;
                //what is this zPanSpeed??
                screenDiff += ZPanSpeedAdjusted * yShift * mainCamera.transform.forward.NormalizedNoY();
                mainCamera.transform.position = startCamPosition + screenDiff * PanSpeedAdjusted;
            }
        }

        void HandleCameraZooming() {            
            if (Input.GetAxis("Mouse ScrollWheel") < 0) { // back
                mainCamera.transform.Translate(Vector3.forward * ScrollSpeedAdjusted);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0) {// forward
                mainCamera.transform.Translate(Vector3.back * ScrollSpeedAdjusted);
            }

            if (Input.GetMouseButtonDown(MouseButtons.LEFT) && Input.GetKey(KeyCode.Z)) {
                startScreenPosition = Input.mousePosition;
                startCamPosition = mainCamera.transform.position;
            }
            if (Input.GetMouseButton(MouseButtons.LEFT) && Input.GetKey(KeyCode.Z)) {
                Vector3 screenDiff = Input.mousePosition - startScreenPosition;
                float shift = Vector3.Dot(Vector3.right, screenDiff);
                mainCamera.transform.position = startCamPosition + mainCamera.transform.forward * shift * ZoomScrubSpeedAdjusted;
            }
        }

        //TODO rotate around SelectedObject
        void HandleCameraRotation() {
            if (Input.GetMouseButtonDown(MouseButtons.LEFT)) {
                pitch = transform.eulerAngles.x;
                yaw = transform.eulerAngles.y;
            }
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(MouseButtons.LEFT)) {
                yaw += RotationSpeedAdjusted * Input.GetAxis("Mouse X");
                pitch -= RotationSpeedAdjusted * Input.GetAxis("Mouse Y");
                transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
            }
        }

        void HandleClippingPlanes() {
            float xDistanceAway = (levelSize * 5f * Mathf.Sqrt(2f)) + CameraDistanceAway;
            mainCamera.farClipPlane = minFarClipPlain + xDistanceAway;
        }
    }
}

