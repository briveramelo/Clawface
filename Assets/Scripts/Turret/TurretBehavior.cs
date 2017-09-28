using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehavior : MonoBehaviour {

    [SerializeField] private bool trackPlayer;
    [SerializeField] private GameObject cannon;
    [SerializeField] private GameObject bulletHellController;

    private GameObject attackTarget;
    private bool activateTurret = false;

    private void Update()
    {
        if (trackPlayer && attackTarget)
        {
            Vector3 lookAtPosition = new Vector3(attackTarget.transform.position.x, 1.5f, attackTarget.transform.position.z);
            cannon.transform.LookAt(lookAtPosition);
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Strings.Tags.PLAYER))
        {
            attackTarget = other.gameObject;
            bulletHellController.SetActive(true);
            activateTurret = true;

        }
    }
}
