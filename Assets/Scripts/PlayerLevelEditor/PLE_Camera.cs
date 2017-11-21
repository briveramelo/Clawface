using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]

public class PLE_Camera : MonoBehaviour
{
    private Material lineMaterial;

    private Camera cam;
    private Color lineColor = Color.red;

    private List<Vector3> vertices = new List<Vector3>();


    public int cameraCurrentZoom = 8;
    public int cameraZoomMax = 20;
    public int cameraZoomMin = 5;

    public float speedH = 0.002f;
    public float speedV = 0.002f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;


    void Awake()
    {
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        lineMaterial = new Material(shader);
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

        cam = Camera.main;
    }

    // Creates a simple two point line

    void Start()
    {

    }

    // Sets line endpoints to center of screen and mouse position

    void Update()
    {
        float v = 0.5f;


        if (Input.GetKey(KeyCode.W))
        {
            gameObject.transform.Translate(v * Vector3.forward);
        }

        if (Input.GetKey(KeyCode.S))
        {
            gameObject.transform.Translate(v * Vector3.back);
        }

        if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.Translate(v * Vector3.right);
        }

        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.Translate(v * Vector3.left);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            if (cameraCurrentZoom < cameraZoomMax)
            {
                cameraCurrentZoom += 1;

                Camera.main.transform.Translate(Vector3.forward * 5);
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            if (cameraCurrentZoom > cameraZoomMin)
            {
                cameraCurrentZoom -= 1;
                Camera.main.transform.Translate(Vector3.back * 5);
            }
        }


        if (Input.GetMouseButtonDown(1))
        {
            pitch = transform.eulerAngles.x;
            yaw   = transform.eulerAngles.y;
        }
        if (Input.GetMouseButton(1))
        {
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

    }

    void OnPostRender()
    {
        GL.Begin(GL.LINES);
        GL.Color(lineColor);

        lineMaterial.SetPass(0);

        for (int i = 0; i < vertices.Count; i++)
        {
            GL.Vertex(vertices[i]);
        }


        GL.End();

        vertices.Clear();
    }

    void OnApplicationQuit()
    {
        DestroyImmediate(lineMaterial);
    }

    public void DrawLine(Vector3 p1, Vector3 p2)
    {
        if(p1 == null || p2 == null)
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
}