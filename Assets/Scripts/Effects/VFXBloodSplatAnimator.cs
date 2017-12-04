using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class VFXBloodSplatAnimator : RoutineRunner {

    public bool loopAnimation;
    public bool playOnEnable=true;
    public int totalFrames;

    private Material MyMaterial {
        get {
            if (material==null) {                
                material = GetComponent<Renderer>().material;
            }
            return material;
        }
    }
    private Material material;
    public int framesPerSecond;    
    private float FrameLength { get { return (1f / framesPerSecond); } }
    public AbsAnim opacityAnim;
    public AbsAnim clipAnim;
    float StartMaskClipVal {
        get {
            if (startMaskClipVal==0f) {
                startMaskClipVal = MyMaterial.GetFloat("_Cutoff");
            }
            return startMaskClipVal;
        }
    }
    float startMaskClipVal;


    private void OnEnable()
    {
        if (playOnEnable) {            
            Play();            
        }
    }

    public void Play()
    {
        opacityAnim.OnUpdate = FadeOpacity;
        clipAnim.OnUpdate = FadeCutoff;
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
        if (loopAnimation) {
            opacityAnim.onComplete = Play;
        }
        else {
            opacityAnim.onComplete = DeActivate;
        }
    }

    void DeActivate() {
        gameObject.SetActive(false);
    }

    void FadeOpacity(float val) {
        MyMaterial.SetFloat("_Opacity", val);
    }

    void FadeCutoff(float val) {
        MyMaterial.SetFloat("_Cutoff", val);        
    }
}
