using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralManager : HMDInputManager
{
    enum CharacterState
    {
        Standing,
        Sitting,
        HandsFree,
        FullAuto
    }

    private void Start()
    {
        LeftGetGripButtonDown += LeftGripButtonCheck;
        RightGetGripButtonDown += RightGripButtonCheck;
    }

    private void RightGripButtonCheck()
    {
        Debug.Log("RightGripButton has pressed");
    }

    private void LeftGripButtonCheck()
    {
        Debug.Log("LeftGripButton has pressed");
    }
}
