using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageHandler : MonoBehaviour
{
    public int maxCapacityKg;
    public float storageUsed;
    public List<SO_ResourceData> inventory;

    private int timer, maxTime = 30;

    private void Start()
    {
        if (maxCapacityKg <= 0) maxCapacityKg = 8;
    }

    private void Update()
    {
        if (timer < maxTime) timer++;
        else
        {
            timer = 0;
            storageUsed = Static_FunctionsList.GetCurrentStorageUsed(inventory);
        }
    }

    public bool TransferResources(StorageHandler from, StorageHandler to)
    {
        bool transferComplete = false;
        if (from.inventory.Count > 0)
        {
            SO_ResourceData resource = from.inventory[0];
            if (Static_FunctionsList.GetCurrentStorageUsed(to.inventory) + resource.weightPerUnit < to.maxCapacityKg && Static_FunctionsList.GetCurrentStorageUsed(from.inventory) > 0)
            {
                to.inventory.Add(resource);
                from.inventory.Remove(resource);
                transferComplete = true;
                Debug.Log(from.name + " transfered to " + to.name);
            }
        }
        return transferComplete;
    }
}
