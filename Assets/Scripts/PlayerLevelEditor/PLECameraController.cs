using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ModMan;

namespace PlayerLevelEditor
{
    [RequireComponent(typeof(Camera))]
    public class PLECameraController : MonoBehaviour
    {

        [SerializeField] private float yawSpeed = 4f;
        [SerializeField] private float pitchSpeed = 4f;
        [SerializeField] private float panSpeed = 0.01f;
        [SerializeField] private float zPanSpeed = 0.01f;
        [SerializeField] private float zoomScrubSpeed = 0.01f;

        private Material lineMaterial;
        private Color lineColor = Color.red;
        private List<Vector3> vertices = new List<Vector3>();
        private Camera mainCamera;
        Vector3 startScreenPosition, startCamPosition;

        private float yaw = 0.0f;
        private float pitch = 0.0f;


        void Awake()
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
        }

        // Creates a simple two point line

        void Start()
        {
            mainCamera = Camera.main;
        }

        // Sets line endpoints to center of screen and mouse position

        void Update()
        {
            HandleCameraMovement();
            HandleCameraRotation();
            HandleCameraZooming();
        }

        //void OnPostRender()
        //{
        //    GL.Begin(GL.LINES);
        //    GL.Color(lineColor);

        //    lineMaterial.SetPass(0);

        //    for (int i = 0; i < vertices.Count; i++)
        //    {
        //        GL.Vertex(vertices[i]);
        //    }


        //    GL.End();

        //    vertices.Clear();
        //}

        void OnApplicationQuit()
        {
            DestroyImmediate(lineMaterial);
        }

        public void DrawLine(Vector3 p1, Vector3 p2)
        {
            if (p1 == null || p2 == null)
            {
                Debug.Log("p1 or p2 is null");
                return;
            }

            vertices.Add(p1);
            vertices.Add(p2);
        }

        public void SetLineColor(Color color)
        {
            lineColor = color;
        }


        
        void HandleCameraMovement() {
            const float moveSpeed = 0.5f;            
            if (Input.GetKey(KeyCode.W)) {
                transform.Translate(moveSpeed * Vector3.forward);
            }

            if (Input.GetKey(KeyCode.S)) {
                transform.Translate(moveSpeed * Vector3.back);
            }

            if (Input.GetKey(KeyCode.D)) {
                transform.Translate(moveSpeed * Vector3.right);
            }

            if (Input.GetKey(KeyCode.A)) {
                transform.Translate(moveSpeed * Vector3.left);
            }            

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
                screenDiff += zPanSpeed * yShift * mainCamera.transform.forward.NormalizedNoY();
                mainCamera.transform.position = startCamPosition + screenDiff * panSpeed;
            }

        }

        void HandleCameraZooming() {
            const float scrollSpeed = 5f;
            if (Input.GetAxis("Mouse ScrollWheel") < 0) { // back
                mainCamera.transform.Translate(Vector3.forward * scrollSpeed);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0) {// forward
                mainCamera.transform.Translate(Vector3.back * scrollSpeed);
            }

            if (Input.GetMouseButtonDown(MouseButtons.LEFT) && Input.GetKey(KeyCode.Z)) {
                startScreenPosition = Input.mousePosition;
                startCamPosition = mainCamera.transform.position;
            }
            if (Input.GetMouseButton(MouseButtons.LEFT) && Input.GetKey(KeyCode.Z)) {
                Vector3 screenDiff = Input.mousePosition - startScreenPosition;
                float shift = Vector3.Dot(Vector3.right, screenDiff);
                mainCamera.transform.position = startCamPosition + mainCamera.transform.forward * shift * zoomScrubSpeed;
            }
        }

        void HandleCameraRotation() {
            if (Input.GetMouseButtonDown(MouseButtons.LEFT)) {
                pitch = transform.eulerAngles.x;
                yaw = transform.eulerAngles.y;
            }
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(MouseButtons.LEFT)) {
                yaw += yawSpeed * Input.GetAxis("Mouse X");
                pitch -= pitchSpeed * Input.GetAxis("Mouse Y");
                transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
            }
        }

    }
}

