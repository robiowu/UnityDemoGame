using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour {
    private Transform tr;
    private LineRenderer line;
    private RaycastHit hit;
    private GameObject rayOj;
    private bool isLaser = false;

	void Start () {
        tr = GetComponent<Transform>();
        LaserLineInit();
	}
	
	void Update () {
        if (isLaser)
        {
            if (Input.GetMouseButton(0))
            {
                rayOj.SetActive(true);
            }
            else
                rayOj.SetActive(false);
        }
    }

    void LaserLineInit()
    {
        rayOj = tr.Find("Line").gameObject;
        rayOj.SetActive(false);
    }
    void OnFire()
    {
        //rayOj.SetActive(true);
    }
    void SetLaser(bool bo)
    {
        isLaser = bo;
    }

    void LaserLineInit_abandoned()
    {
        line = GetComponent<LineRenderer>();
        //以局部坐标系为基准
        line.useWorldSpace = false;
        //游戏运行时禁用
        line.enabled = false;
    }
    void OnFire_abandoned()
    {
        //生成射线
        Ray ray = new Ray(tr.position + (Vector3.up * 0.02f), tr.forward);
        //Ray ray = new Ray(tr.position, tr.forward);
        line.SetPosition(0, tr.InverseTransformPoint(ray.origin));
        if (Physics.Raycast(ray, out hit, 100.0f)) 
        {
            line.SetPosition(1, tr.InverseTransformPoint(hit.point));
        }
        else
        {
            line.SetPosition(1, tr.InverseTransformPoint(ray.GetPoint(100.0f)));
        }
        //调用绘制协程
        StartCoroutine(this.ShowLaserBeam());
    }
    IEnumerator ShowLaserBeam()
    {
        line.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
        line.enabled = false;
    }
}
