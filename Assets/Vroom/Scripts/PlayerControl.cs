using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : HMDInputManager
{
    [SerializeField]
    private GameObject _leftHandInScene = default;
    [SerializeField]
    private GameObject _rightHandInScene = default;
    
    public GameObject HeadCamera { get; private set; }
    public GameObject CameraResetPos { get; private set; }
    public GameObject CameraResetPosStanding { get; private set; }


    
    void Start()
    {
        HeadCamera = transform.Find("HeadCamera").gameObject;
        CameraResetPos = transform.parent.transform.Find("Target/HeadResetPos").gameObject;
        CameraResetPosStanding = transform.parent.transform.Find("Target/HeadResetPosStanding").gameObject;
    }

    void Update()
    {
        IKTarget();

        if (Input.GetKeyDown(KeyCode.Space))
            Calibration(HeadCamera, CameraResetPos);
    }

    private void IKTarget()
    {

        _leftHandInScene.transform.localPosition = LeftHandTransform.position;
        _leftHandInScene.transform.localRotation = LeftHandTransform.rotation;

        _rightHandInScene.transform.localPosition = RightHandTransform.position;
        _rightHandInScene.transform.localRotation = RightHandTransform.rotation;
    }

    public void Calibration(GameObject head, GameObject resetPos)
    {
        Quaternion rot = Quaternion.FromToRotation(head.transform.forward, resetPos.transform.forward);
        rot.x = 0;
        rot.z = 0;
        transform.rotation *= rot;

        Vector3 moveVec = resetPos.transform.position - head.transform.position;
        transform.position += moveVec;

        Debug.Log("Calibration has done");
    }
    

}
