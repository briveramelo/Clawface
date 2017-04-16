/**
 *  @author Cornelia Schultz
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using MovementEffects;

public class InputManager : Singleton<InputManager> {

    #region Protected Constructor
        protected InputManager() { }
    #endregion

    #region Internal State
        private List<IController> controllers = new List<IController>();
        private List<IControlScheme> schemes = new List<IControlScheme>();
        private int activeScheme = -1;
    #endregion

    #region Unity Lifecycle Functions
        protected override void Awake()
        {
            base.Awake();

        //// Insert Appropriate Controllers:
        #if UNITY_STANDALONE
            controllers.Add(new KeyboardController());
        #endif

        #if UNITY_STANDALONE_WIN
            controllers.Add(new XBox360Controller());
        #elif UNITY_STANDALONE_OSX
            // controllers.Add(new OSXController());
        #elif UNITY_STANDALONE_LINUX
            // controllers.Add(new LinuxController());
        #endif

        //// Insert Schemes
        schemes.Add(new Scheme2());

        // Set Active Scheme
        activeScheme = schemes.Count > 0 ? 0 : -1;
    }

        private void Start()
        {
            Assert.AreNotEqual(schemes.Count, 0, "No Control Schemes Found For Platform!");
        }

        private void Update()
        {
            foreach (IController controller in controllers) {
                controller.Update();
            }
        }

        private void OnDisable() {
            controllers.ForEach(controller=> { controller.Vibrate(VibrationTargets.BOTH, 0f);});
        }

    #endregion

    #region Public Interface

    //// Schemes
    public string ActiveScheme
        {
            get
            {
                if (activeScheme == -1)
                    return "";
            return schemes[activeScheme].SchemeName;
            }
            set
            {
                IControlScheme scheme = schemes.Find((cmp) => { return cmp.SchemeName == value; });
                activeScheme = scheme != null ? schemes.IndexOf(scheme) : -1;
            }
        }

        public List<string> GetControlSchemes()
        {
            List<string> schemeNames = new List<string>();
            foreach (IControlScheme scheme in schemes)
            {
                schemeNames.Add(scheme.SchemeName);
            }
            return schemeNames;
        }

        //// Query Controls
        public Vector2 QueryAxes(string axes)
        {
            Assert.AreNotEqual(activeScheme, -1, "No Active Scheme Set!");
            return schemes[activeScheme].QueryAxes(controllers, axes);
        }

        public ButtonMode[] QueryAction(string action)
        {
            Assert.AreNotEqual(activeScheme, -1, "No Active Scheme Set!");
            return schemes[activeScheme].QueryAction(controllers, action);
        }
        public bool QueryAction(string action, ButtonMode mode)
        {
            Assert.AreNotEqual(activeScheme, -1, "No Active Scheme Set!");
            return schemes[activeScheme].QueryAction(controllers, action, mode);
        }

        //// Haptics
        public void Vibrate(VibrationTargets target, float intensity, float duration=0.3f)
        {
            duration=Mathf.Clamp(duration, 0.3f, 1f);
            controllers.ForEach(controller=> { controller.Vibrate(VibrationTargets.BOTH, 0f);});
            Timing.KillCoroutines("vibrate");
            Timing.RunCoroutine(VibrateForTime(target, intensity, duration), "vibrate");
        }

    #endregion

    #region
    private IEnumerator<float> VibrateForTime(VibrationTargets target, float intensity, float duration=0.2f) {
        float timeRemaining=duration;
        controllers.ForEach(controller=> { controller.Vibrate(target, intensity);});
        while(timeRemaining>0f) {
            timeRemaining-=Time.deltaTime;
            yield return 0f;
        }
        controllers.ForEach(controller=> { controller.Vibrate(target, 0f);});
    }
    #endregion

}
