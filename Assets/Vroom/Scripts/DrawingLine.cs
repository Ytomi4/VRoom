using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class DrawingLine : HMDInputManager
{
    private List<LineRenderer> _lineRendererList;

    [SerializeField]
    private GameObject _lineObject;
    [SerializeField]
    private Material _lineMaterial;
    [SerializeField]
    private Color _lineColor;
    [SerializeField]
    [Range(0, 0.05f)] private float _lineWidth;

    private void OnEnable()
    {
        _lineRendererList = new List<LineRenderer>();
    }

    public void DrawingStart()
    {
        AddLineRendererObject();
        //MakeBrushGroup();
    }

    public void DrawingUpdate()
    {
        AddPositionDataToLineRendererList();
        //if(RightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton) && triggerButton)
        //{
        //    InstanciateBrush();
        //}
        
    }

    //LineRendererをつかった描線

    private void AddLineRendererObject()
    {
        Debug.Log("AddLineObject");
        GameObject lineObject = GameObject.Instantiate(_lineObject, transform);
        _lineRendererList.Add(lineObject.GetComponent<LineRenderer>());

        _lineRendererList.Last().positionCount = 0;
    }

    private void AddPositionDataToLineRendererList()
    {
        if(RightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton) && triggerButton)
        {
            _lineRendererList.Last().positionCount += 1;
            _lineRendererList.Last().SetPosition(_lineRendererList.Last().positionCount - 1, GetRaycastHit.Hit.point + new Vector3(0, 0, 0.01f));

            Debug.Log("Draw at " + GetRaycastHit.Hit.transform.position);
        }
    }

    //円を重ねる（Instanciate）ことによる描線
    [SerializeField]
    private GameObject _brush;
    [SerializeField]
    private GameObject _whiteBoard;

    private float _spacing = 0.001f;
    private Vector3 _lastDraw = Vector3.zero;
    private int _lineNum = 0;
    private GameObject _lineGroup;

    private void MakeBrushGroup()
    {
        _lineNum++;
        _lineGroup = new GameObject("lineGroup" + _lineNum);
        _lineGroup.transform.parent = _whiteBoard.transform;
    }

    private void InstanciateBrush()
    {
        if(_lastDraw == Vector3.zero)
        {
            _lastDraw = GetRaycastHit.Hit.point;
            GameObject.Instantiate(_brush, GetRaycastHit.Hit.point + new Vector3(0,0,0.001f), _brush.transform.rotation, _lineGroup.transform);
        }
        else
        {
            Vector3 writeVector = GetRaycastHit.Hit.point - _lastDraw;
            Vector3 writeDir = Vector3.Normalize(writeVector);
            float writeMaxLength = writeVector.sqrMagnitude;

            for(float l = _spacing; l*l < writeMaxLength; l += _spacing)
            {
                GameObject.Instantiate(_brush, GetRaycastHit.Hit.point + new Vector3(0, 0, 0.001f), _brush.transform.rotation, _lineGroup.transform);
                _lastDraw += writeDir * _spacing;
            }
        }
    }

    public void ResetLastDraw()
    {
        _lastDraw = Vector3.zero;
    }
}
