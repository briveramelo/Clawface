using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class InputManager : Singleton<InputManager> {

    #region Protected Constructor
        protected InputManager() { }
    #endregion

    #region Internal State
        List<IController> controllers = new List<IController>();
        List<IControlScheme> schemes = new List<IControlScheme>();
        int activeScheme = -1;
    #endregion

    #region Unity Lifecycle Functions

        protected override void Awake()
        {
            base.Awake();

            // TODO: Insert Controllers and ControlSchemes into lists based on platforms.

            // Set active scheme to 0th
        }

        void Start()
        {
            Assert.AreNotEqual(schemes.Count, 0, "No Control Schemes Found For Platform!");
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
            return schemes[activeScheme].GetAxes(controllers, axes);
        }

        public bool QueryButton(string button, ButtonMode mode)
        {
            Assert.AreNotEqual(activeScheme, -1, "No Active Scheme Set!");
            return schemes[activeScheme].GetButton(controllers, button, mode);
        }

    #endregion
}
