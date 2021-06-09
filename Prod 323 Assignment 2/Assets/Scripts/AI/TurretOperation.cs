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

    [SerializeField] ArmyManager armyManager;

    private GameObject _army;
    private float timer = 0;
    private bool turretOut = false;

    private List<GameObject> armyList = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Army")
        {
            turretAnim.Play("Turret_v1_activation");
            _army = other.gameObject;
            turretOut = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Army")
        {
            turretAnim.Play("Turret_v1_deactivation");
        }
    }

    private void ChooseTarget()
    {
        _army = null; 

        for (int i = 0; i < armyList.Count; i++)
        {
            Health armyHealth = armyList[i].GetComponent<Health>();
            float currentHealth = armyHealth.GetHealth();
            if (currentHealth > 0)
            {
                _army = armyList[i];
                break;
            }
        }

    }

    public void UpdateArmyList()
    {
        armyList.Clear();
        _army = null;
        GameObject[] warriors = GameObject.FindGameObjectsWithTag("Army");

        foreach (GameObject warrior in warriors)
        {
            armyList.Add(warrior);
        }

        ChooseTarget();
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
        else
        {
            if (turretOut)
            {
                turretAnim.Play("Turret_v1_deactivation");
                turretOut = false;
            }
        }


    }

    void FaceArmy()
    {
        turret.transform.LookAt(_army.transform);
    }

    void Attack()
    {

        GameObject bullet = Instantiate(laserBeam, beamer_L.transform.position, beamer_L.transform.rotation) as GameObject;
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * speed;
        Destroy(bullet, 2f);

    }




}
