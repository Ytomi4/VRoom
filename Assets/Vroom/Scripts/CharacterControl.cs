using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class CharacterControl : HMDInputManager
{
    [SerializeField]
    private float _posSpeed = 2f;
    [SerializeField]
    private float _rotSpeed = 100f;

    private GameObject _avatar;
    private VRIK _vrik;

    #region IK Targets
    [SerializeField]
    private GameObject _targetRightHand = default;
    [SerializeField]
    private GameObject _targetMouse = default;

    [SerializeField]
    private GameObject _targetLeftHand = default;
    [SerializeField]
    private GameObject _targetKeyboard = default;

    [SerializeField]
    private GameObject _targetPelvis = default;
    [SerializeField]
    private GameObject _targetLeftLeg = default;
    [SerializeField]
    private GameObject _targetLeftLeg_BendGoal = default;
    [SerializeField]
    private GameObject _targetRightLeg = default;
    [SerializeField]
    private GameObject _targetRightLeg_BendGoal = default;
    #endregion


    void Start()
    {
        ImportVRM.AvatarLoaded += AvatarSetup;
        ImportVRMAsync.AvatarLoaded += AvatarSetup;

        RightGetGripButtonDown += SwitchToHandsFreeMode;
        LeftGetGripButtonDown += SwitchToTrackingMode;        
    }

    void Update()
    {
        Vector2 moveDir;
        Vector2 moveRot;
        Get2DAxisInput_withoutNoise(LeftHand,out moveDir);
        Get2DAxisInput_withoutNoise(RightHand, out moveRot);
        transform.Rotate(new Vector3(0, moveRot.x, 0) * Time.deltaTime * _rotSpeed);

        transform.Translate(new Vector3(moveDir.x,0,moveDir.y) * Time.deltaTime * _posSpeed);
    }

    private void AvatarSetup()
    {
        if(ImportVRM._avatar != null)
        {
            _avatar = ImportVRM._avatar;
        }else if(ImportVRMAsync._avatar != null){
            _avatar = ImportVRMAsync._avatar;
        }else
        {
            Debug.Log("cant find AvatarFile");
        }

        _vrik = _avatar.AddComponent<VRIK>();
        
        _vrik.AutoDetectReferences();

        _vrik.solver.leftArm.stretchCurve = new AnimationCurve();
        _vrik.solver.rightArm.stretchCurve = new AnimationCurve();

        _vrik.solver.spine.headTarget = HeadTransform;
        _vrik.solver.spine.pelvisTarget = _targetPelvis.transform;

        LeftGetGripButtonDown();

        _vrik.solver.leftLeg.target = _targetLeftLeg.transform;
        _vrik.solver.leftLeg.bendGoal = _targetLeftLeg_BendGoal.transform;
        _vrik.solver.rightLeg.target = _targetRightLeg.transform;
        _vrik.solver.rightLeg.bendGoal = _targetRightLeg_BendGoal.transform;

        _vrik.solver.spine.pelvisPositionWeight = 1;
        _vrik.solver.spine.pelvisRotationWeight = 1;
        _vrik.solver.leftLeg.positionWeight = 1;
        _vrik.solver.leftLeg.rotationWeight = 1;
        _vrik.solver.leftLeg.bendGoalWeight = 1;
        _vrik.solver.rightLeg.positionWeight = 1;
        _vrik.solver.rightLeg.rotationWeight = 1;
        _vrik.solver.rightLeg.bendGoalWeight = 1;
        

        if (_vrik == null)
            Debug.Log("cant find VRIK component");

        
    }


    private void SwitchToHandsFreeMode()
    {
        Debug.Log("SwitchToHandsFreeMode");
        _vrik.solver.rightArm.target = _targetMouse.transform;
        _vrik.solver.leftArm.target = _targetKeyboard.transform;
    }

    private void SwitchToTrackingMode()
    {
        _vrik.solver.rightArm.target = _targetRightHand.transform;
        _vrik.solver.leftArm.target = _targetLeftHand.transform;
    }

    
}
