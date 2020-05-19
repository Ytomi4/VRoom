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
    private VRIK _vrik = default;

    private PlayerControl _playerControl;

    private bool _sitting = false;
    private bool _handsfree = false;
    
    private GameObject[] _handsfreeModeGadgets;

    [SerializeField]
    private GameObject _chair = default;
    private Vector3 _chairLocalPos;
    private Quaternion _chairLocalRot;
    private Vector3 _chairLocalScale;
    [SerializeField]
    private GameObject _chairParent= default;

    //[SerializeField]
    //private RuntimeAnimatorController _controller = default;

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

    private GameObject _leftHandIKTarget;
    #endregion


    void Start()
    {
        ImportVRMAsync.AvatarLoaded += GetAndSetupAvatar;

        RightGetGripButtonDown += SwitchStandingSitting;
        LeftGetGripButtonDown += SwitchHandsfreeTracking;

        _playerControl = transform.Find("Player").gameObject.GetComponent<PlayerControl>();

        _handsfreeModeGadgets = GameObject.FindGameObjectsWithTag("HandsFreeMode");

        _chairLocalPos = _chair.transform.localPosition;
        _chairLocalRot = _chair.transform.localRotation;
        _chairLocalScale = _chair.transform.localScale;

        _leftHandIKTarget = transform.Find("Player/LeftHand/IKTarget").gameObject;
        
        //SetupDefaultAvatar();
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

    private void SetupDefaultAvatar()
    {
        _avatar = transform.Find("DefaultAvatar").gameObject;
        AvatarSetup();
    }

    private void GetAndSetupAvatar()
    {
        if (ImportVRMAsync.Avatar != null)
        {
            _avatar = ImportVRMAsync.Avatar;
        }
        else
        {
            Debug.Log("cant find AvatarFile");
        }

        AvatarSetup();
    }

    private void AvatarSetup()
    {
        //Animator animator = _avatar.AddComponent<Animator>();
        //animator.runtimeAnimatorController = _controller;

        _vrik = _avatar.AddComponent<VRIK>();
        Debug.Log("AddComponent<VRIK>");
        
        _vrik.AutoDetectReferences();

        _vrik.GuessHandOrientations();
        Debug.Log("solver.leftArm.wristToPalmAxis= " + _vrik.solver.leftArm.wristToPalmAxis);
        Debug.Log("solver.leftArm.palmToThumbAxix= " + _vrik.solver.leftArm.palmToThumbAxis);

        Vector3 wristToPalm = _vrik.solver.leftArm.wristToPalmAxis;
        Quaternion leftWristRot = Quaternion.FromToRotation(wristToPalm, _vrik.references.leftHand.transform.rotation * Vector3.left);
        Debug.Log("wristToPalmRoted = " + leftWristRot * _vrik.solver.leftArm.wristToPalmAxis);
        _leftHandIKTarget.transform.rotation = leftWristRot * Quaternion.identity;

        Quaternion leftPalmRot = Quaternion.AngleAxis(180, _vrik.references.leftHand.transform.rotation * Vector3.left);

        //Vector3 palmToThumb = leftWristRot * _vrik.solver.leftArm.palmToThumbAxis;
        //Debug.Log("palmToThumb = " + palmToThumb);
        //Quaternion leftPalmRot = Quaternion.FromToRotation(palmToThumb, _vrik.references.leftHand.transform.rotation * Vector3.forward);

        _leftHandIKTarget.transform.rotation = leftPalmRot * _leftHandIKTarget.transform.rotation;

        _leftHandIKTarget.transform.position -= 0.1f * -1 * _vrik.references.leftHand.transform.right;

        _vrik.solver.leftArm.stretchCurve = new AnimationCurve();
        _vrik.solver.rightArm.stretchCurve = new AnimationCurve();

        _vrik.solver.spine.headTarget = HeadTransform;

        _vrik.solver.spine.pelvisTarget = _targetPelvis.transform;

        _vrik.solver.leftLeg.target = _targetLeftLeg.transform;
        _vrik.solver.leftLeg.bendGoal = _targetLeftLeg_BendGoal.transform;
        _vrik.solver.rightLeg.target = _targetRightLeg.transform;
        _vrik.solver.rightLeg.bendGoal = _targetRightLeg_BendGoal.transform;

        SittingMode();
        HandsfreeMode();

        _playerControl.Calibration(_playerControl.HeadCamera, _playerControl.CameraResetPos);

        StartCoroutine("Blink");
    }

    private void SwitchStandingSitting()
    {
        _sitting = !_sitting;
        if (_sitting)
        {
            SittingMode();
            _playerControl.Calibration(_playerControl.HeadCamera, _playerControl.CameraResetPos);
            _chair.transform.SetParent(transform);
            _chair.transform.localPosition = _chairLocalPos;
            _chair.transform.localRotation = _chairLocalRot;
            _chair.transform.localScale = _chairLocalScale;
            
        }
        else
        {
            StandingMode();
            _playerControl.Calibration(_playerControl.HeadCamera, _playerControl.CameraResetPosStanding);
            _chair.transform.SetParent(_chairParent.transform, true);
        }
    }

    private void SwitchHandsfreeTracking()
    {
        _handsfree = !_handsfree;
        if (_handsfree)
        {
            HandsfreeMode();
        }
        else
        {
            TrackingMode();
        }
    }

    private void SittingMode()
    {
        _vrik.solver.spine.pelvisPositionWeight = 1;
        _vrik.solver.spine.pelvisRotationWeight = 1;
        _vrik.solver.leftLeg.positionWeight = 1;
        _vrik.solver.leftLeg.rotationWeight = 1;
        _vrik.solver.leftLeg.bendGoalWeight = 0;
        _vrik.solver.rightLeg.positionWeight = 1;
        _vrik.solver.rightLeg.rotationWeight = 1;
        _vrik.solver.rightLeg.bendGoalWeight = 0;
    }

    private void StandingMode()
    {
        _vrik.solver.spine.pelvisPositionWeight = 0;
        _vrik.solver.spine.pelvisRotationWeight = 0;
        _vrik.solver.leftLeg.positionWeight = 0;
        _vrik.solver.leftLeg.rotationWeight = 0;
        _vrik.solver.leftLeg.bendGoalWeight = 0;
        _vrik.solver.rightLeg.positionWeight = 0;
        _vrik.solver.rightLeg.rotationWeight = 0;
        _vrik.solver.rightLeg.bendGoalWeight = 0;

        _vrik.solver.leftLeg.swivelOffset = 15;
        _vrik.solver.rightLeg.swivelOffset = 15;

        _vrik.solver.locomotion.footDistance = 0.15f;
        _vrik.solver.locomotion.stepThreshold = 0.4f;
        _vrik.solver.locomotion.maxVelocity = 0.3f;
        _vrik.solver.locomotion.velocityFactor = 0.3f;
        _vrik.solver.locomotion.rootSpeed = 30;
    }


    private void HandsfreeMode()
    {
        _vrik.solver.rightArm.target = _targetMouse.transform;
        _vrik.solver.leftArm.target = _targetKeyboard.transform;

        foreach(GameObject gadget in _handsfreeModeGadgets)
        {
            if (gadget.activeSelf == false)
                gadget.SetActive(true);
        }
    }

    private void TrackingMode()
    {
        _vrik.solver.rightArm.target = _targetRightHand.transform;
        _vrik.solver.leftArm.target = _targetLeftHand.transform;

        foreach (GameObject gadget in _handsfreeModeGadgets)
        {
            if (gadget.activeSelf == true)
                gadget.SetActive(false);
        }
    }


    IEnumerator Blink()
    {
        while(ImportVRMAsync.Avatar != null)
        {
            ImportVRMAsync.Avatar.GetComponent<Animator>().SetTrigger("Blink");
            float span = Random.Range(3f, 10f);
            yield return new WaitForSeconds(span);
        }
    }
    
}
