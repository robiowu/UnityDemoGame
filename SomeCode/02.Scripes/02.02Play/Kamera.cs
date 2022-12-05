using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamera : MonoBehaviour
{

    public Transform targetTr;  //要追踪的对象的Transform变量
    public float dist = 10.0f;  //与摄像机的水平地面投影距离，即xOz平面的投影距离
    public float height = 3.0f; //与摄像机的垂直距离，即Y轴方向的距离
    public float dampTrace = 20.0f; //平滑追踪参数

    //摄像机自身的Transform变量
    private Transform tr;
    private Vector3 finalPosition;

    void Start()
    {
        Camera.main.orthographic = false;
        //摄像机自身的Transform变量赋给tr
        tr = GetComponent<Transform>();
        finalPosition = targetTr.position - (targetTr.forward * dist) + (targetTr.up * height);
        tr.position = finalPosition;
    }


    void Update()
    {

    }
    //等所有的Update调用完毕之后才开始调用，避免画面抖动
    void LateUpdate()
    {
        Vector3 tempPosition=new Vector3();
        tempPosition = targetTr.position - (targetTr.forward * dist) + (targetTr.up * height);
        RaycastHit hit;
        Debug.DrawLine(targetTr.position, tempPosition, Color.red);
        if (Physics.Linecast(targetTr.position, tempPosition, out hit, LayerMask.GetMask("WALL")))
        {
            finalPosition = hit.point;
        }
        else
        {
            finalPosition = tempPosition;
        }
        //将摄像机位置在摄像机最终位置与target之间做插值
        //Vector3.Lerp(Vector3起点，Vector3终点，浮点格式时间（0~1之间？）)
        tr.position = Vector3.Lerp(tr.position, finalPosition, Time.deltaTime * dampTrace);
        //修改摄像机朝向target
        tr.LookAt(targetTr.position);
        //tr.forward = targetTr.forward;
    }
}
