using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_CivilianController : MonoBehaviour
{
    public bool isHighlighted;
    public string actionUpdateText;
    public GameObject pointer;

    public HomeHandler home;
    public GameObject workplace;
    public CityHandler homeCity;
    public SO_CilvilianInfo thisCivInfo = null;
    public AI_MoodHandler moodHandler;
    public SO_CilvilianInfo parent1, parent2;

    [Header("AI"), SerializeField] private int updateTimer;
    [SerializeField] private int updateMaxTime;
    public enum action { Idle, Rest, Harvest, Store, Collect, Build }
    public action currentAction;

    // harvesting
    [Header("Harvest"), SerializeField] private float harvestRange;
    [SerializeField] private int harvestTimer, harvestMaxTime;
    [Range(0f, 1f)] public float harvestProgress;
    [SerializeField] private ResourceHandler targetResource;

    // storing
    [Header("Storage"), SerializeField] private float storeRange;
    [SerializeField] private int storeTimer, storeMaxTime;
    [Range(0f, 1f)] public float storeProgress;
    [SerializeField] private Transform targetBuilding;

    private GlobalClockHandler clockHandler;
    private NavMeshAgent nMA;

    // construction
    [Header("Construction"), SerializeField] public ConstructionHandler workingSite;
    [SerializeField] private StorageHandler storageToCollectFrom;
    [SerializeField] public List<SO_ResourceData> requestedResourceList;

    private int idleTimer, idleWaitTime = 120;


    private void Start()
    {
        nMA = GetComponent<NavMeshAgent>();
        clockHandler = GameObject.Find("GlobalGameManager").GetComponent<GlobalClockHandler>();
        if (home == null && transform.parent.name.Contains("House")) home = transform.parent.GetComponent<HomeHandler>();

        Static_CivilianGeneration.PopulateCivilianInfo(this, parent1, parent2);
    }

    private void Update()
    {

        if (updateTimer < updateMaxTime) updateTimer++;
        else
        {
            updateTimer = 0;
            if (isHighlighted)
            {
                pointer.SetActive(true);
                if (GameObject.Find("CivilianInfoDisplayBox").GetComponent<CivilianUIHandler>().civilianToDisplay != this) GameObject.Find("CivilianInfoDisplayBox").GetComponent<CivilianUIHandler>().civilianToDisplay = this;
            }
            else
            {
                pointer.SetActive(false);
                if (GameObject.Find("CivilianInfoDisplayBox").GetComponent<CivilianUIHandler>().civilianToDisplay == this) GameObject.Find("CivilianInfoDisplayBox").GetComponent<CivilianUIHandler>().civilianToDisplay = null;
            }
            switch (currentAction)
            {
                case action.Rest:
                    if (home != null && home != transform.parent) transform.parent = home.transform;
                    else transform.parent = homeCity.transform;
                    if (clockHandler.isDayTime) currentAction = action.Idle;
                    else
                    {
                        Vector3 targetPos;
                        if (home != null) targetPos = home.transform.position;
                        else targetPos = homeCity.transform.position;
                        targetPos.y = transform.position.y;
                        float distance = Vector3.Distance(transform.position, targetPos);
                        if (distance < 1)
                        {
                            actionUpdateText = "Sleeping";
                            nMA.ResetPath();
                        }
                        else if (nMA.destination != targetPos)
                        {
                            nMA.SetDestination(targetPos); 
                            actionUpdateText = "Going to rest";
                        }
                        
                    }
                    break;
                case action.Idle:
                    if (!clockHandler.isDayTime) currentAction = action.Rest;
                    else
                    {
                        actionUpdateText = "Watching the leaves";
                        harvestProgress = 0;
                        storeProgress = 0;
                        DecideNextAction(); 
                    }
                    break;
                case action.Harvest:
                    if (!clockHandler.isDayTime) currentAction = action.Rest;
                    else if (targetResource != null)
                    {
                        if (targetResource.harvestingCivilian == this)
                        {
                            harvestProgress = (float)harvestTimer / (float)harvestMaxTime;
                            Vector3 targetPos1 = targetResource.transform.position;
                            float distance1 = Vector3.Distance(targetPos1, transform.position);
                            if (distance1 < harvestRange)
                            {
                                nMA.ResetPath();
                                if (harvestTimer < harvestMaxTime) harvestTimer++;
                                else
                                {
                                    harvestTimer = 0;
                                    if (!targetResource.HarvestResources(GetComponent<StorageHandler>())) currentAction = action.Idle;
                                    else actionUpdateText = "Harvesting [" + targetResource.resourceData.resourceType + "]";
                                }
                            }
                            else
                            {
                                targetPos1.y = transform.position.y;
                                if (nMA.destination != targetPos1) nMA.SetDestination(targetPos1);
                                else actionUpdateText = "Walking to " + targetResource.resourceData.resourceType;
                            }
                        }
                        else
                        {
                            if (targetResource.harvestingCivilian == null) targetResource.harvestingCivilian = this;
                            else
                            {
                                float distance1 = Vector3.Distance(transform.position, targetResource.transform.position);
                                float distance2 = Vector3.Distance(targetResource.harvestingCivilian.transform.position, targetResource.transform.position);
                                if (distance1 < distance2) targetResource.harvestingCivilian = this;
                                else targetResource = null;
                            }
                        }
                    }
                    else currentAction = action.Idle;
                    break;
                case action.Store:
                    if (!clockHandler.isDayTime) currentAction = action.Rest;
                    else if (targetBuilding != null || Static_FunctionsList.GetCurrentStorageUsed(GetComponent<StorageHandler>().inventory) <= 0)
                    {
                        storeProgress = (float)storeTimer / (float)storeMaxTime;
                        Vector3 targetPos2 = targetBuilding.transform.position;
                        float distance2 = Vector3.Distance(targetPos2, transform.position);
                        if (distance2 < storeRange)
                        {
                            nMA.ResetPath();
                            if (storeTimer < storeMaxTime) storeTimer++;
                            else
                            {
                                storeTimer = 0;
                                if (!GetComponent<StorageHandler>().TransferResources(GetComponent<StorageHandler>(), targetBuilding.GetComponent<StorageHandler>())) currentAction = action.Idle;
                                else if (GetComponent<StorageHandler>().inventory.Count > 0) actionUpdateText = "Storing [" + GetComponent<StorageHandler>().inventory[0].resourceType + "]";
                            }
                        }
                        else
                        {
                            targetPos2.y = transform.position.y;
                            if (nMA.destination != targetPos2) nMA.SetDestination(targetPos2);
                            else actionUpdateText = "Walking to storage";
                        }
                    }
                    else currentAction = action.Idle;
                    break;
                case action.Collect:
                    if (!clockHandler.isDayTime) currentAction = action.Rest;
                    else
                    {
                        if (workingSite == null) currentAction = action.Idle;
                        else if (requestedResourceList.Count > 0)
                        {
                            if (storageToCollectFrom != null)
                            {
                                Vector3 targetPos3 = storageToCollectFrom.transform.position;
                                targetPos3.y = transform.position.y;

                                float distance = Vector3.Distance(transform.position, targetPos3);
                                if (distance < storeRange)
                                {
                                    float itemsTaken = CollectResources();
                                    if (itemsTaken <= 0) storageToCollectFrom = null;
                                }
                                else
                                {
                                    if (nMA.destination != targetPos3) nMA.SetDestination(targetPos3);
                                    else actionUpdateText = "Collecting [" + requestedResourceList[0].resourceType + "] from storage";
                                }
                            }
                            else storageToCollectFrom = GetClosestResourceInStorage();
                        }
                        else currentAction = action.Build;
                    }
                    break;
                case action.Build:
                    if (!clockHandler.isDayTime) currentAction = action.Rest;
                    else
                    {
                        if (workingSite != null)
                        {
                            Vector3 targetPos4 = workingSite.transform.position;
                            targetPos4.y = transform.position.y;
                            float distance = Vector3.Distance(transform.position, targetPos4);
                            if (distance < storeRange)
                            {
                                List<SO_ResourceData> neededResources = workingSite.buildingInfo.requiredResources;
                                foreach (var resource in workingSite.storedResources)
                                {
                                    bool itemFound = false;
                                    foreach (var item in neededResources)
                                    {
                                        if (item.resourceType == resource.resourceType && !itemFound)
                                        {
                                            neededResources.Remove(resource);
                                            itemFound = true;
                                        }
                                    }
                                }
                                if (neededResources.Count > 0)
                                {
                                    bool hasItemsBeenTransfered = false;
                                    foreach (var resource in neededResources)
                                    {
                                        bool itemTransfered = false;
                                        foreach (var item in GetComponent<StorageHandler>().inventory)
                                        {
                                            if (item.resourceType == resource.resourceType && !itemTransfered)
                                            {
                                                actionUpdateText = "Adding resources to [" + workingSite.buildingInfo.buildingName + "] construction site";
                                                workingSite.storedResources.Add(resource);
                                                GetComponent<StorageHandler>().inventory.Remove(item);
                                                neededResources.Remove(resource);
                                                hasItemsBeenTransfered = true;
                                                itemTransfered = true;
                                            }
                                        }
                                    }
                                    if (!hasItemsBeenTransfered) currentAction = action.Idle;
                                }
                                else currentAction = action.Idle;
                            }
                            else
                            {
                                if (nMA.destination != targetPos4) nMA.SetDestination(targetPos4);
                                else actionUpdateText = "Going to [" + workingSite.buildingInfo.buildingName + "] construction site";
                            }
                        }
                        else currentAction = action.Idle;
                    }
                    break;

            }
        }
    }

    private int CollectResources()
    {
        int itemsTaken = 0;
        foreach (var resource in requestedResourceList)
        {
            bool itemTaken = false;
            if (!itemTaken)
            {
                foreach (var item in storageToCollectFrom.inventory)
                {
                    if (item.resourceType == resource.resourceType)
                    {
                        GetComponent<StorageHandler>().inventory.Add(item);
                        storageToCollectFrom.inventory.Remove(item);
                        requestedResourceList.Remove(item);
                        itemsTaken++;
                        itemTaken = true;
                    }
                }
            }
        }
        return itemsTaken;
    }

    private StorageHandler GetClosestResourceInStorage()
    {
        SO_ResourceData resourceToCollect = requestedResourceList[0];
        float closestDistance = float.MaxValue;
        StorageHandler closestStorage = null;
        foreach (var storage in homeCity.storageLocations)
        {
            foreach (var item in storage.inventory)
            {
                if (item.resourceType == resourceToCollect.resourceType)
                {
                    float distance = Vector3.Distance(transform.position, storage.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestStorage = storage;
                    }
                }
            }
        }
        return closestStorage;
    }

    private void DecideNextAction()
    {
        if (!homeCity.waitingForWork.Contains(this)) homeCity.waitingForWork.Add(this);
        else if (workplace != null)
        {
            if (idleTimer < idleWaitTime) idleTimer++;
            else
            {
                idleTimer = 0;
                idleWaitTime = Random.Range(60, 121);
                GetNextWorkAction(); //work
            }
        }
        else
        {
            {
                //recreational?
            }
        }
    }

    private void GetNextWorkAction()
    {
        if (workplace.name.Contains("Harvest"))
        {
            float storageUsed = Static_FunctionsList.GetCurrentStorageUsed(GetComponent<StorageHandler>().inventory);
            ResourceHandler resource = workplace.GetComponent<HarvestHubHandler>().GetClosestResource(this);
            if (resource != null)
            {
                //work
                if (storageUsed + resource.resourceData.weightPerUnit < GetComponent<StorageHandler>().maxCapacityKg)
                {
                    // harvest
                    targetResource = resource;
                    currentAction = action.Harvest;
                }
                else
                {
                    // store
                    StorageHandler storage = Static_FunctionsList.GetClosestAvailableStorage(homeCity.GetComponent<CityHandler>(), this);
                    if (storage != null && storageUsed > 0)
                    {
                        targetBuilding = storage.transform;
                        currentAction = action.Store;
                    }
                    else currentAction = action.Idle;
                }
            }
            else
            {
                // store
                StorageHandler storage = Static_FunctionsList.GetClosestAvailableStorage(homeCity.GetComponent<CityHandler>(), this);
                if (storage != null && storageUsed > 0)
                {
                    targetBuilding = storage.transform;
                    currentAction = action.Store;
                }
                else currentAction = action.Idle;
            }
        }
        else if (workplace.name.Contains("Builder"))
        {
            // wait for request
        }
    }
}
