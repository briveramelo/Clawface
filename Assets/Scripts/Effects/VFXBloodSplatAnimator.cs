using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
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

    private Material material;
    private Material MyMaterial {
        get {
            if (material==null) {
                Material randomMaterial = splatMaterials.GetRandom();
                meshRenderer.material = randomMaterial;
                material = randomMaterial;
            }
            return material;
        }
        set {
            meshRenderer.material = value;
        }
    }
    private float FrameLength { get { return (1f / framesPerSecond); } }
    private float StartMaskClipVal {
        get {
            if (startMaskClipVal==0f) {
                startMaskClipVal = MyMaterial.GetFloat("_Cutoff");
            }
            return startMaskClipVal;
        }
    }
    private float startMaskClipVal;


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

        MyMaterial.SetFloat("_Cutoff", StartMaskClipVal);
        MyMaterial.SetFloat("_Opacity", 1f);

        Timing.RunCoroutine(PlayAnimation(), coroutineName);
    }

    IEnumerator<float> PlayAnimation() {
        for (int i=0; i< totalFrames; i++) {
            MyMaterial.SetFloat("_fbcurrenttileindex6", i);
            yield return Timing.WaitForSeconds(FrameLength);
        }
        opacityAnim.Animate(coroutineName);
        clipAnim.Animate(coroutineName);
    }

    void DeActivate() {
        Timing.RunCoroutine(DelayAction(()=> gameObject.SetActive(false)), coroutineName);
    }

    void FadeOpacity(float val) {
        MyMaterial.SetFloat("_Opacity", val);
    }

    void FadeCutoff(float val) {
        MyMaterial.SetFloat("_Cutoff", val);        
    }
}
