using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : HMDInputManager
{
    public GameObject leftHandCube;
    public GameObject rightHandCube;
    public GameObject headCamera;
    public GameObject cameraResetPos;


    
    void Start()
    {
        ImportVRM.AvatarLoaded += CalibrationIniciate;
    }

    void Update()
    {
        IKTarget();

        if (Input.GetKeyDown(KeyCode.Space))
            Calibration(headCamera, cameraResetPos);
    }

    private void IKTarget()
    {

        leftHandCube.transform.localPosition = LeftHandTransform.position;
        leftHandCube.transform.localRotation = LeftHandTransform.rotation;

        rightHandCube.transform.localPosition = RightHandTransform.position;
        rightHandCube.transform.localRotation = RightHandTransform.rotation;
    }

    public void Calibration(GameObject head, GameObject resetPos)
    {
        float posY = transform.position.y;
        posY += resetPos.transform.position.y - head.transform.position.y;
        transform.position = new Vector3(transform.position.x, posY, transform.position.z);
        Debug.Log("Calibration has done");
    }

    private void CalibrationIniciate()
    {
        Calibration(headCamera, cameraResetPos);
    }

}
