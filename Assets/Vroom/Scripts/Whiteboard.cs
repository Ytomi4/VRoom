using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiteboard : HMDInputManager
{
    private DrawingLine _drawingLine;
    private bool _whiteboardOn = false;

    void Start()
    {
        this.gameObject.SetActive(_whiteboardOn);
        HMDInputManager.LeftGetTouchPadClickDown += CleanWhiteboard;
        HMDInputManager.RightGetTouchPadClickDown += WhiteboardSwitch;

        GameObject gameObject = transform.Find("DrawingLine").gameObject;
        _drawingLine = gameObject.GetComponent<DrawingLine>();
    }

    private void WhiteboardSwitch()
    {
        _whiteboardOn = !_whiteboardOn;
        this.gameObject.SetActive(_whiteboardOn);
    }

    private void CleanWhiteboard()
    {
        foreach (GameObject linesParent in _drawingLine.LinesParentsList)
        {
            Destroy(linesParent);
        }
    }

}
