using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public int money;
    public Text resourceText;
    public GameObject soldier;
    public Transform bornPoint;
    public ArmyManager _am;
    public GameObject startUI;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        startUI.SetActive(false);

    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }


    public void AddSoldier()
    {
        if (money >= 100)
        {
            money -= 100;
            resourceText.text = "Resource: $ " + money;
            Instantiate(soldier, bornPoint);
            _am.UpdateArmy();

        } else
        {
            Debug.Log("Do not have enough money");
        }
    }


}
