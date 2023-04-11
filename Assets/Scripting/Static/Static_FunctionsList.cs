using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public static class Static_FunctionsList
{
    public static float GetCurrentStorageUsed(List<SO_ResourceData> inventory)
    {
        float weight = 0;
        foreach (var resource in inventory) weight += resource.weightPerUnit;
        return weight;
    }

    public static StorageHandler GetClosestAvailableStorage(CityHandler cityCenter, AI_CivilianController civilian)
    {
        HashSet<StorageHandler> tempList = cityCenter.storageLocations;
        if (tempList.Count > 0)
        {
            StorageHandler closestStorage = null;
            float closestDistance = float.MaxValue;
            foreach (var location in tempList)
            {
                float distance = Vector3.Distance(civilian.transform.position, location.transform.position);
                if (distance < closestDistance && GetCurrentStorageUsed(location.inventory) < location.maxCapacityKg)
                {
                    closestDistance = distance;
                    closestStorage = location;
                }
            }
            return closestStorage;
        }
        else return null;
    }
}
