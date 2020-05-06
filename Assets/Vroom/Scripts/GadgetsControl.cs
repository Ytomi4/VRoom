using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgetsControl : MonoBehaviour
{
    private GameObject[] handsFreeModeObjects;

    void Start()
    {
        handsFreeModeObjects = GameObject.FindGameObjectsWithTag("HandsFreeMode");

        Debug.Log(handsFreeModeObjects);

        HandsFreeModeObjectsOFF();

        HMDInputManager.RightGetGripButtonDown += HandsFreeModeObjectsON;
        HMDInputManager.LeftGetGripButtonDown += HandsFreeModeObjectsOFF;
    }

    private void HandsFreeModeObjectsON()
    {
        foreach(GameObject gadget in handsFreeModeObjects)
        {
            if(gadget.activeSelf == false)
                gadget.SetActive(true);
        }
    }

    private void HandsFreeModeObjectsOFF()
    {
        foreach(GameObject gadget in handsFreeModeObjects)
        {
            if (gadget.activeSelf == true)
                gadget.SetActive(false);
        }
    }
}
