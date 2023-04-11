using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DB-R_", menuName = "DataBase/ResourceEntry")]
public class SO_ResourceData : ScriptableObject
{
    public enum type { Wood, Stone, Crop }
    public type resourceType;
    public int resourceID;
    public float weightPerUnit;
}
