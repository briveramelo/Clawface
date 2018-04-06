using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using ModMan;
public class VFXBloodSplatAnimator : RoutineRunner {

    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] List<Material> splatMaterials;
    [SerializeField] bool loopAnimation;
    [SerializeField] bool playOnEnable=true;
    [SerializeField] int totalFrames;
    [SerializeField] int framesPerSecond;    
    [SerializeField] AbsAnim opacityAnim;
    [SerializeField] AbsAnim clipAnim;
    [SerializeField] private float startMaskClipVal;
    
    private Material MyMaterial {
        get {
            return meshRenderer.material;
        }
        set {
            meshRenderer.material = value;
        }
    }
    private float FrameLength { get { return (1f / framesPerSecond); } }


    private void OnEnable()
    {
        if (playOnEnable) {            
            Play();            
        }
    }

    public void Play()
    {
        MyMaterial = splatMaterials.GetRandom();
        opacityAnim.OnUpdate = FadeOpacity;
        clipAnim.OnUpdate = FadeCutoff;

        if (loopAnimation) {
            opacityAnim.onComplete = Play;
        }
        else {
            opacityAnim.onComplete = DeActivate;
        }        

        MyMaterial.SetFloat("_Cutoff", startMaskClipVal);
        MyMaterial.SetFloat("_Opacity", 1f);

        Timing.RunCoroutine(PlayAnimation(), CoroutineName);
    }

    IEnumerator<float> PlayAnimation() {
        for (int i=0; i< totalFrames; i++) {
            MyMaterial.SetFloat("_fbcurrenttileindex6", i);
            yield return Timing.WaitForSeconds(FrameLength);            
        }
        opacityAnim.Animate(CoroutineName);
        clipAnim.Animate(CoroutineName);
    }

    void DeActivate() {
        Timing.RunCoroutine(DelayAction(()=> gameObject.SetActive(false)), CoroutineName);
    }

    void FadeOpacity(float val) {
        MyMaterial.SetFloat("_Opacity", val);
    }

    void FadeCutoff(float val) {
        MyMaterial.SetFloat("_Cutoff", val);        
    }
}
