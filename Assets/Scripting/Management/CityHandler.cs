using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityHandler : MonoBehaviour
{
    public int maxPopulation;
    public List<AI_CivilianController> civilians;
    public List<AI_CivilianController> waitingForWork;

    public Transform homesParent;
    public Transform storageParent;
    public Transform workplaceParent;
    public Transform constructionSiteParent;
    public HashSet<StorageHandler> storageLocations;

    [SerializeField] private GameObject civilianPrefab;

    private int timer, maxTime = 30;

    private void Start()
    {
        UpdateTotalPopulationCapacity();
    }

    private void Update()
    {
        if (timer < maxTime) timer++;
        else
        {
            timer = 0;
            HashSet<StorageHandler> tempList = UpdateStorageList();
            if (storageLocations != tempList) storageLocations = tempList;

            if (civilians.Count < maxPopulation) PopulateCity();
        }
    }

    public void UpdateTotalPopulationCapacity()
    {
        int capacity = 0;
        for (int i = 0; i < homesParent.childCount; i++) capacity += homesParent.GetChild(i).gameObject.GetComponent<HomeHandler>().maxCapacity;
        maxPopulation = capacity;
    }

    private void PopulateCity()
    {
        HomeHandler home = null;
        for (int i = 0; i < homesParent.childCount; i++)
        {
            if (homesParent.GetChild(i).GetComponent<HomeHandler>().occupants.Count < homesParent.GetChild(i).GetComponent<HomeHandler>().maxCapacity)
            {
                home = homesParent.GetChild(i).GetComponent<HomeHandler>();
            }
        }
        if (home != null)
        {
            GameObject newCivilian = Instantiate(civilianPrefab, home.transform.position, Quaternion.identity);
            newCivilian.GetComponent<AI_CivilianController>().homeCity = this;
            newCivilian.transform.parent = home.transform;
            civilians.Add(newCivilian.GetComponent<AI_CivilianController>());
        }
    }

    private HashSet<StorageHandler> UpdateStorageList()
    {
        HashSet<StorageHandler> tempLocations = new HashSet<StorageHandler>();
        for (int i = 0; i < storageParent.childCount; i++)
        {
            tempLocations.Add(storageParent.GetChild(i).gameObject.GetComponent<StorageHandler>());
        }
        return tempLocations;
    }
}
