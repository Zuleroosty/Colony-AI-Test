using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeHandler : MonoBehaviour
{
    public int foodCount;
    public int maxCapacity;
    public HashSet<AI_CivilianController> occupants;

    public CityHandler ownedCity;

    private void Update()
    {
        ownedCity = transform.parent.parent.parent.GetComponent<CityHandler>();
        ownedCity.UpdateTotalPopulationCapacity();
        occupants = GetOccupants();
    }

    private HashSet<AI_CivilianController> GetOccupants()
    {
        HashSet<AI_CivilianController> tempList = new HashSet<AI_CivilianController>();
        for (int i = 0; i < transform.childCount; i++)
        {
            tempList.Add(transform.GetChild(i).GetComponent<AI_CivilianController>());
        }
        return tempList;
    }
}
