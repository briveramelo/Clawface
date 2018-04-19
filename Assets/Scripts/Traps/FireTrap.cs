// FireTrap.cs
// Author: Aaron

using ModMan;

using System.Collections.Generic;
using System.Linq;

using Turing.VFX;

using UnityEngine;

namespace Turing.Gameplay
{
    /// <summary>
    /// Behavior to control fire traps.
    /// </summary>
    public sealed class FireTrap : MonoBehaviour
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// How long it takes for the trap to open (seconds).
        /// </summary>
        [Tooltip("How long it takes for the trap to open (seconds).")]
        [SerializeField] float openTime = 1f;

        /// <summary>
        /// How long the trap stays open (seconds).
        /// </summary>
        [Tooltip("How long the trap stays open (seconds).")]
        [SerializeField] float stayOpenTime = 3f;

        /// <summary>
        /// How long it takes for the trap to close (seconds).
        /// </summary>
        [Tooltip("How long it takes for the trap to close (seconds).")]
        [SerializeField] float closeTime = 1f;

        /// <summary>
        /// How long the trap stays closed (seconds).
        /// </summary>
        [Tooltip("How long the trap stays closed (seconds).")]
        [SerializeField] float stayClosedTime = 3f;

        /// <summary>
        /// How much damage to deal to objects in the trap per second.
        /// </summary>
        [Tooltip("How much damage to deal to objects in the trap per second.")]
        [SerializeField] float damagePerSecond = 10f;

        /// <summary>
        /// Type of trap functionality.
        /// </summary>
        [Tooltip("Type of trap functionality.")]
        [SerializeField] Mode mode;

        #endregion
        #region Private Fields

        const float DOOR_OPEN_DISTANCE = 0.25f;
        const float DOOR_Y = 0.2f;
        const float GRATE_LIFT_DISTANCE = 0.25f;
        const float GRATE_MIN_Y = -0.03f;
        const float GRATE_MAX_Y = 0.02f;

        /// <summary>
        /// Transforms for the various pieces of the trap.
        /// </summary>
        Transform door1, door2, grate;

        /// <summary>
        /// Timer for the applying damage.
        /// </summary>
        float damageTimer = 0f;


        float t;

        /// <summary>
        /// Timer for current state.
        /// </summary>
        float stateTimer;

        /// <summary>
        /// Current state.
        /// </summary>
        State currentState = State.Closed;

        /// <summary>
        /// Damager object attached to this fire trap.
        /// </summary>
        Damager damager = new Damager();

        /// <summary>
        /// Fire effect used on this trap.
        /// </summary>
        VFXOneOff fireEffect;

        #endregion
        #region Public Structures

        /// <summary>
        /// Enum for trap mode.
        /// </summary>
        public enum Mode
        {
            ContinuousStream,
            ContinuousOpenClose,
            PressureTrigger
        }

        #endregion
        #region Private Structures

        /// <summary>
        /// Enum for states.
        /// </summary>
        enum State
        {
            Closed,
            Open,
            Opening,
            Closing
        }

        #endregion
        #region Unity Lifecycle

        void Awake()
        {
            door1 = gameObject.FindInChildren("Door1").transform;
            door2 = gameObject.FindInChildren("Door2").transform;
            grate = gameObject.FindInChildren("Grate").transform;
            fireEffect = GetComponentInChildren<VFXOneOff>();
            damager.Set(damagePerSecond, DamagerType.FireTrap, Vector3.up);
            //_damageVolume = GetComponent<Collider>();

            if (mode == Mode.ContinuousStream) Open();
        }

