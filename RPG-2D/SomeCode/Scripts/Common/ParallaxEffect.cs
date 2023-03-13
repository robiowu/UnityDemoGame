using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;


public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;
    public Transform followTarget;  //�����Ӧ�Ļ�׼���塣һ��ΪPlayer

    public float parallaxScale; //�����Ӿ������Ҫ�ƶ���ֵ����������ʹ�øýű������һ���
    public float parallaxReductionFactor;   //�Ӿ���ÿ��Ҫ�䶯�Ļ���ֵ

    public Boolean frozenY = true;  //�ж�һ������ͼ�Ƿ���Y����������¸������Ǹ��ٵ�Ŀ�꣨һ��Ϊ��ң���trueΪ����

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
