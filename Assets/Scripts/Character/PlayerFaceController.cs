// PlayerFaceController.cs
// Author: Aaron

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Turing.VFX
{
    /// <summary>
    /// Controller for player's facial expression.
    /// </summary>
    public class PlayerFaceController : EventSubscriber
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// Reference to the player's material instance.
        /// </summary>
        //[Tooltip("Reference to the player's material instance.")]
        //[SerializeField] Material playerMaterial;

        /// <summary>
        /// Reference to the player's mesh renderer.
        /// </summary>
        [Tooltip("Reference to the player's mesh renderer.")]
        [SerializeField] SkinnedMeshRenderer playerRenderer;

        [Header("Idle State Settings")]

        /// <summary>
        /// Texture to use for idle expression.
        /// </summary>
        [Tooltip("Texture to use for idle expression.")]
        [SerializeField] Texture2D idleExpression;

        /// <summary>
        /// Color to use for idle expression.
        /// </summary>
        [Tooltip("Color to use for idle expression.")]
        [SerializeField] Color idleColor;

        /// <summary>
        /// Frequency range of blinking (seconds).
        /// </summary>
        [Tooltip("Frequency range of blinking (seconds).")]
        [SerializeField] FloatRange blinkFrequency;

        /// <summary>
        /// Duration of each blink (seconds).
        /// </summary>
        [Tooltip("Duration of each blink (seconds).")]
        [SerializeField] float blinkDuration = 0.2f;

        /// <summary>
        /// Texture to use for idle blink expression.
        /// </summary>
        [Tooltip("Texture to use for idle blink expression.")]
        [SerializeField] Texture2D blinkExpression;

        [Header("Happy State Settings")]

        /// <summary>
        /// Texture to use for happy expression.
        /// </summary>
        [Tooltip("Texture to use for happy expression.")]
        [SerializeField] Texture2D happyExpression;

        /// <summary>
        /// Color to use for happy expression.
        /// </summary>
        [Tooltip("Color to use for happy expression.")]
        [SerializeField] Color happyColor;

        [Header("Angry State Settings")]

        /// <summary>
        /// Texture to use for angry expression.
        /// </summary>
        [Tooltip("Texture to use for angry expression.")]
        [SerializeField] Texture2D angryExpression;

        /// <summary>
        /// Color to use for angry expression.
        /// </summary>
        [Tooltip("Color to use for angry expression.")]
        [SerializeField] Color angryColor;

        #endregion

        #region Private Fields

        /// <summary>
        /// Instantiated material (so that template material isn't changed).
        /// </summary>
        //Material material;

        /// <summary>
        /// Current emotional state of the player.
        /// </summary>
        Emotion emotion = Emotion.Idle;
        bool isLevelComplete = false;
        #endregion

        #region Event Subscriptions
        protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
        protected override Dictionary<string, FunctionPrototype> EventSubscriptions {
            get {
                return new Dictionary<string, FunctionPrototype>() {
                    { Strings.Events.LEVEL_COMPLETED,      LevelComplete },
                    { Strings.Events.SCENE_LOADED, ResetToDefaultStates},
                    { Strings.Events.PLE_ON_LEVEL_READY, ResetToDefaultStates},
                };
            }
        }
        #endregion

        #region Unity Lifecycle
        protected override void Start ()
        {
            base.Start();
            if (gameObject.activeInHierarchy) {
                SetEmotion (emotion);
            }
        }
        #endregion

        #region Public Methods
        public void SetTemporaryEmotion(Emotion emotion, float time) {

            if (gameObject.activeInHierarchy) {
                SetEmotion(emotion);
                StartCoroutine(DoTemporaryEmotion(time));
            }
        }
        /// <summary>
        /// Sets the emotional state of the character.
        /// </summary>
        public void SetEmotion (Emotion emotion)
        {
            if (!isLevelComplete && gameObject.activeInHierarchy) {
                StopAllCoroutines();
                this.emotion = emotion;

                switch (emotion)
                {
                    case Emotion.Idle:
                        SetFacialExpression (idleExpression);
                        SetFacialColor (idleColor);
                        break;
                    case Emotion.Happy:
                        SetFacialExpression (happyExpression);
                        SetFacialColor (happyColor);
                        break;
                    case Emotion.Angry:
                        SetFacialExpression (angryExpression);
                        SetFacialColor (angryColor);
                        break;
                }

                StartCoroutine(FaceCoroutine());
            }
        }

        #endregion

        #region Private Methods
        private void ResetToDefaultStates(params object[] parameters) {
            isLevelComplete = false;
            SetEmotion(Emotion.Idle);
        }
        private void LevelComplete(params object[] parameters) {
            SetEmotion(Emotion.Happy);
            isLevelComplete = true;
        }
        /// <summary>
        /// Main facial expression coroutine.
        /// </summary>
        IEnumerator FaceCoroutine ()
        {
            while (true)
            {
                switch (emotion)
                {
                    case Emotion.Idle:
                        yield return new WaitForSeconds (blinkFrequency.GetRandomValue());
                        yield return Blink();
                        break;

                    case Emotion.Happy:
                        yield return null;
                        break;

                    case Emotion.Angry:
                        yield return null;
                        break;
                }
            }
        }

        /// <summary>
        /// Blinking coroutine.
        /// </summary>
        IEnumerator Blink ()
        {
            SetFacialExpression (blinkExpression);
            yield return new WaitForSeconds (blinkDuration);
            SetFacialExpression (idleExpression);
        }

        IEnumerator DoTemporaryEmotion (float time)
        {
            yield return new WaitForSeconds (time);
            SetEmotion (Emotion.Idle);
        }

        /// <summary>
        /// Sets the current facial expression of the character.
        /// </summary>
        void SetFacialExpression (Texture2D face)
        {
            playerRenderer.material.SetTexture ("_FaceTexture", face);
        }

        

        /// <summary>
        /// Sets the current facial color of the character.
        /// </summary>
        void SetFacialColor (Color color)
        {
            playerRenderer.material.SetColor ("_FaceColor", color);
        }

        #endregion

        #region Public Structures

        /// <summary>
        /// Enum for player emotional state.
        /// </summary>
        public enum Emotion
        {
            Idle,
            Happy,
            Angry
        }

        #endregion
    }
}