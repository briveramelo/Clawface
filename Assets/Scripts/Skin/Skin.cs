using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : MonoBehaviour
{

	public Color GlowColor;
    public float LerpFactor = .10f;

    private List<Material> materials = new List<Material>();
    private Color currentColor;
    private Color targetColor;

    bool flag = false;
    void DeSkin()
    {
        Debug.Log("DEGLOVED");
    }

    void Glow() {     
        StartCoroutine(TurnOnGlow());
    }

    private void Awake()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            materials.AddRange(renderer.materials);
        }
    
    }

    private void Update()
    {
        if (!flag && Input.GetKeyDown(KeyCode.A))
        {
            flag = true;
            StartCoroutine(TurnOnGlow());
        }
        
    }
    IEnumerator TurnOnGlow()
    {
        targetColor = GlowColor;

        while (!currentColor.Equals(targetColor)) {
            currentColor = Color.Lerp(currentColor, targetColor, LerpFactor);
            materials.ForEach(mat => { mat.SetColor("_GlowColor", currentColor); });
            yield return null;
        }
        materials.ForEach(mat => { mat.SetColor("_GlowColor", targetColor); });


    }
}
