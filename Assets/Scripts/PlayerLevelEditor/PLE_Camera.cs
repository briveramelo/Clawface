using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]

public class PLE_Camera : MonoBehaviour
{
    private Material lineMaterial;

    private Camera cam;
    private Color lineColor = Color.red;

    public List<Vector3> vertices = new List<Vector3>();

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

        if (Input.GetKey(KeyCode.UpArrow))
        {
            gameObject.transform.position += new Vector3(0, 0.5f, 0);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            gameObject.transform.position -= new Vector3(0, 0.5f, 0);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            gameObject.transform.position += new Vector3(0.5f, 0, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            gameObject.transform.position -= new Vector3(0.5f, 0, 0);
        }
    }

    void OnPostRender()
    {
        GL.Begin(GL.LINES);
        GL.Color(lineColor);

        lineMaterial.SetPass(0);
        {
            for(int i = 0; i < vertices.Count; i++)
            {
                GL.Vertex(vertices[i]);
            }
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