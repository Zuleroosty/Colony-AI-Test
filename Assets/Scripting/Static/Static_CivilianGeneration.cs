using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Static_CivilianGeneration
{
    public static void PopulateCivilianInfo(AI_CivilianController civilian, SO_CilvilianInfo parent1, SO_CilvilianInfo parent2)
    {
        SO_CilvilianInfo newInfoSet = ScriptableObject.CreateInstance<SO_CilvilianInfo>();
        SetGender(newInfoSet);
        newInfoSet.civilianFirstName = GetFirstName(newInfoSet);
        newInfoSet.civilianFamilyName = GetFamilyName(parent1, parent2);
        civilian.thisCivInfo = newInfoSet;
    }

    private static string GetFirstName(SO_CilvilianInfo newInfoSet)
    {
        string fName = string.Empty;
        switch (newInfoSet.civilianGender)
        {
            case SO_CilvilianInfo.gender.Male:
                fName = mFirstName[Random.Range(0, mFirstName.Length)];
                break;
            case SO_CilvilianInfo.gender.Female:
                fName = fFirstName[Random.Range(0, fFirstName.Length)];
                break;
        }
        return fName;
    }
    private static string GetFamilyName(SO_CilvilianInfo parent1, SO_CilvilianInfo parent2)
    {
        string lName = string.Empty;
        if (parent1 && parent2)
        {
            int randNum = Random.Range(0, 2);
            switch (randNum)
            {
                case 0:
                    lName = parent1.civilianFamilyName;
                    break;
                case 1:
                    lName = parent2.civilianFamilyName;
                    break;
            }
        }
        else if (parent1) lName = parent1.civilianFamilyName;
        else if (parent2) lName = parent2.civilianFamilyName;
        else lName = familyName[Random.Range(0, familyName.Length)];
        return lName;
    }

    private static void SetGender(SO_CilvilianInfo newInfoSet)
    {
        int randNum = Random.Range(0, 101);
        if (randNum <= 50) newInfoSet.civilianGender = SO_CilvilianInfo.gender.Male;
        else newInfoSet.civilianGender = SO_CilvilianInfo.gender.Female;
    }

    private static string[] mFirstName =
    {
        "Liam",
        "Noah",
        "Oliver",
        "Elijah",
        "James",
        "William",
        "Benjamin",
        "Lucas",
        "Henry",
        "Theordore",
        "Jack",
        "Levi",
        "Alexander",
        "Jackson",
        "Mateo",
        "Daniel",
        "Michael",
        "Mason",
        "Sebastian",
        "Ethan",
        "Logan",
        "Owen",
        "Samuel",
        "Jacob",
        "Asher",
        "Aiden",
        "John",
        "Joeseph",
        "Wyatt",
        "David",
        "Leo",
        "Luke",
        "Julian",
        "Hudson",
        "Grayson",
        "Matthew",
        "Ezra",
        "Gabriel",
        "Carter",
        "Isaac",
        "Jayden",
        "Luca",
        "Anthony",
        "Dylan",
        "Lincoln",
        "Thomas",
        "Maverick",
        "Elias",
        "Josiah",
        "Charles"
    };

    private static string[] fFirstName =
    {
        "Olivia",
        "Emma",
        "Charlotte",
        "Amelia",
        "Ava",
        "Sophia",
        "Isabella",
        "Mia",
        "Evelyn",
        "Harper",
        "Luna",
        "Camila",
        "Gianna",
        "Elizabeth",
        "Eleanor",
        "Ella",
        "Abigail",
        "Sofia",
        "Avery",
        "Scarlett",
        "Emily",
        "Aria",
        "Penelope",
        "Chloe",
        "Layla",
        "Mila",
        "Nora",
        "Madison",
        "Ellie",
        "Lily",
        "Nova",
        "Isla",
        "Grace",
        "Violet",
        "Aurora",
        "Riley",
        "Zoey",
        "Willow",
        "Emilia",
        "Stella",
        "Zoe",
        "Victoria",
        "Hannah",
        "Addision",
        "Leah",
        "Lucy",
        "Eliana",
        "Ivy",
        "Everly"
    };
    private static string[] familyName =
    {
        "Smith",
        "Johnson",
        "Williams",
        "Brown",
        "Jones",
        "Garcia",
        "Miller",
        "Davis",
        "Rodriguez",
        "Martinez",
        "Hernandez",
        "Wilson",
        "Anderson",
        "Thomas",
        "Taylor",
        "Moore",
        "Jackson",
        "Martin",
        "Lee",
        "Perez",
        "Thompson",
        "White",
        "Harris",
        "Sanchez",
        "Clark",
        "Ramirez",
        "Lewis",
        "Robinson",
        "Walker",
        "Young",
        "Allen",
        "King",
        "Wright",
        "Scott",
        "Torres",
        "Hill",
        "Flores",
        "Green",
        "Adams",
        "Nelson",
        "Baker",
        "Hall",
        "Rivera",
        "Campbell",
        "Mitchell",
        "Carter",
        "Roberts"
    };
}
