using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class HandTransform
{
    public Vector3 position;
    public Quaternion rotation;

    public HandTransform(InputDevice device)
    {
        device.TryGetFeatureValue(CommonUsages.devicePosition, out this.position);
        device.TryGetFeatureValue(CommonUsages.deviceRotation, out this.rotation);
    }
}

    public class HMDInputManager : MonoBehaviour
{
    public static Transform HeadTransform { get; private set; }

    public static InputDevice LeftHand { get; private set; }
    public static InputDevice RightHand { get; private set; }

    public static HandTransform LeftHandTransform { get; private set; }
    public static HandTransform RightHandTransform { get; private set; }

    void Start()
    {
        HeadTransform = GameObject.Find("HeadCamera").transform;

        LeftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        RightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        InputDevices.deviceConnected += InputDevices_deviceConnected;
        InputDevices.deviceDisconnected += InputDevices_deviceDisconnected;
    }

    private void InputDevices_deviceConnected(InputDevice device)

    {
        Debug.Log("DeviceConnected: " + device.name);
        LeftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        RightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    private void InputDevices_deviceDisconnected(InputDevice device)
    {
        Debug.Log("DeviceDisconnected: " + device.name);
    }

    void Update()
    {
        LeftHandTransform = new HandTransform(LeftHand);
        RightHandTransform = new HandTransform(RightHand);

        //GripButtonDown(LeftHand);
        LeftGripButtonDown();
        RightGripButtonDown();
        LeftTriggerButtonDown();
        RightTriggerButtonDown();
        LeftTouchPadClickDown();
        RightTouchPadClickDown();
        
    }

    public static Action LeftGetGripButtonDown;
    public static Action RightGetGripButtonDown;
    public static Action LeftGetTriggerButtonDown;
    public static Action RightGetTriggerButtonDown;
    public static Action LeftGetTouchPadClickDown;
    public static Action RightGetTouchPadClickDown;

    private static bool lastButtonState_LeftGrip = false;
    private static bool lastButtonState_RightGrip = false;
    private static bool lastButtonState_LeftTrigger = false;
    private static bool lastButtonState_RightTrigger = false;
    private static bool lastButtonState_LeftTouchPadClick = false;
    private static bool lastButtonState_RightTouchPadClick = false;


    private void LeftGripButtonDown()
    {
        LeftHand.TryGetFeatureValue(CommonUsages.gripButton, out bool tempState);

        if(tempState == true && tempState != lastButtonState_LeftGrip)
        {
            LeftGetGripButtonDown.Invoke();
        }

        lastButtonState_LeftGrip = tempState;
    }

    private void RightGripButtonDown()
    {
        RightHand.TryGetFeatureValue(CommonUsages.gripButton, out bool tempState);

        if (tempState == true && tempState != lastButtonState_RightGrip)
        {
            if(RightGetGripButtonDown != null)
                RightGetGripButtonDown();
        }

        lastButtonState_RightGrip = tempState;
    }

    private void LeftTriggerButtonDown()
    {
        LeftHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool tempState);
        if(tempState == true && tempState != lastButtonState_LeftTrigger)
        {
            if (LeftGetTriggerButtonDown != null)
                LeftGetTriggerButtonDown();
        }

        lastButtonState_LeftTrigger = tempState;
    }

    private void RightTriggerButtonDown()
    {
        RightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool tempState);

        if(tempState == true && tempState != lastButtonState_RightTrigger)
        {
            if (RightGetTriggerButtonDown != null)
                RightGetTriggerButtonDown();
        }

        lastButtonState_RightTrigger = tempState;
    }

    private void LeftTouchPadClickDown()
    {
        LeftHand.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool tempState);

        if (tempState == true && tempState != lastButtonState_LeftTouchPadClick)
        {
            if (LeftGetTouchPadClickDown != null)
                LeftGetTouchPadClickDown();
        }

        lastButtonState_LeftTouchPadClick = tempState;
    }


    private void RightTouchPadClickDown()
    {
        RightHand.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool tempState);
        
        if(tempState==true && tempState != lastButtonState_RightTouchPadClick)
        {
            if (RightGetTouchPadClickDown != null)
                RightGetTouchPadClickDown();
        }

        lastButtonState_RightTouchPadClick = tempState;
    }



    public void Get2DAxisInput_withoutNoise(InputDevice device, out Vector2 inputVec)
    {
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputVec);
        if (inputVec.magnitude < 0.2)
        {
            inputVec = Vector2.zero;
        }
    }

}
