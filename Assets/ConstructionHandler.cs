using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConstructionHandler : MonoBehaviour
{
    public CityHandler ownedCity;
    public SO_BuildingInfo buildingInfo;
    public bool resourcesCollected;

    [SerializeField] private float minY, maxY;
    [SerializeField] private Transform progress;
    [SerializeField] public List<SO_ResourceData> requiredResources;
    [SerializeField] public List<SO_ResourceData> queuedResources;
    [SerializeField] public List<SO_ResourceData> storedResources;

    private GlobalClockHandler globalClock;
    private float buildStartTime = 0;

    private void Start()
    {
        if (ownedCity == null) ownedCity = GameObject.Find("City").GetComponent<CityHandler>();
        transform.parent = ownedCity.constructionSiteParent;
        globalClock = GameObject.Find("GlobalGameManager").GetComponent<GlobalClockHandler>();
        foreach (var item in buildingInfo.requiredResources)
        {
            requiredResources.Add(item);
            queuedResources.Add(item);
        }
    }

    private void Update()
    {
        resourcesCollected = !isResourcesRequired();
        if (queuedResources.Count <= 0 && storedResources.Count == requiredResources.Count)
        {
            if (buildStartTime != 0)
            {
                float currentTime = (globalClock.hours * 60) + globalClock.minutes - buildStartTime;
                if (currentTime < buildingInfo.buildTimeMinutes) progress.transform.localPosition = new Vector3(progress.transform.localPosition.x, maxY - ((maxY - minY) * (currentTime / buildingInfo.buildTimeMinutes)), progress.transform.localPosition.z);
                else
                {
                    Instantiate(buildingInfo.buildingPrefab, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
            }
            else buildStartTime = (globalClock.hours * 60) + globalClock.minutes;
        }
        else progress.transform.localPosition = new Vector3(progress.transform.localPosition.x, maxY, progress.transform.localPosition.z);
    }

    public bool isResourcesRequired()
    {
        bool result = false;
        if (queuedResources.Count > 0) result = true;
        return result;
    }
}
