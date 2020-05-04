using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTarget : MonoBehaviour
{
    public GameObject leftBottom;
    public GameObject rightBottom;
    public GameObject leftUp;
    public GameObject mousePoint;

    public float handFloatPos = 0.05f;

    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x / Screen.currentResolution.width, Input.mousePosition.y / Screen.currentResolution.height);
        Vector2 offset = new Vector2(0, 0);
        mousePos -= offset;

        if (Mathf.Abs(mousePos.x) > 0.5f || Mathf.Abs(mousePos.y) > 0.5f)
            offset = mousePos;

        //基本的にははみ出したら中央に戻るような実装を目指した
        //上手く回っていないとりあえずの修正
        //mousePos += new Vector2(0,0.5f);

        Vector3 monitorUp = leftUp.transform.position - leftBottom.transform.position;
        Vector3 monitorRight = rightBottom.transform.position - leftBottom.transform.position;

        Vector3 normal = Vector3.Normalize(Vector3.Cross(monitorUp, monitorRight));

        Vector3 mousePointPos = leftBottom.transform.position +
            monitorUp * mousePos.y + monitorRight * mousePos.x + normal * handFloatPos;

        mousePoint.transform.position = mousePointPos;

    }
}
