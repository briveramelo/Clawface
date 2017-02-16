//Garin
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BloodSplatter : MonoBehaviour {
    
    ParticleSystem ps;
    List<ParticleCollisionEvent> coll = new List<ParticleCollisionEvent>();
    public static PaintBlood Instance;

    /// <summary>
    /// Paint decals to reproduce on textures
    /// </summary>
    public List<Transform> PaintDecalPrefabs;

    /// <summary>
    /// Parent to affect for scene management
    /// </summary>
    public Transform DecalsParent;

    /// <summary>
    /// Minimal scale of a prefab
    /// </summary>
    public float MinScale = 0.75f;

    /// <summary>
    /// Maximal scale of a prefab
    /// </summary>
    public float MaxScale = 3f;

    /// <summary>
    /// Range of the splash raycast
    /// </summary>
    public float SplashRange = 1.5f;

    /// <summary>
    /// Number of decals
    /// </summary>
    public int PoolSize = 300;

    private Transform[] paintDecals;
    private int currentPoolIndex;
    private List<Material> materials;


#if UNITY_EDITOR
    private bool mDrawDebug = true;
    private Vector3 mHitPoint;
    private List<Ray> mRaysDebug = new List<Ray>();
#endif

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateBloodGeyser();
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        //int safeLength = ps.GetSafeCollisionEventSize();
    
        int numCollisions = ps.GetCollisionEvents(other, coll);

        int i = 0;
        //print(numCollisions);
        Vector3 loc = coll[0].intersection;
        //print(loc);
        Paint(loc, Color.red, 1);
        //while (i < numCollisions)
        //{
        //    Vector3 loc = coll[i].intersection;
        //    Debug.Log(loc);
        //}

        print("done");
    }

    private void ActivateBloodGeyser()
    {
        ps.Play(false);
    }

    public void Paint(Vector3 location, Color color, int drops, float scaleBonus = 1f)
    {

#if UNITY_EDITOR
        mHitPoint = location;
        mRaysDebug.Clear();
        mDrawDebug = true;
#endif

        RaycastHit hit;

        // Generate multiple decals in once
        int n = 0;
        while (n < drops)
        {
            Vector3 dir = transform.TransformDirection(UnityEngine.Random.onUnitSphere * SplashRange);

            // Avoid raycast backward as we're in a 2D space
            if (dir.z < 0) dir.z = UnityEngine.Random.Range(0f, 1f);

            // Raycast around the position to splash everwhere we can
            if (Physics.Raycast(location, dir, out hit, SplashRange))
            {
                PaintDecal(hit, color, scaleBonus);

#if UNITY_EDITOR
                mRaysDebug.Add(new Ray(location, dir));
#endif

                n++;
            }
        }
    }

    private void PaintDecal(RaycastHit hit, Color color, float scaleBonus)
    {
        // Create a splash if we found a surface
        int randomIndex = UnityEngine.Random.Range(0, PaintDecalPrefabs.Count);
        Transform paintDecal = PaintDecalPrefabs[randomIndex];

        Vector3 modifiedHit = hit.point;

        modifiedHit.y += .5f;
        Transform paintSplatter = GameObject.Instantiate(paintDecal,
                                                   modifiedHit,
                                                   // Rotation from the original sprite to the normal
                                                   // Prefab are currently oriented to z+ so we use the opposite
                                                   Quaternion.FromToRotation(Vector3.back, hit.normal)
                                                   ) as Transform;

        // Find an existing material to enable batching
        //Material sharedMat = materials.Where(m => m.name.Equals(paintSplatter.GetComponent<Renderer>().material.name)
        //                                    && m.color.Equals(color)
        //                                ).FirstOrDefault();

        //// New material
        //if (sharedMat == null)
        //{
        //    //Material mat = paintSplatter.renderer.material;
        //    Material mat = paintSplatter.GetComponent<Renderer>().material;
        //    mat.color = color;

        //    materials.Add(mat);
        //}
        // Old one
        //else
        //{
        //    paintSplatter.GetComponent<Renderer>().sharedMaterial = sharedMat;
        //}

        // Random scale
        float scaler = UnityEngine.Random.Range(MinScale, MaxScale) * scaleBonus;

        paintSplatter.localScale = new Vector3(
            paintSplatter.localScale.x * scaler,
            paintSplatter.localScale.y * scaler,
            paintSplatter.localScale.z
        );

        // Random rotation effect
        //var rater = UnityEngine.Random.Range(0, 359);
        //paintSplatter.transform.RotateAround(hit.point, hit.normal, rater);

        paintSplatter.parent = DecalsParent;

        // Pool
        //if (paintDecals[currentPoolIndex] != null)
        //{
        //    Destroy(paintDecals[currentPoolIndex].gameObject);
        //    paintDecals[currentPoolIndex] = null;
        //}

        //paintDecals[currentPoolIndex] = paintSplatter;
        //currentPoolIndex++;

        //if (currentPoolIndex >= PoolSize) currentPoolIndex = 0;
        Destroy(paintSplatter.gameObject, 5);
    }


}
