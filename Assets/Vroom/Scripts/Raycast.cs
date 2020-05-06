using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : HMDInputManager
{
    private GameObject _rayOrigin;
    private Vector3 _rayDir;
    private GameObject _rayCastLaser;
    private LineRenderer _laser;

    private string _rightIndexEndPath = "Root/Global/Position/J_Bip_C_Hips/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_R_Shoulder/J_Bip_R_UpperArm/J_Bip_R_LowerArm/J_Bip_R_Hand/J_Bip_R_Index1/J_Bip_R_Index2/J_Bip_R_Index3/J_Bip_R_Index3_end";

    [SerializeField]
    private float _rayDistance = default;
    [SerializeField]
    private LayerMask _layerMask = default;

    void Start()
    {
        _rayCastLaser = transform.Find("RayCastLaser").gameObject;
        _laser = _rayCastLaser.GetComponent<LineRenderer>();

        _rayOrigin = transform.Find("RayOrigin").gameObject;

        ImportVRMAsync.AvatarLoaded += RayOriginToCharactersHand;
    }

    void Update()
    {
        _rayDir = _rayOrigin.transform.rotation * Vector3.right;

        if(Physics.Raycast(_rayOrigin.transform.position, _rayDir, out RaycastHit hit, _rayDistance, _layerMask))
        {
            if (!_rayCastLaser.activeSelf)
            {
                _rayCastLaser.SetActive(true);
                switch (hit.transform.gameObject.name)
                {
                    case "Cube":
                        break;
                }
            }

            _laser.SetPosition(0, _rayOrigin.transform.position);
            _laser.SetPosition(1, hit.point);

        }else if(_rayCastLaser.activeSelf)
        {
            _rayCastLaser.SetActive(false);
        }
    }

    private void RayOriginToCharactersHand()
    {
        _rayOrigin = ImportVRMAsync._avatar.transform.Find(_rightIndexEndPath).gameObject;
    }
    
}