        void Update()
        {
            var dt = Time.deltaTime;
            List<IDamageable> objectsInTrap = new List<IDamageable>();
            Physics.OverlapBox(transform.position, new Vector3(2.5f, 2.5f, 2.5f)).ToList().ForEach(collider =>
            {
                var damageable = collider.gameObject.GetComponent<IDamageable>();
                if (damageable.IsNull()) return;

                if (!objectsInTrap.Contains(damageable))
                {
                    objectsInTrap.Add(damageable);
                    if (mode == Mode.PressureTrigger && currentState == State.Closed) Open();
                }
            });

            if (objectsInTrap.Count == 0)
            {
                Close();
            }

            switch (currentState)
            {
                case State.Closing:
                    if (t >= dt) t -= dt;
                    else
                    {
                        t = 0f;
                        currentState = State.Closed;

                        if (mode == Mode.ContinuousOpenClose)
                            stateTimer = stayClosedTime;
                    }

                    DrawFrame(t / closeTime);
                    break;

                case State.Opening:
                    if (t <= openTime - dt) t += dt;
                    else
                    {
                        t = openTime;
                        currentState = State.Open;

                        if (fireEffect != null) fireEffect.Play();

                        if (mode == Mode.ContinuousOpenClose)
                            stateTimer = stayOpenTime;
                    }

                    DrawFrame(t / openTime);
                    break;

                case State.Open:
                    damageTimer -= dt;
                    if (damageTimer <= 0f)
                    {
                        DoDamage(objectsInTrap);
                        damageTimer += 1f;
                    }

                    if (mode == Mode.ContinuousOpenClose)
                    {
                        stateTimer -= dt;
                        if (stateTimer <= 0f) Close();
                    }
                    break;

                case State.Closed:
                    if (mode == Mode.ContinuousOpenClose)
                    {
                        stateTimer -= dt;
                        if (stateTimer <= 0f) Open();
                    }
                    break;
            }
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// Gets/sets the mode of this trap.
        /// </summary>
        public Mode TrapMode
        {
            get { return mode; }
            set { mode = value; }
        }

        /// <summary>
        /// Gets/sets the opening time of this trap.
        /// </summary>
        public float OpenTime
        {
            get { return openTime; }
            set { openTime = value; }
        }

        /// <summary>
        /// Gets/sets the time to stay open.
        /// </summary>
        public float StayOpenTime
        {
            get { return stayOpenTime; }
            set { stayOpenTime = value; }
        }

        /// <summary>
        /// Gets/sets the DPS of this trap.
        /// </summary>
        public float DamagePerSecond
        {
            get { return damagePerSecond; }
            set { damagePerSecond = value; }
        }

        /// <summary>
        /// Gets/sets the closing time of this trap.
        /// </summary>
        public float CloseTime
        {
            get { return closeTime; }
            set { closeTime = value; }
        }

        /// <summary>
        /// Gets/sets the time to stay closed.
        /// </summary>
        public float StayClosedTime
        {
            get { return stayClosedTime; }
            set { stayClosedTime = value; }
        }

        /// <summary>
        /// Opens the trap.
        /// </summary>
        public void Open()
        {
            if (currentState == State.Open) return;

            currentState = State.Opening;
        }

        /// <summary>
        /// Closes the trap.
        /// </summary>
        public void Close()
        {
            if (currentState == State.Closed) return;
            if (fireEffect != null) fireEffect.Stop();
            currentState = State.Closing;
        }

        #endregion
        #region Private Methods

        /// <summary>
        /// Draws a frame of the trap's coded animation.
        /// </summary>
        void DrawFrame(float t)
        {
            door1.localPosition = new Vector3(0f, DOOR_Y, t * DOOR_OPEN_DISTANCE);
            door2.localPosition = new Vector3(0f, DOOR_Y, -t * DOOR_OPEN_DISTANCE);

            var grateY = GRATE_MIN_Y + (GRATE_MAX_Y - GRATE_MIN_Y) * t;
            grate.localPosition = new Vector3(0f, grateY, 0f);
        }

        /// <summary>
        /// Deals damage to everything in the trap.
        /// </summary>
        /// <param name="_objectsInTrap"></param>
        void DoDamage(List<IDamageable> objectsInTrap)
        {
            foreach (var obj in objectsInTrap)
            {
                obj.TakeDamage(damager);
            }
        }

        #endregion
    }
}