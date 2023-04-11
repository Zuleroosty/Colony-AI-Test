using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DB-C_", menuName = "DataBase/CivilianEntry")]
public class SO_CilvilianInfo : ScriptableObject
{
    public string civilianFirstName;
    public string civilianFamilyName;
    public int civilianAge;
    public enum gender { Male, Female }
    public gender civilianGender;
    public Transform home;
}
