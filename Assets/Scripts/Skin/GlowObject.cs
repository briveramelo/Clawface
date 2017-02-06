using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ModMan;

public class GlowObject : MonoBehaviour
{
	[SerializeField] Color glowColor;
	[SerializeField, Range(0.01f, 0.1f)] float lerpFactor;

	private List<Material> materials = new List<Material>();
	
	/// <summary>
	/// Cache a child materials so composite object work nicely!
	/// </summary>
	void Awake(){
		foreach (var renderer in GetComponentsInChildren<Renderer>()){
			materials.AddRange(renderer.materials);
		}
    }

    public void SetToGlow() {
        StartCoroutine(TurnOnGlow());
    }

    IEnumerator TurnOnGlow(){
        Color currentColor = materials[0].color;
        while (!currentColor.IsAboutEqual(glowColor)){
            currentColor = Color.Lerp(currentColor, glowColor, lerpFactor);
            materials.ForEach(mat => { mat.SetColor("_GlowColor", currentColor); });
            yield return null;
        }
        materials.ForEach(mat => { mat.SetColor("_GlowColor", glowColor); });
    }

}

