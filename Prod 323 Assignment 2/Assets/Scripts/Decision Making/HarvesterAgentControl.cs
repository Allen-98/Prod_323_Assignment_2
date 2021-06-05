using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using UnityEngine.AI;
using UnityEngine.UI;
using System;

public class HarvesterAgentControl : MonoBehaviour
{
    [SerializeField] GameObject allCrystals;
    [SerializeField] GameObject dropOffPoint;
    [SerializeField] GameObject repairPoint;

    [SerializeField] Slider healthBar;
    [SerializeField] Slider resourceBar;
    [SerializeField] Text resourceText;
    [SerializeField] Text alertText;

    [SerializeField] int maxResource;
    [SerializeField] int miningSpeed = 20;
    [SerializeField] float maxHealth = 100f;


    private StateMachine repairing_fsm;
    private StateMachine harvesting_fsm;
    private StateMachine safeLevel_fsm;

    private GameObject currentCrystal;
    private NavMeshAgent agent;
    private float elapsedTime = 0;
    private int resourceLevel = 0;
    private bool enRouteResource = false;
    private bool enRouteStation = false;
    private bool enRouteRepair = false;
    private string prevState;
    [SerializeField]private bool isSafe;
    [SerializeField] GameManager gm;




    // Start is called before the first frame update
    void Start()
    {
        resourceLevel = gm.money;
        resourceText.text = "Resource: $ " + resourceLevel;
        agent = gameObject.GetComponent<NavMeshAgent>();
        resourceBar.maxValue = maxResource;
        //healthBar.maxValue = maxHealth;
        //isSafe = true;

        harvesting_fsm = new StateMachine(this);

        // 1. Searching for resources
        harvesting_fsm.AddState("Searching", new State(onLogic: (state) => SearchingForResource()));

        // 2. Gathering resources
        harvesting_fsm.AddState("Gathering", new State(onLogic: (state) => GatheringResource()));

        // 3. Drop off resources
        harvesting_fsm.AddState("DroppingOff", new State(onLogic: (state) => DroppingOffResource()));

        // A. If we've found resource then gather
        harvesting_fsm.AddTransition(new Transition(
            "Searching",
            "Gathering",
            (transition) => currentCrystal != null || prevState == "Gathering"
            ));

        // B. If finished gathering and not at max capacity
        harvesting_fsm.AddTransition(new Transition(
            "Gathering",
            "Searching",
            (transition) => currentCrystal == null && resourceBar.value < maxResource
            ));

        // C. If finished gathering and at max capacity
        harvesting_fsm.AddTransition(new Transition(
            "Gathering",
            "DroppingOff",
            (transition) => (currentCrystal == null && resourceBar.value == maxResource) || prevState == "DroppingOff"
            ));

        // D. If finished dropping off, search for more resource
        harvesting_fsm.AddTransition(new Transition(
            "DroppingOff",
            "Searching",
            (transition) => resourceBar.value == 0
            ));

        //////////////////////////////////////////////////////////////////////
        repairing_fsm = new StateMachine(this);

        repairing_fsm.AddState("Repairing", new State(onLogic: (state) => RepairingHarvester()));

        repairing_fsm.AddState("Harvesting", harvesting_fsm);

        // E. Low health, do repair
        repairing_fsm.AddTransition(new Transition(
            "Harvesting",
            "Repairing",
            (transition) => healthBar.value < (maxHealth * 0.3)
            ));

        // F. Good health, do harvest
        repairing_fsm.AddTransition(new Transition(
            "Repairing",
            "Harvesting",
            (transition) => healthBar.value > (maxHealth * 0.8)
            ));



        /////////////////////////////////////////////
        safeLevel_fsm = new StateMachine(this);

        safeLevel_fsm.AddState("Safe", repairing_fsm);

        safeLevel_fsm.AddState("Red Alert", new State(onLogic: (state) => ReturnToStation()));

        // G. Red alert, return the station
        safeLevel_fsm.AddTransition(new Transition(
            "Safe",
            "Red Alert",
            (transition) => isSafe == false
            ));

        // H. Is safe, do harvest
        safeLevel_fsm.AddTransition(new Transition(
            "Red Alert",
            "Safe",
            (transition) => isSafe == true
            ));



        harvesting_fsm.SetStartState("Searching");
        //harvesting_fsm.OnEnter();


        repairing_fsm.SetStartState("Harvesting");
        //repairing_fsm.OnEnter();

        safeLevel_fsm.SetStartState("Safe");
        safeLevel_fsm.OnEnter();


    }


    // Update is called once per frame
    void Update()
    {
        //harvesting_fsm.OnLogic();

        //repairing_fsm.OnLogic();

        safeLevel_fsm.OnLogic();

        ReduceHealthOverTime();


        
    }


    private void SearchingForResource()
    {
        if (isSafe)
        {
            alertText.text = "Alert Level: " + "Safe";
        }

        currentCrystal = getNearestCrystal();
    }


    private void GatheringResource()
    {
        if (!enRouteResource)
        {
            agent.SetDestination(currentCrystal.transform.position);
            enRouteResource = true;
        }

        if (CheckResourceInRange())
        {
            if (elapsedTime >= 1f)
            {
                // do gather
                currentCrystal.GetComponent<CrystalMine>().MineCrystal(miningSpeed);
                resourceBar.value += miningSpeed;
                elapsedTime = 0;
                enRouteResource = false;

            }
            else
            {
                elapsedTime += Time.deltaTime;
            }
        }


    }

    private void DroppingOffResource()
    {
        if (!enRouteStation)
        {
            agent.SetDestination(dropOffPoint.transform.position);
            enRouteStation = true;
        }


        if (CheckStationIsReached())
        {
            gm.money += (int)resourceBar.value;
            resourceLevel = gm.money;
            resourceBar.value = 0;
            resourceText.text = "Resource: $ " + resourceLevel;
            enRouteStation = false;
            
        }


    }


    private GameObject getNearestCrystal()
    {
        GameObject nearestCrystal = null;
        float nearestDist = 9999f;
        
        foreach(Transform child in allCrystals.transform)
        {
            float dist = Vector3.Distance(child.position, transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearestCrystal = child.gameObject;
            }
        }

        return nearestCrystal;
    }

    private bool CheckResourceInRange()
    {
        return Vector3.Distance(currentCrystal.transform.position, transform.position) <= agent.stoppingDistance;
    }

    private bool CheckStationIsReached()
    {
        return Vector3.Distance(dropOffPoint.transform.position, transform.position) <= agent.stoppingDistance;
    }

    private bool CheckRepairIsReached()
    {
        return Vector3.Distance(repairPoint.transform.position, transform.position) <= agent.stoppingDistance;
    }



    private void RepairingHarvester()
    {
        if (isSafe)
        {
            alertText.text = "Alert Level: " + "Safe";
        }

        if (!enRouteRepair)
        {
            agent.SetDestination(repairPoint.transform.position);
            prevState = harvesting_fsm.activeState.name;
            enRouteRepair = true;
            enRouteResource = false;
            enRouteStation = false;
        }


        if (CheckRepairIsReached())
        {
            healthBar.value = maxHealth;
            enRouteRepair = false;

        }
    }


    private void ReduceHealthOverTime()
    {
        healthBar.value -= Time.deltaTime;
    }


    private void ReturnToStation()
    {
        if (!isSafe)
        {
            alertText.text = "Alert Level: " + "Red Alert";
        }

        if (!enRouteStation)
        {
            agent.SetDestination(dropOffPoint.transform.position);
            enRouteStation = true;
        }

        if (CheckStationIsReached())
        {
            enRouteStation = false;
        }


    }



}
