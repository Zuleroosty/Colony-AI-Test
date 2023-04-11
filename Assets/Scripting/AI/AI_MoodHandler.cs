using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_MoodHandler : MonoBehaviour
{
    [Range(0, 5)] public int overallHappiness;

    [Header("Influences")]
    [Range(0f, 1f)] public float environment;
    [Range(0f, 1f)] public float tiredness;
    [Range(0f, 1f)] public float hunger;
    [Range(0f, 1f)] public float joy;

    [Header("Drop Rates")]
    [SerializeField] private float tiredBaseRate;
    [SerializeField] private float hungerBaseRate;
    [SerializeField] private float joyBaseRate;

    private AI_CivilianController thisCivCon;

    private void Start()
    {
        thisCivCon = GetComponent<AI_CivilianController>();
    }

    private void Update()
    {
        ManageHunger();
        ManageTiredness();
        overallHappiness = (int)(5 * ((environment - tiredness - hunger + joy) / 2));

        //if (joy > 0) joy -= joyBaseRate * Time.deltaTime;
    }

    private void ManageHunger()
    {
        if (thisCivCon.currentAction == AI_CivilianController.action.Rest)
        {
            if (thisCivCon.home.foodCount > 0 && hunger >= hungerBaseRate)
            {
                thisCivCon.home.foodCount--;
                hunger -= hungerBaseRate;
            }
        }
        else
        {
            if (hunger < 1) hunger += hungerBaseRate * Time.deltaTime;
            else if (hunger < 0) hunger = 0;
        }
    }

    private void ManageTiredness()
    {
        if (thisCivCon.currentAction == AI_CivilianController.action.Rest)
        {
            if (tiredness >= tiredBaseRate) tiredness -= tiredBaseRate;
        }
        else if (tiredness < 1) tiredness += tiredBaseRate * Time.deltaTime;
    }
}
