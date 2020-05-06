using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : HMDInputManager
{
    [SerializeField]
    private GameObject _leftHandInScene = default;
    [SerializeField]
    private GameObject _rightHandInScene = default;
    [SerializeField]
    private GameObject _headCamera = default;
    [SerializeField]
    private GameObject _cameraResetPos = default;


    
    void Start()
    {
        ImportVRM.AvatarLoaded += CalibrationIniciate;
        ImportVRMAsync.AvatarLoaded += CalibrationIniciate;
    }

    void Update()
    {
        IKTarget();

        if (Input.GetKeyDown(KeyCode.Space))
            Calibration(_headCamera, _cameraResetPos);
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
        float posY = transform.position.y;
        posY += resetPos.transform.position.y - head.transform.position.y;
        transform.position = new Vector3(transform.position.x, posY, transform.position.z);
        Debug.Log("Calibration has done");
    }

    private void CalibrationIniciate()
    {
        Calibration(_headCamera, _cameraResetPos);
    }

}
