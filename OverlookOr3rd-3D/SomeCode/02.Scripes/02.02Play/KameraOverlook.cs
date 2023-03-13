using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KameraOverlook : MonoBehaviour {
    private Transform thisTr;
    public Transform targetTr;
    public float dampTrace = 20.0f; //平滑追踪参数
    void Start () {
        Camera.main.orthographic = true;
        thisTr = transform;
	}
	
	void Update () {
		
	}
    void LateUpdate()
    {
        Vector3 temp = targetTr.position + targetTr.up * 10;
        thisTr.position = Vector3.Lerp(thisTr.position, temp, Time.deltaTime * dampTrace);
    }
}
