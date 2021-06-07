using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDestroy : MonoBehaviour
{
    private float speed = 50f;
    private GameObject beamer;
    private TurretOperation _to;

    // Start is called before the first frame update
    void Start()
    {
        beamer = GameObject.FindGameObjectWithTag("beamer");
        _to = beamer.GetComponent<TurretOperation>();

        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = transform.position + transform.position * speed * Time.deltaTime;
    }
}
