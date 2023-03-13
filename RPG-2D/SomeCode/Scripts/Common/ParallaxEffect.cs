using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;


public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;
    public Transform followTarget;  //景深对应的基准物体。一般为Player

    public float parallaxScale; //根据视觉差情况要移动的值比例，所有使用该脚本的物件一起变
    public float parallaxReductionFactor;   //视觉差每层要变动的基础值

    public Boolean frozenY = true;  //判断一个背景图是否在Y轴上下情况下跟随我们跟踪的目标（一般为玩家），true为跟随

    private Vector2 camStartPosition;
    private Vector2 camMoveSinceStart => (Vector2)cam.transform.position - camStartPosition;
    private float zDistanceFromTarget;
    private float yDistanceFromTarget;
    private Vector3 backgroundTargetPostion;


    void Start()
    {
        camStartPosition = cam.transform.position;
        zDistanceFromTarget = transform.position.z - followTarget.transform.position.z;
        yDistanceFromTarget = transform.position.y - followTarget.transform.position.y;
        parallaxScale = 1;
        parallaxReductionFactor = 1f;
        backgroundTargetPostion = transform.position;
    }

    void Update()
    {
        //camMoveSinceStart = (Vector2)cam.transform.position - camStartPosition;

        //float newPositionX = camMoveSinceStart.x * (zDistanceFromTarget * parallaxReductionFactor * parallaxScale + 1);
        //float newPositionY = frozenY ? followTarget.transform.position.y - yDistanceFromTarget : transform.position.y;
        //backgroundTargetPostion = new Vector3(newPositionX, newPositionY, transform.position.z);
    }

    private void LateUpdate()
    {
        float newPositionX = camMoveSinceStart.x * (zDistanceFromTarget * parallaxReductionFactor * parallaxScale + 1);
        float newPositionY = frozenY ? followTarget.transform.position.y - yDistanceFromTarget : transform.position.y;
        backgroundTargetPostion = new Vector3(newPositionX, newPositionY, transform.position.z);

        transform.position = backgroundTargetPostion;
    }
}
