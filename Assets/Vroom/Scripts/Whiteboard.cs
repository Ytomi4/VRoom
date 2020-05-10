using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiteboard : HMDInputManager
{
    private DrawingLine _drawingLine;
    private int _boardNum = 0;

    private GameObject _board;

    void Start()
    {
        _board = transform.Find("Whiteboard").gameObject;

        HMDInputManager.LeftGetTouchPadClickDown += CleanWhiteboard;
        HMDInputManager.RightGetTouchPadClickDown += WhiteboardSwitch;

        GameObject gameObject = transform.Find("DrawingLine").gameObject;
        _drawingLine = gameObject.GetComponent<DrawingLine>();
    }

    private void WhiteboardSwitch()
    {
        if (_board != null)
            _board.SetActive(false);

        _boardNum++;
        if (_boardNum > 2)
            _boardNum = 0;

        switch (_boardNum)
        {
            case 0:
                _board = null;
                break;
            case 1:
                _board = transform.Find("Whiteboard").gameObject;
                _board.SetActive(true);
                break;
            case 2:
                _board = transform.Find("Window").gameObject;
                _board.SetActive(true);
                break;
        }
    }

    private void CleanWhiteboard()
    {
        foreach (GameObject linesParent in _drawingLine.LinesParentsList)
        {
            Destroy(linesParent);
        }
    }

}
