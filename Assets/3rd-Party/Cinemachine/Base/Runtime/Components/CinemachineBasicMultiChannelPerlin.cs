using UnityEngine;
using UnityEngine.Serialization;

namespace Cinemachine
{
    /// <summary>
    /// As a part of the Cinemachine Pipeline implementing the Noise stage, this
    /// component adds Perlin Noise to the Camera state, in the Correction
    /// channel of the CameraState.
    /// 
    /// The noise is created by using a predefined noise profile asset.  This defines the 
    /// shape of the noise over time.  You can scale this in amplitude or in time, to produce
    /// a large family of different noises using the same profile.
    /// </summary>
    /// <seealso cref="NoiseSettings"/>
    [DocumentationSorting(8, DocumentationSortingAttribute.Level.UserRef)]
    [AddComponentMenu("")] // Don't display in add component menu
    [RequireComponent(typeof(CinemachinePipeline))]
    [SaveDuringPlay]
    public class CinemachineBasicMultiChannelPerlin : MonoBehaviour, ICinemachineComponent
    {
        /// <summary>
        /// Serialized property for referencing a NoiseSettings asset
        /// </summary>
        [HideInInspector]
        [Tooltip("The asset containing the Noise Profile.  Define the frequencies and amplitudes there to make a characteristic noise profile.  Make your own or just use one of the many presets.")]
        [FormerlySerializedAs("m_Definition")]
        public NoiseSettings m_NoiseProfile;

        /// <summary>
        /// Gain to apply to the amplitudes defined in the settings asset.
        /// </summary>
        [Tooltip("Gain to apply to the amplitudes defined in the NoiseSettings asset.  1 is normal.  Setting this to 0 completely mutes the noise.")]
        public float m_AmplitudeGain = 1f;

        /// <summary>
        /// Scale factor to apply to the frequencies defined in the settings asset.
        /// </summary>
        [Tooltip("Scale factor to apply to the frequencies defined in the NoiseSettings asset.  1 is normal.  Larger magnitudes will make the noise shake more rapidly.")]
        public float m_FrequencyGain = 1f;

        /// <summary>True if the component is valid, i.e. it has a noise definition and is enabled.</summary>
        public bool IsValid
        { get { return enabled && m_NoiseProfile != null; } }

        /// <summary>Get the Cinemachine Virtual Camera affected by this component</summary>
        public ICinemachineCamera VirtualCamera
        { get { return gameObject.transform.parent.gameObject.GetComponent<ICinemachineCamera>(); } }

        /// <summary>Get the Cinemachine Pipeline stage that this component implements.
        /// Always returns the Noise stage</summary>
        public CinemachineCore.Stage Stage { get { return CinemachineCore.Stage.Noise; } }

        /// <summary>Applies noise to the Correction channel of the CameraState if the
        /// delta time is greater than 0.  Otherwise, does nothing.</summary>
        /// <param name="curState">The current camera state</param>
        /// <param name="deltaTime">How much to advance the perlin noise generator.
        /// Noise is only applied if this value is greater than 0</param>
        public void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            if (!IsValid || deltaTime <= 0)
                return;

            //UnityEngine.Profiling.Profiler.BeginSample("CinemachineBasicMultiChannelPerlin.MutateCameraState");
            if (!mInitialized)
                Initialize();

            ++mNoiseTime;
            float time = mNoiseTime * deltaTime * m_FrequencyGain;
            curState.PositionCorrection += curState.CorrectedOrientation * GetCombinedFilterResults(
                    m_NoiseProfile.PositionNoise, time, mNoiseOffsets) * m_AmplitudeGain;
            Quaternion rotNoise = Quaternion.Euler(GetCombinedFilterResults(
                        m_NoiseProfile.OrientationNoise, time, mNoiseOffsets) * m_AmplitudeGain);
            curState.OrientationCorrection = curState.OrientationCorrection * rotNoise;
            //UnityEngine.Profiling.Profiler.EndSample();
        }

        private bool mInitialized = false;
        private int mNoiseTime = 0;
        private Vector3 mNoiseOffsets = Vector3.zero;

        void Initialize()
        {
            mInitialized = true;
            mNoiseTime = 0;
            mNoiseOffsets = new Vector3(
                    UnityEngine.Random.Range(-Time.time, 10000f),
                    UnityEngine.Random.Range(-Time.time, 10000f),
                    UnityEngine.Random.Range(-Time.time, 10000f));
        }

        static Vector3 GetCombinedFilterResults(
            NoiseSettings.TransformNoiseParams[] noiseParams, float time, Vector3 noiseOffsets)
        {
            float xPos = 0f;
            float yPos = 0f;
            float zPos = 0f;
            if (noiseParams != null)
            {
                for (int i = 0; i < noiseParams.Length; ++i)
                {
                    NoiseSettings.TransformNoiseParams param = noiseParams[i];
                    Vector3 timeVal = new Vector3(
                            param.X.Frequency, param.Y.Frequency, param.Z.Frequency);
                    timeVal.Scale(time * Vector3.one + noiseOffsets);

                    Vector3 noise = new Vector3(
                            Mathf.PerlinNoise(timeVal.x, 0f),
                            Mathf.PerlinNoise(timeVal.y, 0f),
                            Mathf.PerlinNoise(timeVal.z, 0f));

                    noise -= Vector3.one * 0.5f;

                    xPos += noise.x * param.X.Amplitude;
                    yPos += noise.y * param.Y.Amplitude;
                    zPos += noise.z * param.Z.Amplitude;
                }
            }
            return new Vector3(xPos, yPos, zPos);
        }
    }
}