using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CivilianUIHandler : MonoBehaviour
{
    public AI_CivilianController civilianToDisplay;

    [SerializeField] private GameObject infoBox;
    [SerializeField] private TMPro.TextMeshProUGUI firstName;
    [SerializeField] private TMPro.TextMeshProUGUI familyName;
    [SerializeField] private TMPro.TextMeshProUGUI actionUpdate;

    [SerializeField] private Image harvestProgress;
    [SerializeField] private Image storageProgress;

    [SerializeField] private Image environmentBar;
    [SerializeField] private Image tirednessBar;
    [SerializeField] private Image hungerBar;
    [SerializeField] private Image entertainmentBar;

    private void Update()
    {
        if (civilianToDisplay != null)
        {
            if (!infoBox.activeInHierarchy)
            {
                infoBox.SetActive(true);
                SO_CilvilianInfo civInfo = civilianToDisplay.thisCivInfo;
                firstName.text = civInfo.civilianFirstName;
                familyName.text = civInfo.civilianFamilyName + ", " + civInfo.civilianAge;
            }
            else
            {
                actionUpdate.text = civilianToDisplay.actionUpdateText;

                harvestProgress.transform.localRotation = Quaternion.Euler(0, 0, -360 * civilianToDisplay.harvestProgress);
                storageProgress.transform.localRotation = Quaternion.Euler(0, 0, -360 * civilianToDisplay.storeProgress);

                environmentBar.transform.localScale = new Vector3(civilianToDisplay.GetComponent<AI_MoodHandler>().environment, 1, 1);
                tirednessBar.transform.localScale = new Vector3(civilianToDisplay.GetComponent<AI_MoodHandler>().tiredness, 1, 1);
                hungerBar.transform.localScale = new Vector3(civilianToDisplay.GetComponent<AI_MoodHandler>().hunger, 1, 1);
                entertainmentBar.transform.localScale = new Vector3(civilianToDisplay.GetComponent<AI_MoodHandler>().joy, 1, 1);
            }
        }
        else infoBox.SetActive(false);
    }
}
