using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]

public class PLE_Camera : MonoBehaviour
{
    private Material lineMaterial;
    private Camera cam;
    public GameObject pivot;
    public GameObject[] vertices;

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

        if(Input.GetKey(KeyCode.UpArrow))
        {
            gameObject.transform.position += new Vector3(0, 0.5f, 0);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            gameObject.transform.position += new Vector3(0, -0.5f, 0);
        }
    }

    void OnPostRender()
    {
        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        lineMaterial.SetPass(0);
        {
            GL.Vertex(new Vector3(0, 0, 0));
            GL.Vertex(new Vector3(5, 3, 2));

            GL.Vertex(new Vector3(0, 0, 0));
            GL.Vertex(new Vector3(5, 0, 0));

            GL.Vertex(new Vector3(0, 0, 0));

            GL.Vertex(pivot.transform.position);
        }

        GL.End();
    }

    void OnApplicationQuit()
    {
        DestroyImmediate(lineMaterial);
    }

}