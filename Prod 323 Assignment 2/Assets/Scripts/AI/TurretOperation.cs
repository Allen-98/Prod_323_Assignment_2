using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretOperation : MonoBehaviour
{
    [SerializeField] Animation turretAnim;
    [SerializeField] Rigidbody turret;
    [SerializeField] GameObject laserBeam;
    [SerializeField] GameObject beamer_L;
    [SerializeField] GameObject beamer_R;
    [SerializeField] float speed;
    [Range(30f, 60f)]
    public float attackDist = 40f;
    [Range(0.1f, 5f)]
    public float fireInterval = 1f;

    private GameObject _army;
    private float timer = 0;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Army")
        {
            turretAnim.Play("Turret_v1_activation");
            _army = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Army")
        {
            turretAnim.Play("Turret_v1_deactivation");
        }
    }


    private void Update()
    {
        if (_army != null)
        {
            FaceArmy();

            if (timer > fireInterval)
            {
                Attack();
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }

        }


    }

    void FaceArmy()
    {
        turret.transform.LookAt(_army.transform);
    }

    void Attack()
    {
        //Vector3 direction = _army.transform.position - transform.position;

        //float distance = direction.magnitude;
        //if (distance < attackDist)
        //{
        //    Fire(direction.normalized);
        //    return;
        //}

        GameObject bullet = Instantiate(laserBeam, beamer_L.transform.position, beamer_L.transform.rotation) as GameObject;
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * speed;
        Destroy(bullet, 2f);

    }

    //void Fire(Vector3 v)
    //{
    //    var transform = this.transform;
    //    var laserR = Instantiate(laserBeam, beamer_L.transform.position, Quaternion.identity);
    //    Quaternion rot = Quaternion.identity;
    //    rot.eulerAngles = new Vector3(0, 0, 90);
    //    laserR.transform.rotation = transform.rotation * rot;
    //    var laserL = Instantiate(laserBeam, beamer_R.transform.position, Quaternion.identity);
    //    laserL.transform.rotation = transform.rotation * rot;

    //}

}
