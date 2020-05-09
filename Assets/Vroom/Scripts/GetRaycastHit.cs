using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRaycastHit : HMDInputManager
{
    public static RaycastHit Hit { get; private set; }

    private GameObject _rayOrigin;
    private Vector3 _rayDir;
    private Vector3 _rayDirOffset;
    private GameObject _rayCastLaser;
    private LineRenderer _laser;

    private string _rightIndexEndPath = "Root/Global/Position/J_Bip_C_Hips/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_R_Shoulder/J_Bip_R_UpperArm/J_Bip_R_LowerArm/J_Bip_R_Hand/J_Bip_R_Index1/J_Bip_R_Index2/J_Bip_R_Index3/J_Bip_R_Index3_end";

    private delegate void RayCastHitUpdate();
    private RayCastHitUpdate _rayCastHitUpdate;
    private delegate void RayCastHitButtonEvent();
    private RayCastHitButtonEvent _rayCastHitButtonEvent;

    [SerializeField]
    private float _rayDistance = default;
    [SerializeField]
    private LayerMask _layerMask = default;

    void Start()
    {
        _rayCastLaser = transform.Find("RayCastLaser").gameObject;
        _laser = _rayCastLaser.GetComponent<LineRenderer>();

        _rayOrigin = transform.Find("RayOriginDefault").gameObject;
        _rayDirOffset = Vector3.forward;

        ImportVRMAsync.AvatarLoaded += RayOriginToCharactersHand;
    }

    void Update()
    {
        _rayDir = _rayOrigin.transform.rotation * _rayDirOffset;

        if (Physics.Raycast(_rayOrigin.transform.position, _rayDir, out RaycastHit hit, _rayDistance, _layerMask))
        {
            if (!_rayCastLaser.activeSelf)
            {
                _rayCastLaser.SetActive(true);

                switch (hit.transform.gameObject.tag)
                {
                    case "Writable":
                        GameObject gameObject = hit.transform.Find("DrawingLine").gameObject;
                        var drawingLine = gameObject.GetComponent<DrawingLine>();
                        RightGetTriggerButtonDown += drawingLine.DrawingStart;
                        _rayCastHitUpdate += drawingLine.DrawingUpdate;
                        break;
                }
            }

            Hit = hit;

            _rayCastHitUpdate();

            _laser.SetPosition(0, _rayOrigin.transform.position);
            _laser.SetPosition(1, Hit.point);

        }else if(_rayCastLaser.activeSelf)
        {
            _rayCastLaser.SetActive(false);
            _rayCastHitUpdate = delegate () { };
            RightGetTriggerButtonDown = delegate () { };

            //case 円を重ねる（Instanciate）ことによる描線
            //drawingLine.ResetLastDraw();
        }
    }

    private void RayOriginToCharactersHand()
    {
        _rayOrigin = ImportVRMAsync.Avatar.transform.Find(_rightIndexEndPath).gameObject;
        _rayDirOffset = Vector3.right;
    }
    
}
