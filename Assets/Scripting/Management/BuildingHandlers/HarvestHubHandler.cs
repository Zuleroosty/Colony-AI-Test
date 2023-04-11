using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static UnityEditor.ObjectChangeEventStream;

public class HarvestHubHandler : MonoBehaviour
{
    public CityHandler ownedCity;
    public int maxJobs;
    public List<AI_CivilianController> harvesters;

    public SO_ResourceData resourceToHarvest;
    public HashSet<ResourceHandler> resourcesInRange;
    [SerializeField] private float harvestRange;
    [SerializeField] private Transform visualRange;
    [SerializeField] private Transform rangeEdge;

    [SerializeField] private int timer;
    [SerializeField] private int maxTime;

    private void Start()
    {
        if (ownedCity == null) ownedCity = GameObject.Find("City").GetComponent<CityHandler>();
        transform.parent = ownedCity.workplaceParent;
    }

    private void Update()
    {
        if (!UpdateVisualRange())
        {
            if (timer < maxTime) timer++;
            else
            {
                timer = 0;
                if (harvesters.Count < maxJobs && ownedCity.waitingForWork.Count > 0)
                {
                    AI_CivilianController civilian = ownedCity.waitingForWork[0];
                    harvesters.Add(civilian);
                    civilian.workplace = this.gameObject;
                }
                foreach (var civilian in harvesters)
                {
                    if (civilian.workplace != this.gameObject) harvesters.Remove(civilian);
                }
                HashSet<ResourceHandler> tempResourceList = GetLocalResources();
                if (resourcesInRange != tempResourceList) resourcesInRange = tempResourceList;
            }
        }
    }

    private bool UpdateVisualRange()
    {
        float distance = Vector3.Distance(transform.position, rangeEdge.position);
        Vector3 thisScale = visualRange.transform.localScale;
        if (distance < harvestRange - 0.1f) visualRange.transform.localScale = new Vector3(thisScale.x * 1.01f, thisScale.y, thisScale.z * 1.01f);
        else if (distance > harvestRange + 0.1f) visualRange.transform.localScale = new Vector3(thisScale.x * 0.99f, thisScale.y, thisScale.z * 0.99f);
        else return false;
        return true;
    }

    private HashSet<ResourceHandler> GetLocalResources()
    {
        HashSet<ResourceHandler> resources = new HashSet<ResourceHandler>();
        for (int i = 0; i < GameObject.Find("GlobalResources").transform.childCount; i++)
        {
            ResourceHandler resource = GameObject.Find("GlobalResources").transform.GetChild(i).GetComponent<ResourceHandler>();
            if (resource.resourceData.resourceID == resourceToHarvest.resourceID)
            {
                float distance = Vector3.Distance(transform.position, resource.transform.position);
                if (distance < harvestRange) resources.Add(resource);
            }
        }
        return resources;
    }

    public ResourceHandler GetClosestResource(AI_CivilianController civilian)
    {
        if (resourcesInRange.Count > 0)
        {
            ResourceHandler closestResource = null;
            float closestDistance = float.MaxValue;
            foreach (var resource in resourcesInRange)
            {
                Vector3 civPos = civilian.transform.position;
                if (resource != null)
                {
                    Vector3 resPos = resource.transform.position;
                    civPos.y = 0;
                    resPos.y = 0;

                    float distance = Vector3.Distance(civPos, resPos);
                    if (distance < closestDistance && resource.harvestingCivilian == null)
                    {
                        closestResource = resource;
                        closestDistance = distance;
                    }
                }
            }
            if (closestResource != null && closestResource.harvestingCivilian == null) closestResource.harvestingCivilian = civilian;
            return closestResource;
        }
        else return null;
    }
}
