using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class VFXAnimationPlayer : RoutineRunner {

    public bool loopAnimation;
    public bool playOnEnable=true;
    //public ParticleSystem ps;
    //public MeshRenderer meshRenderer;
    public int totalFrames;

    private Material MyMaterial {
        get {
            if (material==null) {
                //material = meshRenderer.material;
                material = GetComponent<Renderer>().material;
            }
            return material;
        }
    }
    private Material material;
    public int framesPerSecond;    
    private float FrameLength { get { return (1f / framesPerSecond); } }

    private void OnEnable()
    {
        if (playOnEnable) {
            Play();            
        }
    }

    public void Play()
    {
        Timing.RunCoroutine(PlayAnimation(), coroutineName);
    }

    IEnumerator<float> PlayAnimation() {
        for (int i=0; i< totalFrames; i++) {
            MyMaterial.SetFloat("_fbcurrenttileindex6", i);
            yield return Timing.WaitForSeconds(FrameLength);
        }        
        if (loopAnimation) {
            Play();
        }
    }    
}
