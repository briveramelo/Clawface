using UnityEngine;

// NOTE: If you are getting errors of the sort that say something like:
//     "The type or namespace name `PostProcessing' does not exist in the namespace"
// it is because you are missing the PostProcessing v1 module.
//
// To make the errors go away, you can either:
//   1 - Download PostProcessing V1 and install it into your project, and take advantage of its features,
// or
//   2 - Delete or comment out this file.
//

namespace Cinemachine.PostFX
{
    /// <summary>
    /// This behaviour is a liaison between Cinemachine with the Post-Processing v1 module.  You must 
    /// have the Post-Processing V1 stack asset store package installed in order to use this behaviour.
    /// 
    /// It's used in 2 ways:
    ///
    /// * As a component on the Unity Camera: it serves as the liaison
    /// between the camera's CinemachineBrain and the camera's Post-Processing behaviour.
    /// It listens for camera Cut events and resets the Post-Processing stack when they occur.
    /// If you are using Post-Processing, then you should add this behaviour to your
    /// camera alongside the CinemachineBrain, always.
    ///
    /// * As a component on the Virtual Camera: In this capacity, it holds
    /// a Post-Processing Profile asset that will be applied to the Unity camera whenever 
    /// the Virtual camera is live.  It also has the (temporary) optional functionality of animating
    /// the Focus Distance and DepthOfField properties of the Camera State, and
    /// applying them to the current Post-Processing profile.
    /// </summary>
    [DocumentationSorting(100, DocumentationSortingAttribute.Level.UserRef)]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("Cinemachine/CinemachinePostFX")]
    [SaveDuringPlay]
    public class CinemachinePostFX : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        static void InitializeModule()
        {
            if (sModuleInitialized)
                return;

            // Add the static global PostFX hook
            CinemachineBrain.sPostProcessingHandler.AddListener((CinemachineBrain brain) => 
                {
                    CinemachinePostFX postFX = brain.PostProcessingComponent as CinemachinePostFX;
                    if (postFX == null)
                        brain.PostProcessingComponent = null;   // object deleted
                    else
                        postFX.PostFXHandler(brain);
                });

            // Add the per-brain hooks
            CinemachineBrain.sPostProcessingOnEnableHook.AddListener((CinemachineBrain brain) => 
                {
                    brain.PostProcessingComponent = brain.GetComponent<CinemachinePostFX>();
                });
            int numBrains = CinemachineCore.Instance.BrainCount;
            for (int i = 0; i < numBrains; ++i)
            {
                CinemachineBrain brain = CinemachineCore.Instance.GetActiveBrain(i);
                brain.PostProcessingComponent = brain.GetComponent<CinemachinePostFX>();
            }

            // Add the per-vcam hooks
            CinemachineVirtualCameraBase.sPostProcessingOnEnableHook.AddListener(
                (ICinemachineCamera vcam) => 
                {
                    CinemachineVirtualCameraBase vcamBase = vcam as CinemachineVirtualCameraBase;
                    if (vcamBase != null)
                        vcamBase.PostProcessingComponent = vcamBase.GetComponent<CinemachinePostFX>();
                });

            // If any cameras happened to be enabled before this init was called, take care of them now
            int numCameras = CinemachineCore.Instance.VirtualCameraCount;
            for (int i = 0; i < numCameras; ++i)
            {
                CinemachineVirtualCameraBase vcam 
                    = CinemachineCore.Instance.GetVirtualCamera(i) as CinemachineVirtualCameraBase;
                if (vcam != null)
                    vcam.PostProcessingComponent = vcam.GetComponent<CinemachinePostFX>();
            }
            sModuleInitialized = true;
        }
        static bool sModuleInitialized = false;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoad]
        class EditorInitialize { static EditorInitialize() { InitializeModule(); } }
#endif

        // Just for the Enabled checkbox
        void Update() {}

        [Tooltip("When this behaviour is on a Unity Camera, this setting is the default Post-Processing profile for the camera, and will be applied whenever it is not overridden by a virtual camera.  When the behaviour is on a virtual camera, then this is the Post-Processing profile that will become active whenever this virtual camera is live")]
        public UnityEngine.PostProcessing.PostProcessingProfile m_Profile;

        [Tooltip("If checked, then the Focus Distance will be set to the distance between the camera and the LookAt target.")]
        public bool m_FocusTracksTarget;

        [Tooltip("Offset from target distance, to be used with Focus Tracks Target.")]
        public float m_FocusOffset;

        // These are used if this behaviour is on a Unity Camera
        CinemachineBrain mBrain;
        UnityEngine.PostProcessing.PostProcessingBehaviour mPostProcessingBehaviour;

        void Awake()
        {
            // If I am a component on the Unity camera, connect to its brain
            // and to its post-processing behaviour
            mBrain = GetComponent<CinemachineBrain>();
            if (mBrain != null)
                mBrain.m_CameraCutEvent.AddListener(OnCameraCut);

            // Must have one of these if connected to a brain
            mPostProcessingBehaviour = GetComponent<UnityEngine.PostProcessing.PostProcessingBehaviour>();
            if (mPostProcessingBehaviour == null && mBrain != null)
                mPostProcessingBehaviour = gameObject.AddComponent<UnityEngine.PostProcessing.PostProcessingBehaviour>();
        }

        void OnDestroy()
        {
            if (mBrain != null)
                mBrain.m_CameraCutEvent.RemoveListener(OnCameraCut);
        }

        // CinemachineBrain callback used when this behaviour is on the Unity Camera
        void PostFXHandler(CinemachineBrain brain)
        {
            ICinemachineCamera vcam = brain.ActiveVirtualCamera;
            if (enabled && mBrain != null && mPostProcessingBehaviour != null)
            {
                // Look for the vcam's PostFX behaviour
                CinemachinePostFX postFX = GetEffectivePostFX(vcam);
                if (postFX == null)
                    postFX = this;  // vcam does not define a profile - apply the default
                if (postFX.m_Profile != null)
                {
                    // Adjust the focus distance
                    CameraState state = brain.CurrentCameraState;
                    UnityEngine.PostProcessing.DepthOfFieldModel.Settings dof
                        = postFX.m_Profile.depthOfField.settings;
                    if (postFX.m_FocusTracksTarget && state.HasLookAt)
                        dof.focusDistance = (state.FinalPosition - state.ReferenceLookAt).magnitude
                            + postFX.m_FocusOffset;
                    postFX.m_Profile.depthOfField.settings = dof;
                }
                // Apply the profile
                if (mPostProcessingBehaviour.profile != postFX.m_Profile)
                {
                    mPostProcessingBehaviour.profile = postFX.m_Profile;
                    mPostProcessingBehaviour.ResetTemporalEffects();
                }
            }
        }

        CinemachinePostFX GetEffectivePostFX(ICinemachineCamera vcam)
        {
            while (vcam != null && vcam.LiveChildOrSelf != vcam)
                vcam = vcam.LiveChildOrSelf;
            CinemachinePostFX postFX = null;
            while (vcam != null && postFX == null)
            {
                CinemachineVirtualCameraBase vcamBase = vcam as CinemachineVirtualCameraBase;
                if (vcamBase != null)
                    postFX = vcamBase.PostProcessingComponent as CinemachinePostFX;
                if (postFX != null && !postFX.enabled)
                    postFX = null;
                vcam = vcam.ParentCamera;
            }
            return postFX;
        }

        void OnCameraCut(CinemachineBrain brain)
        {
            //Debug.Log("CinemachinePostFX.OnCameraCut()");
            if (mPostProcessingBehaviour != null)
                mPostProcessingBehaviour.ResetTemporalEffects();
        }
    }
}
