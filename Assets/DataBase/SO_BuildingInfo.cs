using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DB-B_", menuName = "DataBase/BuildingEntry")]
public class SO_BuildingInfo : ScriptableObject
{
    public string buildingName;
    public GameObject buildingPrefab;
    public List<SO_ResourceData> requiredResources;
    public int buildTimeMinutes;
}
