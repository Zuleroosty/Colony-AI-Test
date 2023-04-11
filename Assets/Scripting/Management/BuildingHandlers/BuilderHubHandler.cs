using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderHubHandler : MonoBehaviour
{
    public CityHandler ownedCity;
    public int maxJobs;
    public List<AI_CivilianController> builders;

    [SerializeField] private int timer;
    [SerializeField] private int maxTime;

    private void Start()
    {
        if (ownedCity == null) ownedCity = GameObject.Find("City").GetComponent<CityHandler>();
        transform.parent = ownedCity.workplaceParent;
    }

    private void Update()
    {
        if (timer < maxTime) timer++;
        else
        {
            timer = 0;
            if (builders.Count < maxJobs && ownedCity.waitingForWork.Count > 0)
            {
                AI_CivilianController civilian = ownedCity.waitingForWork[0];
                if (!builders.Contains(civilian))
                {
                    builders.Add(civilian);
                    ownedCity.waitingForWork.Remove(civilian);
                    civilian.workplace = this.gameObject;
                }
            }

            if (ownedCity.constructionSiteParent.childCount > 0)
            {
                ConstructionHandler[] tempList = FindObjectsOfType<ConstructionHandler>();
                if (tempList.Length > 0)
                {
                    foreach (ConstructionHandler construction in tempList)
                    {
                        if (construction.ownedCity == ownedCity && builders.Count > 0)
                        {
                            foreach (var builder in builders)
                            {
                                if (builder.workplace != this) builders.Remove(builder);
                                else if (builder.currentAction == AI_CivilianController.action.Idle)
                                {
                                    RequestAvailableResources(builder, construction);
                                    builder.currentAction = AI_CivilianController.action.Collect;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void RequestAvailableResources(AI_CivilianController civilian, ConstructionHandler constructionSite)
    {
        StorageHandler civilianInventory = civilian.GetComponent<StorageHandler>();
        float requestWeight = 0;
        foreach (var item in constructionSite.queuedResources)
        {
            float inventoryWeight = Static_FunctionsList.GetCurrentStorageUsed(civilianInventory.inventory);
            if (inventoryWeight + requestWeight + item.weightPerUnit < civilianInventory.maxCapacityKg)
            {
                requestWeight += item.weightPerUnit;
                civilian.requestedResourceList.Add(item);
                constructionSite.queuedResources.Remove(item);
                civilian.workingSite = constructionSite;
            }
        }
    }
}
