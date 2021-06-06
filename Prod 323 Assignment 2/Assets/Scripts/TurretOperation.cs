using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretOperation : MonoBehaviour
{
    [SerializeField] Animation turretAnim;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Army")
        {
            turretAnim.Play("Turret_v1_activation");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Army")
        {
            turretAnim.Play("Turret_v1_deactivation");
        }
    }
}
