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
        RightTriggerButtonDown();
    }

    public static Action LeftGetGripButtonDown;
    public static Action RightGetGripButtonDown;

    public static Action RightGetTriggerButtonDown;

    private static bool lastButtonState_Left = false;
    private static bool lastButtonState_Right = false;
    private static bool lastButtonState_Trigger = false;
    //private static HMD_GetKeyDown getKeyDown;

    private void LeftGripButtonDown()
    {
        LeftHand.TryGetFeatureValue(CommonUsages.gripButton, out bool tempState);

        if(tempState == true && tempState != lastButtonState_Left)
        {
            LeftGetGripButtonDown.Invoke();
        }

        lastButtonState_Left = tempState;
    }

    private void RightGripButtonDown()
    {
        RightHand.TryGetFeatureValue(CommonUsages.gripButton, out bool tempState);

        if (tempState == true && tempState != lastButtonState_Right)
        {
            if(RightGetGripButtonDown != null)
                RightGetGripButtonDown();
        }

        lastButtonState_Right = tempState;
    }

    private void RightTriggerButtonDown()
    {
        RightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool tempState);

        if(tempState == true && tempState != lastButtonState_Trigger)
        {
            if (RightGetTriggerButtonDown != null)
                RightGetTriggerButtonDown();
            Debug.Log("TriggerButton Pressed");
        }

        lastButtonState_Trigger = tempState;
    }


    public void Get2DAxisInput_withoutNoise(InputDevice device, out Vector2 inputVec)
    {
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputVec);
        if (inputVec.magnitude < 0.5)
        {
            inputVec = Vector2.zero;
        }
    }

}
