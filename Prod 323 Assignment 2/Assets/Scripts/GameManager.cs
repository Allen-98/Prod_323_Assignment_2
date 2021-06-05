using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public int money;
    public Text resourceText;
    public GameObject soldier;
    public Transform bornPoint;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void AddSoldier()
    {
        if (money > 100)
        {
            money -= 100;
            resourceText.text = "Resource: $ " + money;
            Instantiate(soldier, bornPoint);

        } else
        {
            Debug.Log("Do not have enough money");
        }
    }


}
