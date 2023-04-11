using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
    public AI_CivilianController harvestingCivilian;
    public SO_ResourceData resourceData;
    public int remainingResource;

    private void Start()
    {
        remainingResource = Random.Range(4, 8);
        gameObject.name = resourceData.name;
    }

    private void Update()
    {
        if (remainingResource <= 0) Destroy(gameObject);
    }

    public bool HarvestResources(StorageHandler targetStorage)
    {
        bool harvestComplete = false;
        if (Static_FunctionsList.GetCurrentStorageUsed(targetStorage.inventory) + resourceData.weightPerUnit < targetStorage.maxCapacityKg)
        {
            SO_ResourceData data = CreateNewResource();
            if (data != null)
            {
                targetStorage.inventory.Add(data);
                harvestComplete = true;
                Debug.Log(targetStorage.name + " harvested 1 resource from " + this.name);
                remainingResource--;
            }
            else Debug.Log("Error creating resource instance @ " + this.name);
        }
        return harvestComplete;
    }

    private SO_ResourceData CreateNewResource()
    {
        SO_ResourceData data = ScriptableObject.CreateInstance<SO_ResourceData>();
        data.resourceType = resourceData.resourceType;
        data.resourceID = resourceData.resourceID;
        data.weightPerUnit = resourceData.weightPerUnit;
        return data;
    }
}
