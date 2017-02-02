using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : MonoBehaviour, ISkinnable
{

	public Color GlowColor;
    public float LerpFactor = 10;

    private List<Material> _materials = new List<Material>();
    private Color _currentColor;
    private Color _targetColor;
    void ISkinnable.DeSkin()
    {
        Debug.Log("DEGLOVED");
    }

    void ISkinnable.Glow()
    {
        StartCoroutine(TurnOnGlow());
    }

    private void Awake()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            _materials.AddRange(renderer.materials);
        }
    
    }

    IEnumerator TurnOnGlow()
    {
        _targetColor = GlowColor;

        _currentColor = Color.Lerp(_currentColor, _targetColor, Time.deltaTime * LerpFactor);

        for (int i = 0; i < _materials.Count; i++)
        {
            _materials[i].SetColor("_GlowColor", _currentColor);
        }


        yield return null;
    }
}
