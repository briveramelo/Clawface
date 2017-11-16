using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighningChain : MonoBehaviour {

	[SerializeField] Material[] materials;

	private LineRenderer lineRender;
	public Transform origin;
    public Transform destination;
    private float distance;
	private float count;
	public float lightningTrailSpeed;

	private float randomWithOffsetMin = 1.0f;
	private float randomWithOffsetMax = 2.0f;

	private float materialSwitchInterval = 0.03f;

	private Vector3[] points = new Vector3[2];

	// Use this for initialization
	void Start () {
		lineRender = GetComponent<LineRenderer>();
        //StartCoroutine(Beam());
        //CalculateMiddle();
		points[0] = origin.position;
		points[1] = destination.position;

        lineRender.SetPositions(points);
        lineRender.startWidth = RandomWidthOffset();
        lineRender.endWidth = RandomWidthOffset();

        distance = Vector3.Distance(origin.position, destination.position);

		//StartCoroutine (SwitchLightningMaterial());
	}

	IEnumerator SwitchLightningMaterial ()
	{
		int materialIndex = 0;
		while (true)
		{
			materialIndex = 1 - materialIndex;

			lineRender.material = materials[materialIndex]; 

			yield return new WaitForSeconds(materialSwitchInterval);
		}
	}

	private float RandomWidthOffset()
    {
        return Random.Range(randomWithOffsetMin, randomWithOffsetMax);
    }

	void Update() {
		if(count < distance){
			count += 0.1f / lightningTrailSpeed;

			float x = Mathf.Lerp(0, distance , count);
			points[0] = origin.position;
			points[1] = destination.position;

			Vector3 pointAnloneLine = x * Vector3.Normalize(points[1] - points[0]) + points[0];

			lineRender.SetPosition(1, points[1]);
		}
	}
}
