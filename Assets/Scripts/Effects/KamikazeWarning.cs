using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Turing.VFX
{

    public class KamikazeWarning : MonoBehaviour
    {
        [SerializeField] VFXOneOff vfx;
        ParticleSystem[] particleSystems;

        private void Awake()
        {
            if (particleSystems == null)
                particleSystems = GetComponentsInChildren<ParticleSystem>();

            transform.localScale = new Vector3(
                EnemyStatsManager.Instance.kamikazeStats.blastRadius,
                EnemyStatsManager.Instance.kamikazeStats.blastRadius,
                EnemyStatsManager.Instance.kamikazeStats.blastRadius
                    ) / 6.5f;

            bool wasPlaying = particleSystems[0].isPlaying;
            if (wasPlaying) vfx.Stop();

            foreach (ParticleSystem ps in particleSystems)
            {
                ParticleSystem.MainModule main = ps.main;
                //main.duration = EnemyStatsManager.Instance.kamikazeStats.selfDestructTime;
                main.simulationSpeed = 0.5f / EnemyStatsManager.Instance.kamikazeStats.selfDestructTime;
            }

            if (wasPlaying) vfx.Play();
        }
    }
}